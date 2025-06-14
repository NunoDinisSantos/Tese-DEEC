using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UnityClientProxy : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;

    TcpClient client;
    NetworkStream stream;
    private string mode = "0";

    [SerializeField] PlayerMovementWater playerMovement;
    [SerializeField] HarpoonTrigger harpoonTrigger;
    [SerializeField] private ShopProxyScript shopProxy;
    [SerializeField] private PlayerLookSimpleEDITOR playerMovementTutorial;
    [SerializeField] private DialogController dialogController;
    public StoreColliderScript storeColliderScript;
    public CallDEECSupportScript callDEECSupport;
    public CalibrationStepsScript calibrationSteps;
    public SkipTutorialExercise skipTutorialExercise;

    [HideInInspector] public float horizontal = 0;
    [HideInInspector] public float vertical = 0;
    [HideInInspector] public float horizontalLook = 0;
    [HideInInspector] public float verticalLook = 0;
    [HideInInspector] public bool isReeling = false;
    [HideInInspector] public bool inShop = false;
    [HideInInspector] public int Fire = 0;
    [HideInInspector] public bool harpoonMode = false;
    private bool currentModeHarpoon = false;
    public bool tPose = false;
    public bool armDir = false;
    public bool calibrated = false;

    [SerializeField] private TMP_InputField playerID;
    [SerializeField] private MainMenuManager menuManager;
    public bool inTutorialScene = false;
    public bool inMenuScene = false;
    public bool inGameScene = false;
    
    private bool foundTutMovement = false;

    private bool triedChangeScene = false;
    private bool connected = true;
    private Thread thread;

    void OnDestroy()
    {
        Debug.Log("ClientManager is being destroyed, closing connection...");

        connected = false; // Stop the communication loop

        if (stream != null)
        {
            stream.Close();
            Debug.Log("Stream closed.");
        }

        if (client != null)
        {
            client.Close();
            Debug.Log("Client connection closed.");
        }

        if (thread != null && thread.IsAlive)
        {
            thread.Abort(); // Forcefully stop the thread
            Debug.Log("Thread stopped.");
        }
    }

    void Start()
    {
        thread = new Thread(ConnectToServer);
        thread.Start();
    }

    void ConnectToServer()
    {
        client = new TcpClient("127.0.0.1", 25001);

        while(connected)
        {
            try
            {
                Communicate();
            }

            catch (Exception e)
            {
                stream.Close();
                client.Close();
                Debug.LogError(e.Message + " Error in connection... Not listening");
                Debug.Log(e.Message);
                thread.Abort();
                thread.Start();
                connected = true;
                Thread.Sleep(1000);
            }
        }
    }

    void Communicate()
    {

        stream = client.GetStream();
        StringBuilder data = new StringBuilder();

        if (inMenuScene)
        {
            byte[] response = Encoding.UTF8.GetBytes("m");
            stream.Write(response, 0, response.Length);
        }

        else
        {
            byte[] response = Encoding.UTF8.GetBytes(mode);
            stream.Write(response, 0, response.Length);
        }

            byte[] received = new byte[client.ReceiveBufferSize];
        int bytesRead = stream.Read(received, 0, received.Length);
        data.Append(Encoding.UTF8.GetString(received, 0, bytesRead));
        connected = true;

        ProcessBuffer(data);
    }

    private void ProcessBuffer(StringBuilder data) // We were receiving too many lines and, sometimes, the first was split. This solves it
    {
        if (data.Length > 0)
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

    private void DEECDataParser(string data)
    {
        if (inMenuScene && data.Length == 13 && data.StartsWith("ID:") && !triedChangeScene)
        {
            triedChangeScene = true;
            menuManager.CallStartGameFromProxy(data.Substring(3));
            return;
        }

        DecodeDataDEEC(data);

        if (inMenuScene)
        {
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
            /*if (!foundTutMovement)
            {
                playerMovementTutorial = ProxyHelper.instance.FindMyGameObject("tutMovement").GetComponent<PlayerLookSimpleEDITOR>();
                dialogController = playerMovementTutorial.GetComponentInParent<DialogController>();
            }*/


            if (playerMovementTutorial != null)
            {
                foundTutMovement = true;
                playerMovementTutorial.ProxyPlayerLook(horizontalLook, verticalLook);
                calibrationSteps.tPose = tPose;
                calibrationSteps.armDir = armDir;
                calibrationSteps.calibrated = calibrated;

                if (Fire == 1)
                {
                    if (dialogController != null)
                    {
                        dialogController.ClickedButtonProxy();
                    }
                }

                if (isReeling)
                {
                    skipTutorialExercise.tryingToSkip = true;
                }

                else
                {
                    skipTutorialExercise.tryingToSkip = false;
                }

                return;
            }

            return;
        }

        if (inGameScene)
        {
            calibrationSteps.tPose = tPose;
            calibrationSteps.armDir = armDir;
            calibrationSteps.calibrated = calibrated;

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

            if (!currentModeHarpoon) 
            {
                MovePlayer();
                PlayerCameraLookMovement();

                if (isReeling)
                {
                    callDEECSupport.callingSupport = true;
                }
                else
                {
                    callDEECSupport.callingSupport = false;
                }
                return;
            }

            if (currentModeHarpoon)
            {
                HandleFire();
                LockPlayerInPlace();
                harpoonTrigger.isReeling = isReeling;
                if (!isReeling && !harpoonTrigger.grabbedFish) 
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

    private void HandleHarpoonMode()
    {
        if (!AllowedToSwitchStates())
        {
            return;
        }

        currentModeHarpoon = harpoonMode;

        if (!currentModeHarpoon)
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

    private void HandleFire()
    {
        if (Fire == 1)
        {
            if (!harpoonTrigger.canFire) 
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

    private bool AllowedToSwitchStates()
    {
        return harpoonTrigger.CheckIfIsFiring();
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
        playerMovement.ProxyPlayerLook(horizontalLook, verticalLook); 
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
        tPose = convertedData.tPose;
        armDir = convertedData.armDir;
        calibrated = convertedData.calibrated;
    }

    void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
    }

    //FLASK

    //TO CALL FROM CALIBRATION!

    public IEnumerator EnableImageStream()
    {
        string url = "http://127.0.0.1:5000/set_image_stream";

        var json = "{\"enabled\": true}";
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image stream enabled.");
        }
        else
        {
            Debug.LogError("Failed to enable image stream: " + request.error);
        }
    }

    public IEnumerator GetImageCoroutine()
    {
        while (true) // Or use a flag to stop later
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture("http://127.0.0.1:5000/get_image");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                rawImage.texture = texture; // Assign to a RawImage in your UI
            }
            else
            {
                Debug.LogWarning("No image or error: " + request.error);
            }

            yield return new WaitForSeconds(0.2f); // Limit FPS to 5 per second
        }
    }


    public IEnumerator GetImageCoroutineNew()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("http://127.0.0.1:5000/get_image");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            rawImage.texture = texture; // Assign to a RawImage in your UI
        }
        else
        {
            Debug.LogWarning("No image or error: " + request.error);
        }
    }


    public IEnumerator DisableImageStream()
    {
        string url = "http://127.0.0.1:5000/set_image_stream";

        var json = "{\"enabled\": false}";
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image stream disabled.");
        }
        else
        {
            Debug.LogError("Failed to disable image stream: " + request.error);
        }
    }
}
