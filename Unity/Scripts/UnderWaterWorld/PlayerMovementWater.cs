using UnityEngine;

public class PlayerMovementWater : MonoBehaviour
{
    [Header("UNITY TESTING")]
    public bool unityEditor = true;
    [SerializeField]
    bool EasyControls = true;

    [Header("Misc")]
    [SerializeField] private Material skyboxMat;
    [SerializeField] private AudioSource underWaterSound;
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private float volumeEngine = 0;
    [SerializeField] private float volumeEngineMultiplier = 1;
    [SerializeField] private GameObject[] ZonesToDeActivate;
    [SerializeField] private Camera mainCamera;
    private Camera thisCamera;
    [SerializeField] private AudioSource surfaceSound;

    [SerializeField] private bool inSpace = false;
    public bool ByPassWater = false; 
    [SerializeField] bool reelingFish = false;
    [SerializeField] bool inWater = false;
    [SerializeField] bool inShore = false;
    [SerializeField] float myGravity = 9.8f; 
    [SerializeField] float waterSurface = 48f;
    [SerializeField] float inBetweenWaterLevel = 50.5f;
    // Patient Helpers ------------
    [SerializeField] float sensibilidadeX = 70f;
    [SerializeField] float sensibilidadeY = 70f;
    float xRotation;
    float yRotation;
    public Transform orientation;
    [SerializeField] private ControlVolumes controlVolumes;
    //-----------------------------

    [Header("Movement")]
    public float speed;
    [SerializeField] float maxSpeed;
    public float horizontalInput;
    public float verticalInput;

    [SerializeField]
    int horizontalMovementMulti = 1;
    [SerializeField]
    int verticalMovementMulti = 1;
    Vector3 moveDirection;

    [Header("Look Proxy")]
    public float lookX;
    public float lookY;

    Rigidbody rb;

    public bool aiming = false;
    [SerializeField] private HarpoonTrigger ht;
    [SerializeField] private int maxFogColor = 160;
    [SerializeField] private int minFogColor = 0;
    [SerializeField] private float lastDepthSaved = 95f;
    [SerializeField] private float NextDepthTreshold = 5;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private GameObject[] waterDiveParticles;
    [SerializeField] private GameObject Fog;
    [SerializeField] private GameObject Hand;

    public bool lookLocked = false;
    private PlayerVisionController playerVisionController;
    [SerializeField] private DayManager dayManager;
    
    void Start()
    {
        thisCamera = GetComponent<Camera>();
        ZonesToDeActivate[1].SetActive(false);
        underWaterSound.enabled = false;
        playerVisionController = GetComponent<PlayerVisionController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = true;
        audioSource.Play();
    }

    void Update()
    {
        PlayerLook();

        if (!reelingFish)
        {
            PlayerMovement(horizontalInput, verticalInput);
        }
    }

    private void FixedUpdate()
    {
        if (!reelingFish)
        {
            if (InWater())
            {
                PlayerMove();
            }

            else if (!ByPassWater)
            {
                Falling();
                return;
            }
        }
    }

    private void CalculateFogColorAndDensity()
    {
        float dist = transform.position.y - lastDepthSaved;
        float abs = Mathf.Abs(dist);
        float green = RenderSettings.fogColor.g;
        float blue = RenderSettings.fogColor.b;
        float red = RenderSettings.fogColor.r;

        if (dist < 0) // descer
        {
            if(abs > NextDepthTreshold)
            {
                lastDepthSaved = transform.position.y;
                NextDepthTreshold = lastDepthSaved + NextDepthTreshold;
                green -= 0.1f;
                blue -= 0.1f;
                red -= 0.1f;

                if(red < 0)
                {
                    green = 0; red = 0; blue = 0;
                }

                RenderSettings.fogColor = new Color(green/255, blue/255, red / 255);
            }
            return;
        }
        else//subir
        {
            lastDepthSaved = transform.position.y;
            NextDepthTreshold = lastDepthSaved - NextDepthTreshold;
            green+=0.1f;
            blue += 0.1f;
            red += 0.1f;

            if (red > 0.7f)
            {
                green = 0.7f; red = 0.7f; blue = 0.7f;
            }

            RenderSettings.fogColor = new Color(green / 255, blue / 255, red / 255);
        }
    }

