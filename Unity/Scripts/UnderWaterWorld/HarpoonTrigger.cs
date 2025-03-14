using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class HarpoonTrigger : MonoBehaviour
{
    [HideInInspector] public bool isReeling = false;

    [HideInInspector] private int HarpoonLength = 25;
    [HideInInspector] private float HarpoonSpeed;
    [Header("Devia ser consoante a velocidade do movimento do execicio")]
    [SerializeField] private float HarpoonRetractSpeed = 0.3f;
    [HideInInspector][SerializeField] private Camera MainCamera;
    [HideInInspector][SerializeField] private Vector3 HarpoonSpawnPoint;
    [HideInInspector][SerializeField] private GameObject HarpoonHandTarget;
    [HideInInspector][SerializeField] private GameObject HarpoonHandModel;
    [HideInInspector] public bool canFire = true;
    [HideInInspector][SerializeField] bool fired = false;
    [HideInInspector] public bool aiming = false;
    [HideInInspector] private bool wantsToStopAim = false;
    [HideInInspector][SerializeField] public bool locked = false;
    [HideInInspector][SerializeField] PlayerMovementWater playerMovementWater;
    [HideInInspector] public float multiplyer = 1;
    [HideInInspector] public float timerMax = 4;
    [HideInInspector] Vector2 aim = Vector2.zero;
    [HideInInspector][SerializeField] private bool retractingHarpoon = false;

    private float fishEscapeTimerMin = 5;
    private float fishEscapeTimerMax = 10;
    private float ExerciseReelingSpeed = 10f;
    [HideInInspector] public bool grabbedFish = false;
    [HideInInspector][SerializeField] private int fishStrength = 1;
    [HideInInspector][SerializeField] private Transform GrabbedFishTransform;
    [HideInInspector] public float handOffset = 0.8f;
    [HideInInspector][SerializeField] private bool pullingDirectionClockwise = true;
    [HideInInspector][SerializeField] private float GrabX;
    [HideInInspector][SerializeField] private float GrabY;
    [HideInInspector][SerializeField] private float GrabAngle;
    [HideInInspector][SerializeField] private float AngleTolerance = 10f;
    [HideInInspector][SerializeField] private float RTolerance = 0.5f;
    [HideInInspector] public bool stopAimCalled = false;
    [HideInInspector][SerializeField] private float aimingSensitivityMultiplierX = 0.01f;
    [HideInInspector][SerializeField] private float aimingSensitivityMultiplierY = 0.01f;
    [HideInInspector][SerializeField] public bool FiredFromProxy = false;
    [HideInInspector][SerializeField] private bool CloseTocompleteRot = false;
    [HideInInspector][SerializeField] private Inventory inventory;
    [HideInInspector][SerializeField] private GameObject PlayerMessageScript;
    [HideInInspector][SerializeField] private AudioCuesManagerScript cuesManagerScript;
    private AudioSource audioSource;
    [HideInInspector][SerializeField] private AudioClip[] audioClips;
    private bool reelFishSound = false;
    private bool reelSound = false;
    [HideInInspector] public GameObject FireParticles;
    [HideInInspector] public FishControllerScript fishController;
    [HideInInspector] public PlayerProgress _playerProgress;
    [HideInInspector] public GameObject CanvasMira;
    private bool grabbedAchievObject = false;
    private bool checkedWhatIs = false;
    [HideInInspector][SerializeField] private Animation fishInventoryAnimation;
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
    private float fishEscapeTimer = 8f;
    [HideInInspector][SerializeField] private Collider harpoonCollider;

    private float vertical;
    private float horizontal;

    [Description("D� cr�ditos enquanto est� reeling de X em X segundos")]
    public const float GiveCreditTimer = 0.25f;
    private float creditTimer;
    //#################################################################################

    private void Start()
    {
        switch(_playerProgress.shipReelStrenghtModule)
        {
            case 0:
                HarpoonLength = 35;
                HarpoonSpeed = 20;
                HarpoonRetractSpeed = 25;
                break;
            case 1:
                HarpoonLength = 45;
                HarpoonSpeed = 27;
                HarpoonRetractSpeed = 25f; 
                break;
            case 2:
                HarpoonLength = 55;
                HarpoonSpeed = 32;
                HarpoonRetractSpeed = 25f;
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
                HarpoonLength = 35;
                HarpoonSpeed = 20;
                HarpoonRetractSpeed = 25f;
                break;
            case 1:
                HarpoonLength = 45;
                HarpoonSpeed = 27;
                HarpoonRetractSpeed = 25f;
                break;
            case 2:
                HarpoonLength = 55;
                HarpoonSpeed = 32;
                HarpoonRetractSpeed = 25f;
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
            Debug.Log("Aiming");
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
            fishEscapeTimer = UnityEngine.Random.Range(fishEscapeTimerMin, fishEscapeTimerMax);
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

    private void ReelFishtwo()
    {
        if(!isReeling)
        {
            return;
        }

        HarpoonHandModel.transform.localPosition = Vector3.MoveTowards(HarpoonHandModel.transform.localPosition, new Vector3(0, -1.6f, -2.3f), ExerciseReelingSpeed * 0.01f);
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
                ResetStatus();
                StartCoroutine(ActivateFishAfterTime());
            }
            else
            {
                SendAchievObjectToInvetory(GrabbedFishTransform.gameObject.GetComponent<AchievementObject>());
                ResetStatus();
            }

            //ResetStatus();
        }
    }

    private IEnumerator ActivateFishAfterTime()
    {
        float activateWaitingTimer = Random.Range(10, 30);
        yield return new WaitForSeconds(activateWaitingTimer);
        GrabbedFishTransform.gameObject.transform.position = GrabbedFishTransform.GetComponent<FishWaypoints>().spawnedPosition;
        GrabbedFishTransform.gameObject.SetActive(true);
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

    public bool CheckIfIsFiring()
    {
        return canFire;
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