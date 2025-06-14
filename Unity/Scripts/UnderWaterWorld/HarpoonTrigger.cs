using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class HarpoonTrigger : MonoBehaviour
{
    public bool isReeling = false;

    [HideInInspector] private int HarpoonLength = 25;
    [HideInInspector] private float HarpoonSpeed;
    [Header("Devia ser consoante a velocidade do movimento do execicio")]
    [SerializeField] private float HarpoonRetractSpeed = 0.3f;
    [HideInInspector][SerializeField] private Camera MainCamera;
    [HideInInspector][SerializeField] private Vector3 HarpoonSpawnPoint;
    [HideInInspector][SerializeField] private GameObject HarpoonHandTarget;
    [HideInInspector][SerializeField] private GameObject HarpoonHandModel;
    public bool canFire = true;
    [SerializeField] bool fired = false;
    [HideInInspector] public bool aiming = false;
    [HideInInspector] private bool wantsToStopAim = false;
    [HideInInspector][SerializeField] public bool locked = false;
    [HideInInspector][SerializeField] PlayerMovementWater playerMovementWater;
    [HideInInspector] public float multiplyer = 1;
    [HideInInspector] public float timerMax = 4;
    [HideInInspector] Vector2 aim = Vector2.zero;
    [SerializeField] private bool retractingHarpoon = false;

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
    [HideInInspector][SerializeField] private AudioSource audioSourceReel;
    //[HideInInspector]
    [SerializeField] private AudioClip[] audioClips;
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
    [SerializeField] private GameObject DomeFishCaught;

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
    [SerializeField] private Collider harpoonCollider;

    private float vertical;
    private float horizontal;

    [Description("Gives credits each X second")]
    public const float GiveCreditTimer = 1f;
    private float creditTimer;
    //#################################################################################

    [SerializeField] private GameObject harpoonModeText;
    [SerializeField] private GameObject exerciseText;

    private List<Transform> fishToActivateList = new();

    private void Start()
    {
        switch(_playerProgress.shipReelStrenghtModule)
        {
            case 0:
                HarpoonLength = 35;
                HarpoonSpeed = 50;
                HarpoonRetractSpeed = HarpoonSpeed;
                ExerciseReelingSpeed = HarpoonSpeed*0.30f;
                break;
            case 1:
                HarpoonLength = 45;
                HarpoonSpeed = 70;
                HarpoonRetractSpeed = HarpoonSpeed;
                ExerciseReelingSpeed = HarpoonSpeed * 0.21f;
                break;
            case 2:
                HarpoonLength = 55;
                HarpoonSpeed = 85;
                HarpoonRetractSpeed = HarpoonSpeed;
                ExerciseReelingSpeed = HarpoonSpeed * 0.22f;               
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
                HarpoonSpeed = 50;
                HarpoonRetractSpeed = HarpoonSpeed;
                ExerciseReelingSpeed = HarpoonSpeed * 0.30f;
                break;
            case 1:
                HarpoonLength = 45;
                HarpoonSpeed = 70;
                HarpoonRetractSpeed = HarpoonSpeed;
                ExerciseReelingSpeed = HarpoonSpeed * 0.21f;
                break;
            case 2:
                HarpoonLength = 55;
                HarpoonSpeed = 85;
                HarpoonRetractSpeed = HarpoonSpeed;
                ExerciseReelingSpeed = HarpoonSpeed * 0.22f;
                break;
        }
    }

    void Update()
    {
        if (stopAimCalled)
        {
            StopAiming();
            stopAimCalled = false;
            harpoonModeText.SetActive(false);
        }

        if (!aiming)
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

            harpoonModeText.SetActive(true);

            if (FiredFromProxy && canFire || Input.GetKeyUp(KeyCode.F))
            {
                FireSoundAndParticles();
                canFire = false;
                harpoonCollider.enabled = true;

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

                    HarpoonHandModel.transform.localPosition = Vector3.MoveTowards(HarpoonHandModel.transform.localPosition, new Vector3(0, -1.6f, -1.5f), HarpoonRetractSpeed * 0.01f);
                    if (HarpoonHandModel.transform.localPosition.z - HarpoonSpawnPoint.z - handOffset >= 0)
                    {
                        canFire = true;
                        fired = false;
                        retractingHarpoon = false;
                        locked = false;
                        reelSound = false;
                        audioSourceReel.volume = 0.0f;
                        HarpoonHandModel.transform.localPosition = new Vector3(0, -1.6f, -3.5f);
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
            PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage("Fish storage full!");
            cuesManagerScript.PlayAudioClip(2);

            ResetStatus();
            return true;
        }
        if (!grabbedAchievObject && inventory.CheckIfGoingFull(GrabbedFishTransform.gameObject.GetComponent<Fish>().fishSpaceSize))
        {
            PlayerMessageScript.SetActive(true);
            PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage("Fish storage full!");
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
            exerciseText.SetActive(true);
            checkedWhatIs = true;
            DomeFishCaught.SetActive(true);
            CatchSomething();
            aiming = true;
            HarpoonHandModel.transform.position = GrabbedFishTransform.position;
            if (HarpoonSpawnPoint.z - HarpoonHandModel.transform.localPosition.z >= HarpoonLength)
            {
                grabbedFish = false;
                exerciseText.SetActive(false);
                DomeFishCaught.SetActive(false);
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
            exerciseText.SetActive(false);
            DomeFishCaught.SetActive(false);
            fishEscapeTimer = UnityEngine.Random.Range(fishEscapeTimerMin, fishEscapeTimerMax);
            harpoonCollider.enabled = false;
            retractingHarpoon = true;
            checkedWhatIs = false;
            harpoonCollider.enabled = false;
            reelSound = false;


            grabbedAchievObject = false;
            //fired = false;
            //aiming = true;
            reelFishSound = false;
            reelSound = false;
            harpoonCollider.enabled = false;

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
            audioSourceReel.volume = 0.0f;
            return;
        }

        exerciseText.SetActive(true);
        DomeFishCaught.SetActive(true);
        HarpoonHandModel.transform.localPosition = Vector3.MoveTowards(HarpoonHandModel.transform.localPosition, new Vector3(0, -1.6f, -1.5f), ExerciseReelingSpeed * 0.01f);
        GrabbedFishTransform.position = HarpoonHandModel.transform.position;

        creditTimer -= Time.deltaTime;

        if (creditTimer < 0)
        {
            creditTimer = GiveCreditTimer;
            _playerProgress.UpdateCredits(1);
        }

        if (!reelFishSound)
        {
            reelFishSound = true;
            RetractingSoundAndParticles();
        }

        if (HarpoonHandModel.transform.localPosition.z - HarpoonSpawnPoint.z - handOffset >= 0)
        {
            fishToActivateList.Add(GrabbedFishTransform.gameObject.transform);
            GrabbedFishTransform.gameObject.SetActive(false);

            if (!grabbedAchievObject)
            {
                SendFishToInventory(GrabbedFishTransform.gameObject.GetComponent<Fish>());
                ResetStatus();
                StartCoroutine("ActivateFishAfterTime");
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
        float activateWaitingTimer = Random.Range(10, 15);

        yield return new WaitForSeconds(activateWaitingTimer);

        if(fishToActivateList.Count>0)
        {
            fishToActivateList[0].gameObject.transform.position = fishToActivateList[0].gameObject.GetComponent<FishWaypoints>().spawnedPosition;
            fishToActivateList[0].gameObject.SetActive(true);
            fishToActivateList.RemoveAt(0);
        }
    }

    private void SendFishToInventory(Fish fish)
    {
        fishEscapeTimer = UnityEngine.Random.Range(fishEscapeTimerMin, fishEscapeTimerMax);
        audioSourceReel.volume = 0.0f;
        CatchFishSoundAndParticles();
        inventory.ControlStorageSpace(fish);
        fishInventoryAnimation.Play("CatchFish");
        fish.gameObject.SetActive(false);
    }

    private void SendAchievObjectToInvetory(AchievementObject achievementObject)
    {
        audioSourceReel.volume = 0.0f;
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
            harpoonModeText.SetActive(false);
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
        audioSourceReel.volume = 0.4f;
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
        exerciseText.SetActive(false);
        DomeFishCaught.SetActive(false);
        fired = false;
        retractingHarpoon = false;
        aiming = true;
        locked = false;
        reelFishSound = false;
        reelSound = false;
        HarpoonHandModel.transform.localPosition = new Vector3(0, -1.6f, -3.5f);
        harpoonCollider.enabled = false;
        canFire = true;
    }

    public bool CheckIfIsFiring()
    {
        return canFire == true && grabbedFish == false && retractingHarpoon == false;
    }
}