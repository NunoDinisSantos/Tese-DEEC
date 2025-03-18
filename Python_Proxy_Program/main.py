import math
import cv2
import mediapipe as mp
import numpy as np
import socket
import json
import time

# Initialize MediaPipe Pose and Drawing utilities
mp_pose = mp.solutions.pose
mp_drawing = mp.solutions.drawing_utils

# Start video capture from the webcam
#cap = cv2.VideoCapture("http://10.231.215.47:8080/video")
cap = cv2.VideoCapture(0)
image_width = 640
image_height = 480
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)


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
host = "127.0.0.1"# localhost
port = 25001

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
    "isReeling": isReeling
}

'''
def confirm_receive(sock):
    global data
    try:
        #while True:
        sock.settimeout(0.5)  # Ensure recv() doesn't hang forever
        sock.sendall(b"Ping")
        result = sock.recv(1024)
        print(result)
    except socket.timeout:
        print("❌ No response from Unity. Retrying...")
        sock.close()
        sock = reconnect_to_server()
'''
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
        sock.connect((host, port))
        return sock
    except Exception as e:
        print(f"Connection failed: {e}. Retrying...")
        time.sleep(1)

def reconnect_to_server():
    #while True:
    global connectionLost
    try:
        new_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        new_sock.connect((host, port))
        print("✅ Reconnected to Unity server.")
        connectionLost = False
        return new_sock
    except Exception as e:
        print(f"❌ Reconnection failed: {e}. Retrying in 1 second...")
        time.sleep(1)  # Wait before retrying

def send_data(sock):
    global data
    global connectionLost
    global bypassMode

    try:
        gameData = f"<START>{json.dumps(data)}<END>"
        sock.settimeout(1)
        sock.sendall(gameData.encode("utf-8"))

        bypassMode = int(sock.recv(1024).decode("utf-8").strip()) # whenever in store or tutorial, we bypass the harpoon mode so we can use fire. This value comes from Unity
        #print(bypassMode)

    except Exception as e:
        print(f"⚠️ Connection lost: {e}. Reconnecting...")
        connectionLost = True
        sock.close()
