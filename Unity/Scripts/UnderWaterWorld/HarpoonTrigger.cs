using System.ComponentModel;
using UnityEngine;

public class HarpoonTrigger : MonoBehaviour
{
    public bool isReeling = false;

    [HideInInspector] private int HarpoonLength = 15;
    [HideInInspector] private float HarpoonSpeed;
    [Header("Devia ser consoante a velocidade do movimento do execicio")]
    [SerializeField] private float HarpoonRetractSpeed = 0.3f;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Vector3 HarpoonSpawnPoint;
    [SerializeField] private GameObject HarpoonHandTarget;
    [SerializeField] private GameObject HarpoonHandModel;
    public bool canFire = true;
    [SerializeField] bool fired = false;
    public bool aiming = false;
    private bool wantsToStopAim = false;
    [SerializeField] public bool locked = false;
    [SerializeField] PlayerMovementWater playerMovementWater;
    public float multiplyer = 1;
    public float timerMax = 4;
    Vector2 aim = Vector2.zero;
    [SerializeField] private bool retractingHarpoon = false;

    private bool grabbedFish = false;
    [SerializeField] private int fishStrength = 1;
    [SerializeField] private Transform GrabbedFishTransform;
    public float handOffset = 0.8f;
    [SerializeField] private bool pullingDirectionClockwise = true;
    [SerializeField] private float GrabX;
    [SerializeField] private float GrabY;
    [SerializeField] private float GrabAngle;
    [SerializeField] private float AngleTolerance = 10f;
    [SerializeField] private float RTolerance = 0.5f;
    public bool stopAimCalled = false;
    [SerializeField] private float aimingSensitivityMultiplierX = 0.01f;
    [SerializeField] private float aimingSensitivityMultiplierY = 0.01f;
    [SerializeField] public bool FiredFromProxy = false;
    [SerializeField] private bool CloseTocompleteRot = false;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject PlayerMessageScript;
    [SerializeField] private AudioCuesManagerScript cuesManagerScript;
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
    private bool reelFishSound = false;
    private bool reelSound = false;
    public GameObject FireParticles;
    public FishControllerScript fishController;
    public PlayerProgress _playerProgress;
    public GameObject CanvasMira;
    private bool grabbedAchievObject = false;
    private bool checkedWhatIs = false;
    [SerializeField] private Animation fishInventoryAnimation;
    private GetPatientCircleData patientCircleData;

    //#################################################################################
    #region OBSOLETE_VR
    /*
    public int numSegments = 16; // Number of circle segments 
    private float segmentAngle; // Angle covered by each segment

    private Vector2 circleCenter; // Center point of the circle (start point)
    private float expectedRadius = 1.0f; // Expected radius of the circle movement

    private float totalReelProgress = 0f; // Progress for reeling in the fish (0 to 1)
    private bool[] segmentsCompleted; // Track if a segment has been completed
    private Vector2 startPoint;
    private bool doingCircle = false;
    */
    #endregion
    private float fishEscapeTimer = 3f;
    [SerializeField] private Collider harpoonCollider;

    private float vertical;
    private float horizontal;

    [Description("Dá créditos enquanto está reeling de X em X segundos")]
    public const float GiveCreditTimer = 0.25f;
    private float creditTimer;
    //#################################################################################

    private void Start()
    {
        switch(_playerProgress.shipReelStrenghtModule)
        {
            case 0:
                HarpoonLength = 25;
                HarpoonSpeed = 18;
                HarpoonRetractSpeed = HarpoonSpeed * 1.5f;
                break;
            case 1:
                HarpoonLength = 45;
                HarpoonSpeed = 35;
                HarpoonRetractSpeed = HarpoonSpeed * 1.5f; 
                break;
            case 2:
                HarpoonLength = 70;
                HarpoonSpeed = 65;
                HarpoonRetractSpeed = HarpoonSpeed * 1.5f;
                break;
        }

        HarpoonHandTarget.transform.localPosition = new Vector3 (0, 0.45f, -HarpoonLength);
        audioSource = GetComponent<AudioSource>();
    }
    
