using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Proxy : MonoBehaviour
{
    Thread thread;
    public int Port = 25001;
    TcpListener server;
    TcpClient client;
    [SerializeField] bool listening;

    [SerializeField] PlayerMovementWater playerMovement;
    [SerializeField] HarpoonTrigger harpoonTrigger;

    public float horizontal = 0;
    public float vertical = 0;
    public float horizontalLook = 0;
    public float verticalLook = 0;
    public bool isReeling = false;
    public bool inShop = false;
    public int Fired = 0;
    public bool harpoonMode = false;
    private ShopProxyScript shopProxy;

    void Start()
    {
        shopProxy = GetComponent<ShopProxyScript>();
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
    }

    private void GetData()
    {

        //server = new TcpListener(IPAddress.Any, Port); // <- funciona para qualquer dispositivo!
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), Port); // local (funciona)
        //server = new TcpListener(IPAddress.Parse("192.168.1.9"), Port); // IP DO PC não funciona n sei pq
        server.Start();

        //client = server.AcceptTcpClient();
        //listening = true;

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
                Debug.Log(client.Available);
                Connection();
                }
            }

            catch (Exception e) 
            {
                Debug.LogError(e.Message+" Error in connection... Not listening");
                listening = false;
                Thread.Sleep(1000);
            }
        }

        server.Stop();
        client?.Close();

        // NOT WORKING... Ao menos n crasha...
        while (!listening)
        {
            try
            {
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), Port); // local (funciona)
                server.Start();
                Thread.Sleep(1000);
                Debug.Log("Trying to reconnect...");
                thread.Start();
                listening = true;
            }

            catch
            {
                server.Stop();
                client?.Close();
                Debug.Log("Something went wrong");
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
            listening = false;
            return;
        }

        listening = true;

        data.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
        ProcessBuffer(data);


        /* string dataReceived = Encoding.UTF8.GetString(buffer,0,bytesRead);

        if (!string.IsNullOrEmpty(dataReceived))
        {
            DEECDataParser(dataReceived);
        }*/
    }

    private void DEECDataParser(string data)
    {
        DecodeDataDEEC(data);
        HandleHarpoonMode();

        if (!harpoonMode) // Normal mode -> Navegar pelo mundo
        {
            if (inShop)
            {
                shopProxy.firedButton = true;
                return;
            }

            else
            {
                MovePlayer();
                PlayerCameraLookMovement();
            }
        }

        if (harpoonMode)
        {
            HandleFire();
            LockPlayerInPlace();
            harpoonTrigger.isReeling = isReeling;
            if (!isReeling) // Se estiver a reel, bloqueamos o camera look
            {
                PlayerCameraLookMovement();
            }
        }
    }

    private void DecodeDataDEEC(string data)
    {
        // Recebe json com info do programa python
        //var reader = new StringReader(data); // Estamos a receber demasiada data (linhas). Asssim lemos só a primeira.
        //data = reader.ReadLine();

        var convertedData = JsonUtility.FromJson<SocketData>(data);
        horizontal = convertedData.horizontal;
        vertical = convertedData.vertical;
        horizontalLook = convertedData.horizontalLook;
        verticalLook = convertedData.verticalLook;
        isReeling = convertedData.ReelingIndexExercise != 0;
        harpoonMode = convertedData.HarpoonMode == 1;
        Fired = Convert.ToInt32(convertedData.Fire);
    }

    private void HandleFire()
    {
        if (Fired == 1)
        {
            if (!harpoonTrigger.canFire) // Se disparou recentemente ou está reeling, então bloqueia o disparo
            {
                Fired = 0;
            }

            else
            {
                harpoonTrigger.FiredFromProxy = true;
            }
        }
    }

    private void HandleHarpoonMode()
    {
        if (!harpoonMode)
        {
            harpoonTrigger.stopAimCalled = true;
            playerMovement.aiming = false;
        }

        else
        {
            playerMovement.aiming = true;
            harpoonTrigger.stopAimCalled = false;
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

    private void ProcessBuffer(StringBuilder data) // Estavamos a receber demasiadas linhas e, por vezes, a primeira vinha partida. Assim resolvemos
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
    public bool Fire;
    public int ReelingIndexExercise;
    public int HarpoonMode;
}