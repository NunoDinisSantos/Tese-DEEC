import math
import cv2
import mediapipe as mp
import numpy as np
import socket
import json
import time
from pyzbar.pyzbar import decode

from flask import Flask, request, make_response
import threading
app = Flask(__name__)
send_images_enabled = False
last_frame = None

HOST = "127.0.0.1"
PORT = 25001

server_socket = None
client = None

# Initialize MediaPipe Pose and Drawing utilities
mp_pose = mp.solutions.pose
mp_drawing = mp.solutions.drawing_utils

# Start video capture from the webcam

#cap = cv2.VideoCapture("http://192.168.1.197:8080/video")
cap = cv2.VideoCapture(0)


image_width = 480
image_height = 640
cap.set(cv2.CAP_PROP_FRAME_WIDTH, image_width)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, image_height)
target_aspect_ratio = 9 / 16

#Variables
SquatStage = "up"
SquatThreshold = 0.1
SquatTimer = 1
SquatHeadTolerance = 0.05
FeetThreshold = 1
WalkStage = None
DirectionStage = None
FireState = None
canSwitchMode = True
PlayerId = 123

head_initial = None  # To store initial head position
left_ankle_initial = None  # To store initial foot positions
right_ankle_initial = None  # To store initial foot positions
right_wrist_initial = None
left_wrist_initial = None
left_knee_initial = None
right_knee_initial = None

calibrated = False   # Flag to check if calibration is done
calibratedDirection = False
#UNITY#######################################


horizontal = 0.0
vertical = 0.0
horizontalLook = 0.0
verticalLook = -0.0
Fire = 0
ReelingIndexExercise = 0
harpoonMode = 0
futureTimerModeSwitch = 0
VerticalSpeed = 0.15
HorizontalSpeed = 0.1
isReeling = 0
firedTimer = 0


## THESE VALUES CALCULATED IN THE CALIBRATION ##
restingPointHand = 0
hipDistanceThreshold = 0.035
hipDistanceThresholdFire = 0.15
horizontalLookThresholdLeft = 0.085
horizontalLookThresholdRight= 0.040
horizontalLookThresholdLeft = 0.05
switchModeHandThreshold = 0.1
playerLookHandThreshold = 0.1
playerLookHandThresholdVerticalUp = 0.3
playerLookHandThresholdVerticalDown = 0.4

normalizor = 0

##########################################################
bypassMode = "0"
connectionLost = False

data = {
    "horizontal": horizontal,
    "vertical": vertical,
    "horizontalLook": horizontalLook,
    "verticalLook": verticalLook,
    "FireState": FireState,
    "fire": Fire,
    "ReelingIndexExercise": ReelingIndexExercise,
    "harpoonMode": harpoonMode,
    "isReeling": isReeling,
    "tPose": calibrated,
    "armDir": calibratedDirection,
    "calibrated": calibrated and calibratedDirection
}


def run_flask():
    app.run(host='127.0.0.1', port=5000)

flask_thread = threading.Thread(target=run_flask)
flask_thread.daemon = True
flask_thread.start()

@app.route('/set_image_stream', methods=['POST'])
def set_image_stream():
    global send_images_enabled
    send_images_enabled = request.json.get("enabled", False)
    return {"status": "ok"}


@app.route('/get_image', methods=['GET'])
def get_image():
    global last_frame
    if not send_images_enabled or last_frame is None:
        return '', 204

    response = make_response(last_frame)
    response.headers['Content-Type'] = 'image/jpeg'
    return response


def calculate_angle(a,b,c):
    a = np.array(a)
    b = np.array(b)
    c = np.array(c)

    radians = np.arctan2(c[1]-b[1], c[0]-b[0]) - np.arctan2(a[1]-b[1],a[0]-b[0])
    angle = np.abs(radians*180.0/np.pi)

    if angle > 180.0:
        angle = 360-angle
    return angle

def calculate_distance(a,b):

    return math.fabs(a-b)

def calculate_distanceVec(a,b):
    a = np.array(a)
    b = np.array(b)

    return np.linalg.norm(a - b)

def connect_to_server():
    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.connect((HOST, PORT))
        return sock
    except Exception as e:
        print(f"Connection failed: {e}. Retrying...")
        time.sleep(1)