    private bool InWater()
    {
        if (transform.position.y < waterSurface)
        {
            inShore = false;
            if (inWater)
            {
                return true;
            }

            ZonesToDeActivate[0].SetActive(true);
            underWaterSound.enabled = true;
            mainCamera.farClipPlane = 300;
            controlVolumes.SwitchVolumes(2);
            skyboxMat.SetColor("_AlphaColor",Color.white);
            skyboxMat.SetFloat("_StarAmount", 0);
            waterDiveParticles[0].SetActive(true);
            waterDiveParticles[0].GetComponent<AudioSource>().Play();
            inWater = true;
            ActivateSoundSurface(0.0f);
            playerVisionController.SetAmbientColors(false); //Checks if is night time to set colors straight. If is day, doesn't change!
            return inWater; 
        }

        if(transform.position.y < inBetweenWaterLevel)
        {
            inWater = false;
            if (inShore)
            {
                return inShore;
            }

            inShore = true;
            ZonesToDeActivate[0].SetActive(false);
            underWaterSound.enabled = false;
            mainCamera.farClipPlane = 1500;
            controlVolumes.SwitchVolumes(0);
            Fog.SetActive(false);
            skyboxMat.SetColor("_AlphaColor", Color.black);
            skyboxMat.SetFloat("_StarAmount", dayManager.starAmount);
            playerVisionController.SetAmbientColors(true);
            waterDiveParticles[1].SetActive(true);
            waterDiveParticles[1].GetComponent<AudioSource>().Play();
            ActivateSoundSurface(0.15f);
            inShore = true;
            return inShore;
        }

        return false;
    }

    private void ActivateSoundSurface(float volume)
    {
        surfaceSound.volume = volume;
    }

    private void Falling()
    {
        rb.velocity = new Vector3(0,0,0);
        transform.position = new Vector3(transform.position.x, transform.position.y - (myGravity*Time.deltaTime),transform.position.z);
    }

    private void PlayerLook()
    {
        if (unityEditor)
        {
            if (Input.GetKeyUp(KeyCode.L)) lookLocked = true;
            if (Input.GetKeyUp(KeyCode.U)) lookLocked = false;

            if (lookLocked)
            {
                return;
            }

            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensibilidadeX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensibilidadeY;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(-xRotation, yRotation, 0);
        }

        else
        {
            float mouseX = lookX;
            float mouseY = lookY;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(-xRotation, yRotation, 0);
        }
    }

    public void ProxyPlayerLook(float x, float y)
    {
        lookX = x;
        lookY = y;
    }

    public void PlayerMovement(float h, float v)
    {
        if (EasyControls)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical")*-1;

            if(Input.GetKeyDown(KeyCode.E))
            {
                Hand.SetActive(true);
                aiming = true;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ht.StopAiming();
                aiming = false;
            }

            if(aiming)
            {
                ht.Aiming();
            }

        }
        else
        {
            verticalInput = v;
            horizontalInput = h;
        }
    }
    
    private void PlayerMove()
    {
        if(ht.locked || aiming)
        {
            ControlEngineSound(-1);
            return;
        }

        moveDirection = transform.forward * verticalInput + transform.right * -horizontalInput;
        rb.AddForce(moveDirection * speed * -1, ForceMode.Force);
        if(moveDirection.magnitude > 0.1f)
        {
            ControlEngineSound(1);
            return;
        }

        ControlEngineSound(-1);
    }

    private void ControlEngineSound(float x)
    {
        volumeEngine += Time.deltaTime * volumeEngineMultiplier * x;
        if(volumeEngine >= 0.3f)
            volumeEngine = 0.3f;
        if (volumeEngine<= 0.0f)
            volumeEngine = 0.0f;

        engineSound.volume = volumeEngine;
    }

    public void ResetMovement()
    {
        speed = 10.0f;
        ByPassWater = false;
        lookLocked = false;
        rb.freezeRotation = false;
    }
}