def calculate_pose(sock):
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
    frame_count = 0
    scanned = False

    PROCESS_EVERY_N_FRAMES = 2
    #endregion

    # Initialize MediaPipe Pose
    with mp_pose.Pose(
            min_detection_confidence=0.5,
            min_tracking_confidence=0.5
    ) as pose:

        while cap.isOpened():
            #region Image Processing

            success, frame = cap.read()

            frame_count += 1

            if frame_count % PROCESS_EVERY_N_FRAMES != 0:
                continue


            # RGB to send to mediapipe
            image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

            # Make detection
            if frame_count % PROCESS_EVERY_N_FRAMES == 0:
                results = pose.process(image)

            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

            if frame_count % 5 == 0:  # Draw every 5 frames
                mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)



            #mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)
            # Display the image
            #endregion

            #region Calibrate

            if calibrated == False or calibratedDirection == False:
                detector = cv2.QRCodeDetector()
                decodedText, points, _ = detector.detectAndDecode(image)


                if decodedText != "":
                    scanningPlayerId = int(decodedText)
                    if scanningPlayerId != PlayerId:
                        scanned = False

                    if scanned == False:
                        PlayerId = scanningPlayerId
                        print(f"✅ PlayerId scanned: {PlayerId}")
                        try:
                            myData = f"<START>ID:{PlayerId}<END>"
                            sock.sendall(myData.encode("utf-8"))
                            print("Data send")
                            scanned = True
                        except Exception as e:
                            print(f"⚠️ Failed to send data: {e}")  # ✅ Print error for debugging

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


                    #print(leftDownAngle > 85 and leftDownAngle < 110 and leftUpAngle > 170 and leftUpAngle < 180)
                    #print(f"{leftDownAngle}x{leftUpAngle}")
                    #print(rightDownAngle > 90 and rightDownAngle < 110 and rightUpAngle > 170 and rightUpAngle < 180)
                    normalizor = calculate_distance(left_hip[1], left_shoulder[1])

                    #cv2.putText(image, str("O"), tuple(np.multiply(head+normalizor*0.45,[640,480]).astype(int)), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2,
                    #            cv2.LINE_AA)
                    #print(f"{head},{head+normalizor*0.6}")

                    #cv2.putText(image, str("YOOO"), (head, head-normalizor*0.6), cv2.FONT_HERSHEY_SIMPLEX, 1,
                    #            (0, 0, 255), 1,cv2.LINE_AA)

                    if leftDownAngle > 90 and leftDownAngle < 110 and leftUpAngle > 145 and leftUpAngle < 180 and rightDownAngle > 90 and rightDownAngle < 110 and rightUpAngle > 145 and rightUpAngle < 180 and ankleDistance < 0.08 and kneeDistance < 0.25:
                        if not calibrated:  # Save initial positions once
                            calibrated = True

                            head_initial = head
                            right_wrist_initial = right_wrist
                            left_wrist_initial = left_wrist
                            left_knee_initial = left_knee
                            right_knee_initial = right_knee

                            #print(thresholdDistance)
                            print("✅ T-Pose detected! Initial calibration complete.")

                    if calibrated and calibratedDirection == False:
                        if right_wrist[1] < head_initial:
                            restingPointHand = left_wrist
                            print("✅ Saving resting point of direction.")
                            calibratedDirection = True


                except:
                    pass
            #endregion

            #if calibrated:
            if calibratedDirection:
                cv2.putText(image, str("Calibrated"), (25, 25), cv2.FONT_HERSHEY_SIMPLEX, 1, (5, 255, 50), 2,
                            cv2.LINE_AA)
                try:
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
                    #endregion

                    #Calculate normalizor
                    normalizor = calculate_distance(left_hip[1], left_shoulder[1])

                    #region Moving in Water
                    if(data["harpoonMode"] == 0 and bypassMode == 0):
                        #region MoveForward
                        if SquatStage == "down":
                            data["vertical"] = 0
                        else:
                            #if(math.fabs(right_knee[1]-right_knee_initial[1])>normalizor*0.1 or math.fabs(left_knee[1]-left_knee_initial[1])>normalizor*0.1):
                            # print(f"{calculate_distance(right_knee[1], right_knee_initial[1]) > normalizor * 0.2} -- {calculate_distance(left_knee[1], left_knee_initial[1]) > normalizor * 0.2}")
                            if calculate_distance(right_knee[1], right_knee_initial[1]) > normalizor * 0.1 or calculate_distance(left_knee[1], left_knee_initial[1]) > normalizor * 0.1:
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
                        #print(f"{20 < calculate_angle(right_wrist,right_shoulder,right_hip) < 75} ---- {calculate_distance(right_wrist[0], right_hip[0]) > normalizor*0.1}")
                        if 20 < calculate_angle(right_wrist,right_shoulder,right_hip) < 75 and calculate_distance(right_wrist[0], right_hip[0]) > normalizor*0.25:
                            if time.time() > firedTimer:
                                data["fire"] = 1
                                firedTimer = time.time()+1
                        else:
                            data["fire"] = 0
                    #endregion

                    #region SwitchModes
                    if (right_wrist[1] < right_wrist_initial[1]-switchModeHandThreshold and canSwitchMode):  # é a mao esquerda n sei pq
                        canSwitchMode = False
                        if harpoonMode == 0:
                            harpoonMode = 1
                            data["harpoonMode"] = 1
                        else:
                            harpoonMode = 0
                            data["harpoonMode"] = 0

                        futureTimerModeSwitch = time.time()+1.5
                    if time.time() > futureTimerModeSwitch and canSwitchMode == False:
                        canSwitchMode = True


                    #endregion

                    #region DoSquat

                    #if head > head_initial + SquatThreshold and SquatStage == "up":
                    if head > head_initial+normalizor*0.45 and SquatStage == "up":
                        SquatStage = "down"
                        SquatTimer = time.time() + 1.5
                        #print("Started squat. Has 1 seconds to complete")
                    #if head < head_initial + SquatHeadTolerance and SquatStage == "down":
                    if head < head_initial+normalizor*0.45 and SquatStage == "down":
                        SquatStage = "up"
                        #print("got back up")
                        if time.time() < SquatTimer:
                            data["isReeling"] = 1
                            isReeling = 1
                            #print("reeling 1")
                            SquatTimer = time.time() + 1.5
                            #print("Squat successfull. Next timer is in 1.5 sec")

                    if time.time() > SquatTimer:
                        #print("reeling 0")
                        data["isReeling"] = 0
                        isReeling = 0

                    #endregion

                    #region PlayerLook
                    if SquatStage == "up" and isReeling == 0:

                    #print(calculate_distanceVec(left_wrist, left_hip))
                    #resting pose
                        if left_wrist[1] > left_hip[1]:
                            data["horizontalLook"] = 0
                            data["verticalLook"] = 0
                        else:
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
                    #print(f"{calculate_distance(left_wrist[0], right_wrist[0]) < normalizor * 0.05} --- { left_wrist[1] < head_initial-normalizor*0.1}")
                    #print(f"{calculate_distance(left_wrist[0], right_wrist[0])} ---- {normalizor * 0.05}")

                    if calculate_distance(left_wrist[0], right_wrist[0]) < normalizor * 0.1 and left_wrist[1] < head_initial-normalizor*0.1:
                        print(f"⚠️ Recalibration in progress. Do T-Pose and arm direction pose...")
                        calibrated = False
                        calibratedDirection = False
                        data["horizontalLook"] = 0
                        data["verticalLook"] = 0
                        data["vertical"] = 0
                        data["fire"] = 0
                        data["harpoonMode"] = 0
                        SquatStage == "up"
                        data["isReeling"] = 0
                        isReeling = 0


                    #endregion

                    if connectionLost:
                        sock = reconnect_to_server()
                    else:
                        send_data(sock)

                    #bypassMode = sock.recv(1024)
                    #print(bypassMode)

                except:
                    pass
            #Show video. To be commented
            cv2.imshow("MediaPipe Pose", image)
            if cv2.waitKey(5) & 0xFF == ord('q'):
                break

    # Release the video capture and close all OpenCV windows
    cap.release()
    cv2.destroyAllWindows()

def main():
    sock = connect_to_server()
    calculate_pose(sock)

if __name__ == "__main__":
    main()