    public void UpdateModule()
    {
        switch (_playerProgress.shipReelStrenghtModule)
        {
            case 0:
                HarpoonLength = 25;
                HarpoonSpeed = 18;
                HarpoonRetractSpeed = HarpoonSpeed * 1.5f;
                break;
            case 1:
                HarpoonLength = 45;
                HarpoonSpeed = 35;
                HarpoonRetractSpeed = HarpoonSpeed * 1.5f;
                break;
            case 2:
                HarpoonLength = 70;
                HarpoonSpeed = 65;
                HarpoonRetractSpeed = HarpoonSpeed * 1.5f;
                break;
        }
    }

    void Update()
    {
        if (stopAimCalled)
        {
            StopAiming();
            stopAimCalled = false;
        }

        if(!aiming)
        {
            return;
        }

        if(grabbedFish)
        {
            GrabbedFish();
            return;
        }

        else {

            Aiming(); 

            if (FiredFromProxy && canFire || Input.GetKeyUp(KeyCode.F))
            {
                FireSoundAndParticles();
                canFire = false;
                HarpoonHandModel.SetActive(true);
                HarpoonHandTarget.SetActive(false);
                fired = true;
                locked = true;
            }

            if (fired) 
            {
                FiredFromProxy = false;
                HarpoonHandTarget.transform.localPosition = new Vector3(0, 0.45f, -HarpoonLength);
                Vector3 aimedPosition = HarpoonHandTarget.transform.localPosition;
                if (retractingHarpoon)
                {
                    if (!reelSound)
                    {
                        RetractingSoundAndParticles();
                        reelSound = true;
                    }

                    HarpoonHandModel.transform.localPosition = Vector3.MoveTowards(HarpoonHandModel.transform.localPosition, new Vector3(0, -1.6f, -2.3f), HarpoonRetractSpeed * 0.01f);
                    if (HarpoonHandModel.transform.localPosition.z - HarpoonSpawnPoint.z - handOffset >= 0)
                    {
                        canFire = true;
                        fired = false;
                        retractingHarpoon = false;
                        locked = false;
                        reelSound = false;
                        harpoonCollider.enabled = true;
                    }
                }

                else
                {
                    HarpoonHandModel.transform.localPosition = Vector3.MoveTowards(HarpoonHandModel.transform.localPosition, aimedPosition, HarpoonSpeed * 0.01f);
                    if (HarpoonHandModel.transform.localPosition.z - aimedPosition.z <= 0)
                    {
                        retractingHarpoon = true;
                    }
                }
            }
        }
    }

