using UnityEngine;

public class PlayerMovementWater : MonoBehaviour
{
    [Header("UNITY TESTING")]
    public bool unityEditor = true;

    [Header("Misc")]
    [HideInInspector][SerializeField] private Material skyboxMat;
    [HideInInspector][SerializeField] private AudioSource underWaterSound;
    [HideInInspector][SerializeField] private AudioSource engineSound;
    [HideInInspector][SerializeField] private float volumeEngine = 0;
    [HideInInspector][SerializeField] private float volumeEngineMultiplier = 1;
    [HideInInspector][SerializeField] private GameObject[] ZonesToDeActivate;
    [HideInInspector][SerializeField] private Camera mainCamera;
    private Camera thisCamera;
    [HideInInspector][SerializeField] private AudioSource surfaceSound;

    [HideInInspector] public bool ByPassWater = false;
    [HideInInspector][SerializeField] bool reelingFish = false;
    [HideInInspector][SerializeField] bool inWater = false;
    [HideInInspector][SerializeField] bool inShore = false;
    [HideInInspector][SerializeField] float myGravity = 9.8f;
    [HideInInspector][SerializeField] float waterSurface = 48f;
    [HideInInspector][SerializeField] float inBetweenWaterLevel = 50.5f;
    // Patient Helpers ------------
    [HideInInspector][SerializeField] float sensibilidadeX = 70f;
    [HideInInspector][SerializeField] float sensibilidadeY = 70f;
    float xRotation;
    float yRotation;
    [HideInInspector] public Transform orientation;
    [HideInInspector][SerializeField] private ControlVolumes controlVolumes;
    //-----------------------------

    [Header("Movement")]
    [HideInInspector] public float speed;
    [SerializeField] float maxSpeed;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;

    [HideInInspector]
    [SerializeField]
    int horizontalMovementMulti = 1;
    [HideInInspector]
    [SerializeField]
    int verticalMovementMulti = 1;
    Vector3 moveDirection;

    [Header("Look Proxy")]
    [HideInInspector] public float lookX;
    [HideInInspector] public float lookY;

    Rigidbody rb;

    [HideInInspector] public bool aiming = false;
    [HideInInspector][SerializeField] private HarpoonTrigger ht;

    [HideInInspector][SerializeField] private AudioSource audioSource;
    [HideInInspector][SerializeField] private AudioClip[] audioClips;
    [HideInInspector][SerializeField] private GameObject[] waterDiveParticles;
    [HideInInspector][SerializeField] private GameObject Fog;
    [HideInInspector][SerializeField] private GameObject Hand;

    [HideInInspector] public bool lookLocked = false;
    [HideInInspector] private PlayerVisionController playerVisionController;
    [HideInInspector][SerializeField] private DayManager dayManager;
    
    void Start()
    {
        Application.targetFrameRate = 60;

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
            //playerVisionController.SetAmbientColors(true);
            playerVisionController.SetAmbientColorAtSurface();
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
        if (unityEditor)
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