def reconnect_to_server():
    #while True:
    global connectionLost
    try:
        new_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        new_sock.connect((HOST, PORT))
        print("✅ Reconnected to Unity server.")
        connectionLost = False
        return new_sock
    except Exception as e:
        print(f"❌ Reconnection failed: {e}. Retrying in 1 second...")
        time.sleep(1)  # Wait before retrying

def send_data():
    global data
    global connectionLost
    global bypassMode
    global server_socket
    global client
    global calibrated
    global calibratedDirection
    global isReeling

    try:
        gameData = f"<START>{json.dumps(data)}<END>"
        client.sendall(gameData.encode())  # Send response back
        bypassMode = int(client.recv(1024).decode("utf-8").strip())


    except Exception as e:
        print(f"⚠️ Connection lost: {e}. Reconnecting...")
        connectionLost = True
        client.close()
        client= None
        calibrated = False
        calibratedDirection = False
        data["horizontalLook"] = 0
        data["verticalLook"] = 0
        data["vertical"] = 0
        data["fire"] = 0
        data["harpoonMode"] = 0
        SquatStage == "up"
        data["isReeling"] = 0
        data["tPose"] = 0
        data["armDir"] = 0
        data["calibrated"] = 0
        isReeling = 0


def calculate_pose():
    #region variables
    global calibrated
    global calibratedDirection
    global restingPointHand
    global image
    global canSwitchMode
    global futureTimerModeSwitch
    global harpoonMode
    global horizontalLookThresholdRight
    global horizontalLookThresholdLeft
    global horizontalLookSpeed
    global switchModeHandThreshold
    global playerLookHandThresholdVerticalUp
    global playerLookHandThresholdVerticalDown
    global playerLookHandThreshold
    global VerticalSpeed
    global VerticalSpeed
    global hipDistanceThreshold
    global SquatStage
    global SquatTimer
    global SquatHeadTolerance
    global SquatThreshold
    global firedTimer
    global hipDistanceThresholdFire
    global connectionLost
    global normalizor
    global bypassMode
    global PlayerId
    global client
    global last_frame
    
    frame_count = 0
    scanned = False
    moving = False
    movingTimer = 0.5


    PROCESS_EVERY_N_FRAMES = 2
    #endregion
    # Initialize MediaPipe Pose
    with mp_pose.Pose(
            min_detection_confidence=0.5,
            min_tracking_confidence=0.5
    ) as pose:

        while cap.isOpened():
            #region Image Processing
            try:
                client.send(b"ping")
            except Exception as e:
                print("Client disconnected:", e)
                connectionLost = True
                client = None
                calibrated = False
                data["tPose"] = 0
                data["armDir"] = 0
                data["calibrated"] = 0
                calibratedDirection = False
                data["horizontalLook"] = 0
                data["verticalLook"] = 0
                data["vertical"] = 0
                data["fire"] = 0
                data["harpoonMode"] = 0
                SquatStage = "up"
                data["isReeling"] = 0
                isReeling = 0

            success, frame = cap.read()

            #######################CHANGE ASPECT RATIO???

            frame_count += 1

            if frame_count % PROCESS_EVERY_N_FRAMES != 0:
                continue

            #Tentar rodar camara para jogador ficar mais perto
            ##PARA CAMARA QUE VAI SER USADA. USAR ISTO:
            frame = cv2.rotate(frame, cv2.ROTATE_90_COUNTERCLOCKWISE)


            #if frame_count % 2 == 0:
            frame_resized = cv2.resize(frame, (320, 240))
            _, jpeg = cv2.imencode('.jpg', frame_resized, [cv2.IMWRITE_JPEG_QUALITY, 60])
            last_frame = jpeg.tobytes()

            # RGB to send to mediapipe
            image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

            # Make detection
            if frame_count % PROCESS_EVERY_N_FRAMES == 0:
                results = pose.process(image)


            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

            '''
            if results.pose_landmarks:
                nose = results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_HIP]
                distance = nose.z  # Negative values mean closer to the camera


                print(f"{distance}---{distance<=-0.5}")
                if distance <= -0.5:  # Only track people closer than 1.5m
                    mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)
                else:
                    results.pose_landmarks = None  # Clear the pose to prevent tracking
            '''
            #To draw the dots, uncomment this
            #mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)


            # Display the image
            #endregion

            #region Calibrate

            if client is None:
                print("Server ready, waiting for connection...")
                client, addr = server_socket.accept()
                print(f"Connection from {addr}")

            try:
                inMenu = client.recv(1024).decode("utf-8").strip()
            except:
                pass

            if calibrated == False or calibratedDirection == False:
                if inMenu == "mm":  # meaning it is in menu
                    try:
                        decodedText = ""
                        decoded_QR = decode(frame_resized)

                        if decoded_QR:
                            obj = decoded_QR[0]
                            decodedText = obj.data.decode('utf-8')
                            print(f"QR Code Detected: {decodedText}")

                            if decodedText != "":
                                scanningPlayerId = int(decodedText)

                                # Only update if it's a *new* scan
                                if not scanned or scanningPlayerId != PlayerId:
                                    PlayerId = scanningPlayerId
                                    print(f"✅ PlayerId scanned: {PlayerId}")
                                    try:
                                        myData = f"<START>ID:{PlayerId}<END>"
                                        client.sendall(myData.encode("utf-8"))
                                        scanned = True
                                    except Exception as e:
                                        print(f"⚠️ Failed to send data: {e}")

                        '''
                        decoded_QR = decode(frame_resized)
                        
                        if decoded_QR:
                            # Only process the first detected QR code
                            obj = decoded_QR[0]
                            decodedText = obj.data.decode('utf-8')
                            print(decodedText)

                            print(f"QR Code Detected: {decodedText}")

                        if decodedText != "":
                            scanningPlayerId = int(decodedText)
                            if scanningPlayerId != PlayerId:
                                scanned = False

                            if scanned == False:
                                PlayerId = scanningPlayerId
                                print(f"✅ PlayerId scanned: {PlayerId}")
                                try:
                                    myData = f"<START>ID:{PlayerId}<END>"
                                    client.sendall(myData.encode("utf-8"))
                                    scanned = True
                                except Exception as e:
                                    print(f"⚠️ Failed to send data: {e}")  # ✅ Print error for debugging
                            '''
                    except:
                        pass

                else:
                    if calibrated == False:
                        cv2.putText(image, str("Not Calibrated"), (25, 25), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2,
                            cv2.LINE_AA)

                    if calibratedDirection == False and calibrated == True:
                        cv2.putText(image, str("Direction arm not calibrated"), (25, 25), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 255), 1,
                                    cv2.LINE_AA)


                    try:

                        landmarks = results.pose_landmarks.landmark

                        head = landmarks[mp_pose.PoseLandmark.NOSE.value].y


                        #region normal
                        '''                    
                        left_shoulder = [landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].x,
                                         landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].y]
                        left_elbow = [landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].x,
                                      landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].y]
                        left_wrist = [landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].x,
                                      landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].y]
                        right_shoulder = [landmarks[mp_pose.PoseLandmark.RIGHT_SHOULDER.value].x,
                                          landmarks[mp_pose.PoseLandmark.RIGHT_SHOULDER.value].y]
                        right_elbow = [landmarks[mp_pose.PoseLandmark.RIGHT_ELBOW.value].x,
                                       landmarks[mp_pose.PoseLandmark.RIGHT_ELBOW.value].y]
                        right_wrist = [landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value].x,
                                       landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value].y]
                        left_ankle = [landmarks[mp_pose.PoseLandmark.LEFT_ANKLE.value].x,
                                      landmarks[mp_pose.PoseLandmark.LEFT_ANKLE.value].y]
                        left_knee = [landmarks[mp_pose.PoseLandmark.LEFT_KNEE.value].x,
                                     landmarks[mp_pose.PoseLandmark.LEFT_KNEE.value].y]
                        left_hip = [landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].x,
                                    landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].y]
                        right_ankle = [landmarks[mp_pose.PoseLandmark.RIGHT_ANKLE.value].x,
                                       landmarks[mp_pose.PoseLandmark.RIGHT_ANKLE.value].y]
                        right_knee = [landmarks[mp_pose.PoseLandmark.RIGHT_KNEE.value].x,
                                      landmarks[mp_pose.PoseLandmark.RIGHT_KNEE.value].y]
                        right_hip = [landmarks[mp_pose.PoseLandmark.RIGHT_HIP.value].x,
                                     landmarks[mp_pose.PoseLandmark.RIGHT_HIP.value].y]
                        
                        '''
                        #endregion
                        #region Inverted
                        right_shoulder = [landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].x,
                                         landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].y]
                        right_elbow = [landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].x,
                                      landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].y]
                        right_wrist = [landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].x,
                                      landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].y]
                        left_shoulder = [landmarks[mp_pose.PoseLandmark.RIGHT_SHOULDER.value].x,
                                          landmarks[mp_pose.PoseLandmark.RIGHT_SHOULDER.value].y]
                        left_elbow = [landmarks[mp_pose.PoseLandmark.RIGHT_ELBOW.value].x,
                                       landmarks[mp_pose.PoseLandmark.RIGHT_ELBOW.value].y]
                        left_wrist = [landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value].x,
                                       landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value].y]
                        right_ankle = [landmarks[mp_pose.PoseLandmark.LEFT_ANKLE.value].x,
                                      landmarks[mp_pose.PoseLandmark.LEFT_ANKLE.value].y]
                        right_knee = [landmarks[mp_pose.PoseLandmark.LEFT_KNEE.value].x,
                                     landmarks[mp_pose.PoseLandmark.LEFT_KNEE.value].y]
                        right_hip = [landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].x,
                                    landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].y]
                        left_ankle = [landmarks[mp_pose.PoseLandmark.RIGHT_ANKLE.value].x,
                                       landmarks[mp_pose.PoseLandmark.RIGHT_ANKLE.value].y]
                        left_knee = [landmarks[mp_pose.PoseLandmark.RIGHT_KNEE.value].x,
                                      landmarks[mp_pose.PoseLandmark.RIGHT_KNEE.value].y]
                        left_hip = [landmarks[mp_pose.PoseLandmark.RIGHT_HIP.value].x,
                                     landmarks[mp_pose.PoseLandmark.RIGHT_HIP.value].y]
                        #endregion

                        leftDownAngle = calculate_angle(left_hip, left_shoulder, left_elbow)  # 90 a 110
                        leftUpAngle = calculate_angle(left_shoulder, left_elbow, left_wrist)  # 170 a 180
                        rightDownAngle = calculate_angle(right_hip, right_shoulder, right_elbow)  # 90 a 110
                        rightUpAngle = calculate_angle(right_shoulder, right_elbow, right_wrist)  # 170 a 180

                        ankleDistance = calculate_distance(right_ankle[0], left_ankle[0])  # 0.08 max
                        kneeDistance = calculate_distance(right_knee[0], left_knee[0])  # 0.25 max

                        normalizor = calculate_distance(left_ankle[1], left_shoulder[1])

                        #print(
                           # f"Wrist distance is : {calculate_distance(right_wrist[0],left_wrist[0]) < 0.1 * normalizor}") # TO CHECK ARM DIR START




                        if leftDownAngle > 90 and leftDownAngle < 110 and leftUpAngle > 145 and leftUpAngle < 180 and rightDownAngle > 90 and rightDownAngle < 110 and rightUpAngle > 145 and rightUpAngle < 180 and ankleDistance < 0.08 and kneeDistance < 0.25:
                            if not calibrated:  # Save initial positions once
                                calibrated = True
                                data["tPose"] = True
                                send_data()
                                head_initial = head
                                right_wrist_initial = right_wrist



                                print("✅ T-Pose detected! Initial calibration complete.")


                        if calibrated and calibratedDirection == False:
                            #print(calculate_angle(right_elbow, right_shoulder, right_hip))
                            if calculate_distance(right_wrist[0],left_wrist[0]) < 0.1 * normalizor:
                                restingPointHand = left_wrist
                                print("✅ Saving resting point of direction.")
                                calibratedDirection = True
                                data["armDir"] = True
                                send_data()




                    except:
                        pass
            #endregion

            if calibratedDirection:
                cv2.putText(image, str("Calibrated"), (25, 25), cv2.FONT_HERSHEY_SIMPLEX, 1, (5, 255, 50), 2,
                            cv2.LINE_AA)
                try:
                    scanned = False

                    #region Body landmarks
                    landmarks = results.pose_landmarks.landmark
                    head = landmarks[mp_pose.PoseLandmark.NOSE.value].y
                    '''
                    left_shoulder = [landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].x,
                                     landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].y]
                    left_elbow = [landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].x,
                                  landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].y]
                    left_wrist = [landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].x,
                                  landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].y]
                    right_shoulder = [landmarks[mp_pose.PoseLandmark.RIGHT_SHOULDER.value].x,
                                      landmarks[mp_pose.PoseLandmark.RIGHT_SHOULDER.value].y]
                    right_elbow = [landmarks[mp_pose.PoseLandmark.RIGHT_ELBOW.value].x,
                                   landmarks[mp_pose.PoseLandmark.RIGHT_ELBOW.value].y]
                    right_wrist = [landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value].x,
                                   landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value].y]
                    left_ankle = [landmarks[mp_pose.PoseLandmark.LEFT_ANKLE.value].x,
                                  landmarks[mp_pose.PoseLandmark.LEFT_ANKLE.value].y]
                    left_knee = [landmarks[mp_pose.PoseLandmark.LEFT_KNEE.value].x,
                                 landmarks[mp_pose.PoseLandmark.LEFT_KNEE.value].y]
                    left_hip = [landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].x,
                                landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].y]
                    right_ankle = [landmarks[mp_pose.PoseLandmark.RIGHT_ANKLE.value].x,
                                   landmarks[mp_pose.PoseLandmark.RIGHT_ANKLE.value].y]
                    right_knee = [landmarks[mp_pose.PoseLandmark.RIGHT_KNEE.value].x,
                                  landmarks[mp_pose.PoseLandmark.RIGHT_KNEE.value].y]
                    right_hip = [landmarks[mp_pose.PoseLandmark.RIGHT_HIP.value].x,
                                 landmarks[mp_pose.PoseLandmark.RIGHT_HIP.value].y]
                     '''
                    right_shoulder = [landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].x,
                                     landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].y]
                    right_elbow = [landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].x,
                                  landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].y]
                    right_wrist = [landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].x,
                                  landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].y]
                    left_shoulder = [landmarks[mp_pose.PoseLandmark.RIGHT_SHOULDER.value].x,
                                      landmarks[mp_pose.PoseLandmark.RIGHT_SHOULDER.value].y]
                    left_elbow = [landmarks[mp_pose.PoseLandmark.RIGHT_ELBOW.value].x,
                                   landmarks[mp_pose.PoseLandmark.RIGHT_ELBOW.value].y]
                    left_wrist = [landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value].x,
                                   landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value].y]
                    right_ankle = [landmarks[mp_pose.PoseLandmark.LEFT_ANKLE.value].x,
                                  landmarks[mp_pose.PoseLandmark.LEFT_ANKLE.value].y]
                    right_knee = [landmarks[mp_pose.PoseLandmark.LEFT_KNEE.value].x,
                                 landmarks[mp_pose.PoseLandmark.LEFT_KNEE.value].y]
                    right_hip = [landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].x,
                                landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].y]
                    left_ankle = [landmarks[mp_pose.PoseLandmark.RIGHT_ANKLE.value].x,
                                   landmarks[mp_pose.PoseLandmark.RIGHT_ANKLE.value].y]
                    left_knee = [landmarks[mp_pose.PoseLandmark.RIGHT_KNEE.value].x,
                                  landmarks[mp_pose.PoseLandmark.RIGHT_KNEE.value].y]
                    left_hip = [landmarks[mp_pose.PoseLandmark.RIGHT_HIP.value].x,
                                 landmarks[mp_pose.PoseLandmark.RIGHT_HIP.value].y]
                    right_foot = landmarks[mp_pose.PoseLandmark.RIGHT_FOOT_INDEX.value].y
                    left_foot = landmarks[mp_pose.PoseLandmark.LEFT_FOOT_INDEX.value].y


                    #endregion

                    #Calculate normalizor
                    normalizor = calculate_distance(left_hip[1], left_shoulder[1])


                    #region Moving in Water
                    if(data["harpoonMode"] == 0 and bypassMode == 0):
                        #region MoveForward
                        if SquatStage == "down":
                            data["vertical"] = 0
                        else:
                            #if calculate_distance(right_ankle[1], right_ankle_initial[1]) > normalizor * 0.1 or calculate_distance(left_ankle[1], left_ankle_initial[1]) > normalizor * 0.1:
                            #print(f"right_foot is at {right_foot} and needs to be higher than: {left_foot + normalizor * 0.15}")
                            #print(right_foot > left_foot + normalizor * 0.12)
                            if right_foot > left_foot + normalizor * 0.12 or left_foot > right_foot + normalizor * 0.12:
                                moving = True
                                movingTimer = time.time() + 0.5

                            else:
                                if time.time() > movingTimer:
                                    moving = False

                            if moving == True:
                                data["vertical"] = -1
                            else:
                                data["vertical"] = 0


                #endregion

                    # region Fire
                    else:
                        ##########################################
                        ##########################################
                        ##########################################
                        ##########################################
                        if 60 < calculate_angle(right_elbow,right_shoulder, right_hip) < 90 and calculate_distance(right_wrist[0], right_hip[0]) > normalizor*0.65:
                            if time.time() > firedTimer:
                                data["fire"] = 1
                                firedTimer = time.time()+1
                        else:
                            data["fire"] = 0
                    #endregion

                    #region SwitchModes
                    if (right_wrist[1] < head_initial and calculate_distance(right_wrist[0], right_shoulder[0]) > normalizor*0.1) and canSwitchMode:  # é a mao esquerda n sei pq
                        canSwitchMode = False
                        if harpoonMode == 0:
                            harpoonMode = 1
                            data["harpoonMode"] = 1
                            data["vertical"] = 0
                        else:
                            harpoonMode = 0
                            data["harpoonMode"] = 0

                        futureTimerModeSwitch = time.time()+1.5
                    if time.time() > futureTimerModeSwitch and canSwitchMode == False:
                        canSwitchMode = True


                    #endregion

                    #region DoSquat

                    if head > head_initial+normalizor*0.45 and SquatStage == "up":
                        SquatStage = "down"
                        SquatTimer = time.time() + 1.5
                    #if head < head_initial + SquatHeadTolerance and SquatStage == "down":
                    if head < head_initial+normalizor*0.45 and SquatStage == "down":
                        SquatStage = "up"
                        if time.time() < SquatTimer:
                            data["isReeling"] = 1
                            isReeling = 1
                            SquatTimer = time.time() + 1.5

                    if time.time() > SquatTimer:
                        data["isReeling"] = 0
                        isReeling = 0

                    #endregion

                    #region PlayerLook
                    if SquatStage == "up" and isReeling == 0:
                        dirX = left_wrist[0] - restingPointHand[0]
                        dirY = left_wrist[1] - restingPointHand[1]

                        multiplier = 3

                        data["horizontalLook"] = dirX * -multiplier
                        data["verticalLook"] = dirY * multiplier
                    else:
                        data["horizontalLook"] = 0
                        data["verticalLook"] = 0
                    #endregion

                    #region New Calibration
                    if calculate_distance(left_wrist[0], right_wrist[0]) < normalizor * 0.1 and left_wrist[
                        1] < head_initial - normalizor * 0.1 and right_wrist[
                        1] < head_initial - normalizor * 0.1:
                        print(f"⚠️ Recalibration in progress. Do T-Pose and arm direction pose...")
                        calibrated = False
                        calibratedDirection = False
                        data["horizontalLook"] = 0
                        data["verticalLook"] = 0
                        data["vertical"] = 0
                        data["fire"] = 0
                        data["harpoonMode"] = 0
                        SquatStage = "up"
                        data["isReeling"] = 0
                        isReeling = 0
                        data["tPose"] = False
                        data["armDir"] = False
                        data["calibrated"] = False
                        scanned = False
                        send_data()


                    #endregion

                    #if connectionLost:
                    #    sock = reconnect_to_server()
                    #else:
                    #    send_data(sock)

                    send_data()

                    #if connectionLost == False:
                    #    send_data(client)
                    #else:
                        #client.close()
                        #client, addr = server_socket.accept()
                    #    connectionLost = True

                except:
                    pass
            #Show video. To be commented


            #image = cv2.rotate(image, cv2.ROTATE_90_CLOCKWISE)
            cv2.imshow("MediaPipe Pose", image)
            if cv2.waitKey(5) & 0xFF == ord('q'):
                break

    # Release the video capture and close all OpenCV windows
    cap.release()
    cv2.destroyAllWindows()

def start_server():
    global server_socket
    global client

    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((HOST, PORT))
    server_socket.listen(1)  # Allow only one client at a time
    print("✅ Server started. Waiting for a connection...")

def main():
    #connect_to_server()
    start_server()
    calculate_pose()

if __name__ == "__main__":
    main()