    private bool CheckInventorySpace()
    {
        checkedWhatIs = true;
        if (GrabbedFishTransform.gameObject.GetComponent<AchievementObject>() == null)
        {
            grabbedAchievObject = false;
        }
        else
        {
            grabbedAchievObject = true;
        }

        if (grabbedAchievObject && inventory.CheckIfGoingFull(GrabbedFishTransform.gameObject.GetComponent<AchievementObject>().Size))
        {
            PlayerMessageScript.SetActive(true);
            PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage("Inventario de peixes cheio!");
            cuesManagerScript.PlayAudioClip(2);

            ResetStatus();
            return true;
        }
        if (!grabbedAchievObject && inventory.CheckIfGoingFull(GrabbedFishTransform.gameObject.GetComponent<Fish>().fishSpaceSize))
        {
            PlayerMessageScript.SetActive(true);
            PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage("Inventario de peixes cheio!");
            cuesManagerScript.PlayAudioClip(2);

            ResetStatus();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GrabbedFish()
    {
        if (!checkedWhatIs)
        {
            checkedWhatIs = true;
            CatchSomething();
            aiming = true;
            HarpoonHandModel.transform.position = GrabbedFishTransform.position;
            if (HarpoonSpawnPoint.z - HarpoonHandModel.transform.localPosition.z >= HarpoonLength)
            {
                grabbedFish = false;
                retractingHarpoon = true;
            }

            if (CheckInventorySpace())
            {
                return;
            }
        }

        fishEscapeTimer -= Time.deltaTime;
        HarpoonHandModel.transform.position = GrabbedFishTransform.position;

        if (fishEscapeTimer < 0 || (HarpoonHandModel.transform.localPosition - HarpoonSpawnPoint).magnitude >= HarpoonLength)
        {
            grabbedFish = false;
            fishEscapeTimer = UnityEngine.Random.Range(3f, 5f);
            harpoonCollider.enabled = false;
            retractingHarpoon = true;
            return;
        }

        ReelFishtwo();
    }

    //#####################################################################################################################
    //########################################                             ################################################
    //########################################       FISH TO INVENTORY     ################################################
    //########################################                             ################################################
    //#####################################################################################################################

    private void ReelFish(int segment)
    {
        //totalReelProgress += 1f / numSegments; // -- Obsolete -> Was for VR patientes

        HarpoonHandModel.transform.localPosition = Vector3.MoveTowards(HarpoonHandModel.transform.localPosition, new Vector3(0, -1.6f, -2.3f), HarpoonRetractSpeed * 0.01f);
        GrabbedFishTransform.position = HarpoonHandModel.transform.position;

        //#####################################################################################################################
        //########################################                             ################################################
        //########################################       DAR CRÉDITOS AQUI     ################################################
        //########################################                             ################################################
        //#####################################################################################################################
        creditTimer -= Time.deltaTime;

        if (creditTimer < 0)
        {
            creditTimer = GiveCreditTimer;
            _playerProgress.UpdateCredits(1);
        }

        //#####################################################################################################################
        //#####################################################################################################################
        //#####################################################################################################################
        //#####################################################################################################################
        //#####################################################################################################################

        if (reelFishSound)
        {
            reelFishSound = true;
            ReelingSoundAndParticles();
        }

        if (HarpoonHandModel.transform.localPosition.z - HarpoonSpawnPoint.z - handOffset >= 0)
        {            
            GrabbedFishTransform.gameObject.SetActive(false);
            
            if (!grabbedAchievObject)
            {
                SendFishToInventory(GrabbedFishTransform.gameObject.GetComponent<Fish>());
                GrabbedFishTransform.gameObject.transform.position = GrabbedFishTransform.GetComponent<FishWaypoints>().spawnedPosition;
                GrabbedFishTransform.gameObject.SetActive(true); 
            }
            else
            {
                SendAchievObjectToInvetory(GrabbedFishTransform.gameObject.GetComponent<AchievementObject>());
            }

            ResetStatus();
        }
    }

    private void ReelFishtwo()
    {
        if(!isReeling)
        {
            return;
        }

        HarpoonHandModel.transform.localPosition = Vector3.MoveTowards(HarpoonHandModel.transform.localPosition, new Vector3(0, -1.6f, -2.3f), HarpoonRetractSpeed * 0.01f);
        GrabbedFishTransform.position = HarpoonHandModel.transform.position;

        creditTimer -= Time.deltaTime;

        if (creditTimer < 0)
        {
            creditTimer = GiveCreditTimer;
            _playerProgress.UpdateCredits(1);
        }

        if (reelFishSound)
        {
            reelFishSound = true;
            ReelingSoundAndParticles();
        }

        if (HarpoonHandModel.transform.localPosition.z - HarpoonSpawnPoint.z - handOffset >= 0)
        {
            GrabbedFishTransform.gameObject.SetActive(false);

            if (!grabbedAchievObject)
            {
                SendFishToInventory(GrabbedFishTransform.gameObject.GetComponent<Fish>());
                GrabbedFishTransform.gameObject.transform.position = GrabbedFishTransform.GetComponent<FishWaypoints>().spawnedPosition;
                GrabbedFishTransform.gameObject.SetActive(true);
            }
            else
            {
                SendAchievObjectToInvetory(GrabbedFishTransform.gameObject.GetComponent<AchievementObject>());
            }

            ResetStatus();
        }
    }

    private void SendFishToInventory(Fish fish)
    {
        CatchFishSoundAndParticles();
        inventory.ControlStorageSpace(fish);
        fishInventoryAnimation.Play("CatchFish");
    }

    private void SendAchievObjectToInvetory(AchievementObject achievementObject)
    {
        CatchFishSoundAndParticles();
        inventory.ControlStorageSpaceAchiev(achievementObject);
    }

    public void Aiming()
    {
        if (fired)
        {
            return;
        }

        if (!wantsToStopAim)
        {
            wantsToStopAim = false;
            HarpoonHandTarget.transform.localPosition = new Vector3(HarpoonHandTarget.transform.localPosition.x + (aim.x * multiplyer),
                HarpoonHandTarget.transform.localPosition.y + (aim.y * multiplyer), HarpoonHandTarget.transform.localPosition.z);
            aiming = true;
            HarpoonHandTarget.SetActive(true);
            CanvasMira.SetActive(true);
            HarpoonHandModel.SetActive(true);
        }
    }

    public void CallAim(float x, float y)
    {
        aiming = true;
        vertical = y;
        horizontal = x;
    }

    public void StopAiming()
    {
        wantsToStopAim = true;
        if (!locked && wantsToStopAim)
        {
            aiming = false;
            aim = Vector2.zero;
            HarpoonHandTarget.SetActive(false);
            HarpoonHandModel.SetActive(false);
            CanvasMira.SetActive(false);
            wantsToStopAim = false;
            retractingHarpoon = false;
        }
    }

    public void StartReeling(Transform grabbedTransform)
    {
        if (grabbedFish)
        {
            return;
        }

        grabbedFish = true;
        GrabbedFishTransform = grabbedTransform.transform;
    }

    private void FireSoundAndParticles()
    {
        FireParticles.SetActive(true);
        audioSource.PlayOneShot(audioClips[0]);
    }

    private void RetractingSoundAndParticles()
    {
        audioSource.PlayOneShot(audioClips[1]);
    }

    private void ReelingSoundAndParticles()
    {
        audioSource.PlayOneShot(audioClips[2]);
    }

    private void CatchFishSoundAndParticles()
    {
        audioSource.PlayOneShot(audioClips[3]);
    }

    private void CatchSomething()
    {
        audioSource.PlayOneShot(audioClips[2]);
    }

    private void ResetStatus()
    {
        checkedWhatIs = false;
        grabbedAchievObject = false;
        grabbedFish = false;
        fired = false;
        retractingHarpoon = false;
        aiming = true;
        locked = false;
        reelFishSound = false;
        reelSound = false;
        HarpoonHandModel.transform.localPosition = new Vector3(0, -1.6f, -2.3f);

        canFire = true;
    }

    #region OBSOLETE_VR
    /*
    public void CheckCircularMovement(float x, float y)
    {
        GrabbedFishTransform.position = HarpoonHandModel.transform.position;
        TrackCircleSegment(new Vector2(vertical, horizontal));

        return;      
    } 
     
    private bool AllSegmentsCompleted()
    {
        foreach (bool segmentCompleted in segmentsCompleted)
        {
            if (!segmentCompleted) return false;
        }
        return true;
    }

    // Reset when the player completes a full circle
    private void CompleteCircle()
    {
        Debug.Log("Full circle completed, resetting segments for next circle!");

        // Reset segment tracking for the next circle
        segmentsCompleted = new bool[numSegments];

        // Optionally reset progress, or accumulate it across multiple circles
        totalReelProgress = 0f;
    }

    // Track the player's progress through circle segments
    private void TrackCircleSegment(Vector2 playerPosition)
    {
        if(!doingCircle)
        {
            startPoint = playerPosition;
            doingCircle = true;
        }

        // Calculate the angle between the player's current position and the circle center
        float angle = CalculateAngleFromCenter(playerPosition);

        // Determine which segment the player is currently in
        int currentSegment = Mathf.FloorToInt(angle / segmentAngle);

        // If the segment hasn't been completed, mark it and reel the fish in
        if (!segmentsCompleted[currentSegment])
        {
            segmentsCompleted[currentSegment] = true;
            ReelFish(currentSegment);
        }

        // If all segments are completed, reset for a new circle
        if (AllSegmentsCompleted())
        {
            CompleteCircle();
            doingCircle = false;
        }
    }

    // Calculate the angle between the player position and the circle center
    private float CalculateAngleFromCenter(Vector2 position)
    {
        Vector2 direction = position - circleCenter;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Convert radians to degrees
        if (angle < 0) angle += 360f; // Ensure angle is in range [0, 360]
        return angle;
    }

    float CalculateCircularityError(List<Vector2> positions, float expectedRadius)
    {
        float circularityError = 0f;
        Vector2 center = startPoint;  

        foreach (Vector2 pos in positions)
        {
            float currentRadius = Vector2.Distance(center, pos);
            float error = Mathf.Abs(currentRadius - expectedRadius);
            circularityError += error;
        }

        // Average circularity error across the entire movement
        return circularityError / positions.Count;
    }*/
    #endregion
}