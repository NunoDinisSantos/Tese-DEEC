#import pygame
import socket
import json
import time

#host = "192.168.1.5" #'''IP DA INTERNET DO TLM!'''
host = "127.0.0.1"# localhost
port = 25001
##########################################################################################
#PARA JOYSTICK. SE USAR MEDIAPIPE, NÃO É NECESSÁRIO#######################################
##########################################################################################
##########################################################################################
#pygame.joystick.init()
#joysticks = [pygame.joystick.Joystick(x) for x in range(pygame.joystick.get_count())]
#clock = pygame.time.Clock
#pygame.init()
##########################################################################################
##########################################################################################


horizontal = 0.0
vertical = 0.0
horizontalLook = 0.0
verticalLook = -0.0
FireState = False
Fired = False
ReelingIndexExercise = 0
harpoonMode = 0

data = {
    "horizontal": horizontal,
    "vertical": vertical,
    "horizontalLook": horizontalLook,
    "verticalLook": verticalLook,
    "FireState": FireState,
    "Fire": Fired,
    "ReelingIndexExercise": ReelingIndexExercise,
    "harpoonMode" : harpoonMode
}
#A tentar no method
#sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
#sock.connect((host, port)) # Tentar sempre ter ligação

#receber data através de MediaPipe
def connect_to_server():
    while True:
        try:
            sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            sock.connect((host, port))
            print("Connected to Unity server.")
            return sock
        except Exception as e:
            print(f"Connection failed: {e}. Retrying...")
            time.sleep(1)

def send_data(sock):
    try:
        while True:

            ##MEDIA PIPE VARIABLES HERE####
            data["ReelingIndexExercise"] = 1
            data["verticalLook"] = -1
##############################################################

            gameData = f"<START>{json.dumps(data)}<END>"
            sock.sendall(gameData.encode("utf-8"))
            time.sleep(1 / 30)
    except (socket.error, BrokenPipeError) as e:
        print(f"Connection lost: {e}. Reconnecting...")
        sock.close()
        main()  # Restart the process


def main():
    sock = connect_to_server()
    send_data(sock)

if __name__ == "__main__":
    main()

#while True:

    #data["ReelingIndexExercise"] = 1
    #data["horizontalLook"] = 0
    #data["verticalLook"] = -0.5

    #encontrar maneira de só enviar UMA linha COMPLETA de info!!!!
    #gameData = json.dumps(data).encode("utf-8") + b"\n"
    #sock.sendall(gameData)

    #gameData = f"<START>{json.dumps(data)}<END>"
    #sock.sendall(gameData.encode("utf-8"))
    #time.sleep(1 / 30)




    ##########################################################################################
    #######################################FOR JOYSTICK#######################################
    ##########################################################################################
    ##########################################################################################
    #for event in pygame.event.get():
    #    print(event.dict)

       # if event.type == 1536: # é axis
        #    value = event.dict.get("value")
         #   axis = event.dict.get("axis")
          #  x = "A"+str(axis)+" "+str(value)
           # sock.sendall(repr(x).encode("utf-8"))

        #if event.type == 1539: # 1540 is button up  -- button down is 1539
        #    button = event.dict.get("button")
        #    sock.sendall(repr(button).encode("utf-8"))
##########################################################################################
##########################################################################################
##########################################################################################