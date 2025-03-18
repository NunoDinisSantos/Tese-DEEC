using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Proxy : MonoBehaviour
{
    public bool inTutorialScene = false;
    public bool inMenuScene = false;
    public bool inGameScene = false;
    public Thread thread;
    [HideInInspector] public int Port = 25001;
    TcpListener server;
    public TcpClient client;
    [SerializeField] bool listening;

    [HideInInspector] [SerializeField] PlayerMovementWater playerMovement;
    [HideInInspector] [SerializeField] HarpoonTrigger harpoonTrigger;
    [SerializeField] private ShopProxyScript shopProxy;
    [SerializeField] private PlayerLookSimpleEDITOR playerMovementTutorial;
    [SerializeField] private DialogController dialogController;
    public StoreColliderScript storeColliderScript;
    public CallDEECSupportScript callDEECSupport;

    [HideInInspector] public float horizontal = 0;
    [HideInInspector] public float vertical = 0;
    [HideInInspector] public float horizontalLook = 0;
    [HideInInspector] public float verticalLook = 0;
    [HideInInspector] public bool isReeling = false;
    [HideInInspector] public bool inShop = false;
    [HideInInspector] public int Fire = 0;
    [HideInInspector] public bool harpoonMode = false;

    [SerializeField] private TMP_InputField playerID;
    [SerializeField] private MainMenuManager menuManager;
    
    private bool foundTutMovement = false;

    private string mode = "0";

    void Start()
    {
        shopProxy = GetComponent<ShopProxyScript>();
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
    }

    public void StopServer()
    {
        if (client != null)
        {
            client.Close();
        }

        if (server != null)
        {
            server.Stop();
        }

        if(thread != null && thread.IsAlive)
        {
            thread.Abort();
        }
    }

    private void GetData()
    {
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), Port); 
        server.Start();

        while (listening)
        {
            try
            {
                if (client == null || !client.Connected)
                {
                    client?.Close();
                    Debug.Log("CLIENT WAS NULL OR NOT CONNECTED");
                    client = server.AcceptTcpClient();
                }
                else
                {
                    Debug.Log("CLIENT CONNECTED: "+client);
                    Connection();
                }
            }

            catch (Exception e) 
            {
                //client.Close();
                StopServer();
                Debug.LogError(e.Message+" Error in connection... Not listening");
                Thread.Sleep(1000);
                thread.Start();
            }
        }
    }

    private void Connection()
    {
        StringBuilder data = new StringBuilder();

        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = stream.Read(buffer, 0,client.ReceiveBufferSize);

        if(bytesRead == 0)
        {
            client.Close();
            Debug.Log("Client disconnected...");
            return;
        }

        listening = true;

        byte[] response = Encoding.UTF8.GetBytes(mode);
        stream.Write(response, 0, response.Length);

        data.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

        ProcessBuffer(data);
    }

    private void DEECDataParser(string data)
    {
        Debug.Log(inMenuScene && data.Length == 13 && data.StartsWith("ID:"));
        if (inMenuScene && data.Length == 13 && data.StartsWith("ID:"))
        {
            //playerID.text = data.Substring(3);
            menuManager.CallStartGameFromProxy(data.Substring(3));
        }

        DecodeDataDEEC(data);

        if (inMenuScene)
        {
            Debug.Log(data.Length);

            if (data.Length == 10)
            {
                playerID.text = data.ToString();
            }

            shopProxy.inStore = true;

            if (shopProxy != null)
            {
                shopProxy.MoveCursorProxy(horizontalLook, verticalLook);

                if (Fire == 1 && shopProxy.canFire)
                {
                    shopProxy.firedButton = true;
                    Fire = 0;
                }
            }

            return;
        }

        if (inTutorialScene)
        {
            mode = "1";
            if (!foundTutMovement)
            {
                playerMovementTutorial = ProxyHelper.instance.FindMyGameObject("tutMovement").GetComponent<PlayerLookSimpleEDITOR>();
                dialogController = playerMovementTutorial.GetComponentInParent<DialogController>();
            }

            if (playerMovementTutorial != null)
            {
                foundTutMovement = true;
                playerMovementTutorial.ProxyPlayerLook(horizontalLook, verticalLook); // either this or the previous!?

                if (Fire == 1)
                {
                    if (dialogController != null)
                    {
                        dialogController.ClickedButtonProxy();
                    }
                }

                return;
            }
        }

        if (inGameScene)
        {
            if (storeColliderScript != null && storeColliderScript.inStore)
            {
                mode = "1";
                shopProxy.inStore = true;
                shopProxy.MoveCursorProxy(horizontalLook, verticalLook);


                if (Fire == 1 && shopProxy.canFire)
                {
                    shopProxy.firedButton = true;
                    Fire = 0;
                }

                return;
            }

            mode = "0";
            shopProxy.inStore = false;

            HandleHarpoonMode();

            if (!harpoonMode) // Normal mode -> Navegar pelo mundo
            {
                MovePlayer();
                PlayerCameraLookMovement();

                // TENTATIVA DE CHAMAR DEEC RIDE
                if(isReeling)
                {
                    callDEECSupport.callingSupport = true;
                }
                else
                {
                    callDEECSupport.callingSupport = false;
                }
            }

            if (harpoonMode)
            {
                HandleFire();
                LockPlayerInPlace();
                harpoonTrigger.isReeling = isReeling;
                if (!isReeling && !harpoonTrigger.grabbedFish) // Se estiver a reel ou com peixe, bloqueamos o camera look
                {
                    PlayerCameraLookMovement();
                }
                else
                {
                    horizontalLook = 0;
                    verticalLook = 0;
                    playerMovement.ProxyPlayerLook(horizontalLook, verticalLook);
                }
            }
        }
    }

    private void DecodeDataDEEC(string data)
    {
        var convertedData = JsonUtility.FromJson<SocketData>(data);
        horizontal = convertedData.horizontal;
        vertical = convertedData.vertical;
        horizontalLook = convertedData.horizontalLook;
        verticalLook = convertedData.verticalLook;
        isReeling = convertedData.isReeling != 0;
        harpoonMode = convertedData.harpoonMode == 1;
        Fire = convertedData.fire;
    }

    private void HandleFire()
    {
        if (Fire == 1)
        {
            if (!harpoonTrigger.canFire) // Se disparou recentemente ou está reeling, então bloqueia o disparo
            {
                Fire = 0;
            }

            else
            {
                Debug.Log("Fired from proxy");
                harpoonTrigger.FiredFromProxy = true;
            }
        }
    }

    private void HandleHarpoonMode()
    {
        if (!harpoonMode)
        {
            if (harpoonTrigger.CheckIfIsFiring())
            {
                harpoonTrigger.stopAimCalled = true;
                playerMovement.aiming = false;
                harpoonTrigger.aiming = false;
            }
        }

        else
        {
            playerMovement.aiming = true;
            harpoonTrigger.stopAimCalled = false;
            harpoonTrigger.aiming = true;
        }
    }

    private void LockPlayerInPlace()
    {
        playerMovement.PlayerMovement(0, 0);
    }

    private void MovePlayer()
    {
        playerMovement.PlayerMovement(horizontal, vertical);
    }

    private void PlayerCameraLookMovement()
    {
        //harpoonTrigger.CallAim(horizontal, vertical);
        playerMovement.ProxyPlayerLook(horizontalLook, verticalLook); // either this or the previous!?
    }

    private void ProcessBuffer(StringBuilder data) // We were receiving too many lines and, sometimes, the first was split. This solves it
    {
        if(data.Length > 0) 
        {
            string bufferContent = data.ToString();

            int startIndex = bufferContent.IndexOf("<START>");
            int endIndex = bufferContent.IndexOf("<END>");

            if (startIndex != -1 && endIndex != -1 && startIndex < endIndex)
            {
                string completeMessage = bufferContent.Substring(startIndex + 7, endIndex - (startIndex + 7));

                data.Remove(0, endIndex + 5);

                DEECDataParser(completeMessage);
            }            
        }
    }
    #region Possible_Obsolote
    //Obsolete?
    private bool GetReelingValue(int exerciseIndex)
    {
        bool reeling = false;
        // get data from 'data' string
        return reeling;
    }

    private void ObtainXYValues(string data)
    {
        if(data.Length < 3)
        {
            return;
        }

        float horizontalInput;
        float verticalInput;
        int axis = Convert.ToInt32(data[2]);

        string[] value = data.Split(" ", StringSplitOptions.None);
        string x = value[1].Remove(value[1].Length - 1);

        x = x.Replace('.', ',');

        float.TryParse(x, out float movement);

        if (axis == 48) // X axis
        {
            horizontalInput = movement;
            horizontal = horizontalInput;
        }

        if (axis == 49) // Y axis
        {
            verticalInput = movement;
            vertical = verticalInput;
        }
    }

    private void DecodeData(string data)
    {
        float horizontalInput;
        float verticalInput;
        int axis = Convert.ToInt32(data[2]);
        
        string[] value = data.Split(" ", StringSplitOptions.None);
        string x = value[1].Remove(value[1].Length - 1);

        x = x.Replace('.', ',');

        float.TryParse(x, out float movement);

        if (axis == 48) // X axis
        {
            horizontalInput = movement;
            horizontal = horizontalInput;
        }

        if (axis == 49) // Y axis
        {
            verticalInput = movement;
            vertical = verticalInput;
        }
    }

    private void ParseData(string data)
    {
        if (!harpoonMode)
        {
            if (data.Length > 1)
            {
                DecodeData(data);
                playerMovement.PlayerMovement(horizontal, vertical);
            }

            else // Buttons pressed...
            {
                if (inShop)
                {
                    shopProxy.firedButton = true;
                    return;
                }

                harpoonMode = !harpoonMode;
                if (harpoonMode)
                {
                    playerMovement.aiming = true;
                    harpoonTrigger.CallAim(horizontal, vertical);
                }
                /*else
                {
                    harpoonTrigger.stopAimCalled = true;
                    playerMovement.aiming = false;
                }
                return;*/
    }
}

        else
        {
            if (data.Length > 1)
            {
                DecodeData(data);
            }

            else
            {
                int buttonClicked = Convert.ToInt32(data[0]); // 48 -> 0 || 53 -> 5
                if (buttonClicked == 48)
                {
                    harpoonTrigger.stopAimCalled = true;
                    playerMovement.aiming = false;
                    harpoonMode = false;
                }

                if (buttonClicked == 53)
                {
                    harpoonTrigger.FiredFromProxy = true;
                }
            }
        }
    }

    private void ParseDataTwo(string data)
    {
        if (!harpoonMode)
        {
            if (data.Length > 1)
            {
                DecodeData(data);
                playerMovement.PlayerMovement(horizontal, vertical);
            }

            else // Buttons pressed...
            {
                if (inShop)
                {
                    shopProxy.firedButton = true;
                    return;
                }

                harpoonMode = !harpoonMode;
            }
        }

        if (harpoonMode)
        {
            ObtainXYValues(data); // Precisamos para tirar info do horizontal e vertical. Nome mais correcto seria (ObterValoresJoystick)
            harpoonTrigger.CallAim(horizontal, vertical);
            playerMovement.PlayerMovement(0, 0);
            int buttonClicked = Convert.ToInt32(data[0]); // 48 -> 0 || 53 -> 5
            if (buttonClicked == 48)
            {
                harpoonTrigger.stopAimCalled = true;
                playerMovement.aiming = false;
                harpoonMode = false;
            }

            if (buttonClicked == 53)
            {
                harpoonTrigger.FiredFromProxy = true;
            }
        }
    }

    private void ParseDataThree(string data)
    {
        if (!harpoonMode)
        {
            if (inShop)
            {
                shopProxy.firedButton = true;
                return;
            }

            else
            {
                DecodeData(data);
                playerMovement.PlayerMovement(horizontal, vertical);
                playerMovement.ProxyPlayerLook(horizontalLook, verticalLook);
            }

            harpoonMode = !harpoonMode;
        }

        if (harpoonMode)
        {
            harpoonTrigger.isReeling = GetReelingValue(0);
            ObtainXYValues(data); // Precisamos para tirar info do horizontal e vertical. Nome mais correcto seria (ObterValoresJoystick)
            harpoonTrigger.CallAim(horizontal, vertical);
            playerMovement.PlayerMovement(0, 0);
            int buttonClicked = Convert.ToInt32(data[0]); // 48 -> 0 || 53 -> 5
            if (buttonClicked == 48) // STOP HARPOOING MOVEMENT!!!!!!
            {
                harpoonTrigger.stopAimCalled = true;
                playerMovement.aiming = false;
                harpoonMode = false;
            }

            if (buttonClicked == 53) // FIRE MOVEMENT
            {
                harpoonTrigger.FiredFromProxy = true;
            }
        }
    }
    #endregion
}

[Serializable]
public class SocketData
{
    public float horizontal;
    public float vertical;
    public float horizontalLook;
    public float verticalLook;
    public bool FireState;
    public bool Defire;
    public int fire;
    public int ReelingIndexExercise;
    public int harpoonMode;
    public int isReeling;
}