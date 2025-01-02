using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    [Description("Control star amount Day and Night")]
    public int starAmountDay = 2;
    public int starAmountNight = 15;

    [HideInInspector] public PlayerProgress progress;
    [HideInInspector] public Animation canvasAnimation;
    [HideInInspector] public GameObject[] typeOfDay;
    [HideInInspector] [SerializeField] private Material skyboxMat;
    [HideInInspector] public Color[] dayColor;
    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public PlayerMovementWater playerMovementWater;
    [HideInInspector] public StoreColliderScript storeCol;
    [HideInInspector] public bool isDay = true;
    [HideInInspector] public PlayerVisionController playerVisionController;
    [HideInInspector] public GameObject StorageGuys;
    [HideInInspector] public UnderWaterStoreFishScript underWaterStoreFish;
    [HideInInspector] [SerializeField] private GameObject Fog;
    [HideInInspector] [SerializeField] private GameObject[] waterDayNight;
    [HideInInspector] public int starAmount;
    [HideInInspector] public GameObject shopCamera;
    [HideInInspector] public GameObject shopCanvas;
    [HideInInspector] [SerializeField] private Proxy proxy;
    [HideInInspector] [SerializeField] private GameObject miscStoreObjects;
    [HideInInspector] [SerializeField] private AudioClip[] surfaceSounds;
    [HideInInspector] [SerializeField] private AudioSource surfaceAudioSource;

    private void Start()
    {
        RandomWeather();
    }

    private void RandomWeather()
    {
        int i = Random.Range(0, 100);
        if (i <= 55)
        {
            isDay = true;
            playerVisionController.SetAmbientColorsTerminateDay(isDay);
            waterDayNight[0].SetActive(true);
            waterDayNight[1].SetActive(false);
            SetSceneColor(0);
            i = Random.Range(0, 100);
            if (i <= 70)
            {
                typeOfDay[0].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[0];
                surfaceAudioSource.Play();
                return;
            }

            if (i > 70 && i < 90)
            {
                typeOfDay[1].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[0];
                surfaceAudioSource.Play();
                return;
            }
            else
            {
                typeOfDay[2].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[1];
                surfaceAudioSource.Play();
                return;
            }
        }

        else
        {
            isDay = false;
            playerVisionController.SetAmbientColorsTerminateDay(isDay);
            waterDayNight[0].SetActive(false);
            waterDayNight[1].SetActive(true);
            SetSceneColor(1);
            i = Random.Range(0, 100);
            if (i <= 70)
            {
                typeOfDay[3].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[0];
                surfaceAudioSource.Play();
                return;
            }

            if (i > 70 && i < 90)
            {
                typeOfDay[4].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[1];
                surfaceAudioSource.Play();
                return;
            }
            else
            {
                typeOfDay[5].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[0];
                surfaceAudioSource.Play();
                return;
            }
        }
    }

    public void TerminateDay()
    {
        miscStoreObjects.SetActive(false);
        proxy.inShop = false;
        Fog.SetActive(false);
        progress.IncrementDays();
        canvasAnimation.Play("NextDay");
        StorageGuys.SetActive(true);
        underWaterStoreFish.times = 3;
        StartCoroutine(TerminateDayWait());
    }

    private IEnumerator TerminateDayWait()
    {


        yield return new WaitForSeconds(1f);
        shopCanvas.SetActive(false);
        shopCamera.SetActive(false);
        playerMovementWater.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        playerMovementWater.gameObject.transform.GetChild(0).GetComponent<Animation>().Play("CameraAnimationStoreReset");
        playerHealth.JustRespawn();
        storeCol.inStore = false;
        playerMovementWater.speed = 15.0f; 
        playerMovementWater.ByPassWater = false;  
        playerMovementWater.lookLocked = false;
        playerMovementWater.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
        playerMovementWater.enabled = true;

        int i = Random.Range(0, 100);
        if (i <= 70)
        {
            isDay = true;
            waterDayNight[0].SetActive(true);
            waterDayNight[1].SetActive(false);
            SetSceneColor(0);
            i = Random.Range(0, 100);
            if (i <= 70)
            {
                typeOfDay[0].SetActive(true);
                yield return null;
            }

            if (i > 70 && i < 90)
            {
                typeOfDay[1].SetActive(true);
                yield return null;
            }
            else
            {
                typeOfDay[2].SetActive(true);
                yield return null;
            }
        }

        else
        {
            isDay = false;
            waterDayNight[0].SetActive(false);
            waterDayNight[1].SetActive(true);
            SetSceneColor(1);
            i = Random.Range(0, 100);
            if (i <= 70)
            {
                typeOfDay[3].SetActive(true);
                yield return null;
            }

            if (i > 70 && i < 90)
            {
                typeOfDay[4].SetActive(true);
                yield return null;
            }
            else
            {
                typeOfDay[5].SetActive(true);
                yield return null;
            }
        }
    }

    private void SetSceneColor(int index)
    {
        if(index == 0)
        {
            playerVisionController.isDayTime = true;
            playerVisionController.FogColor(isDay);
            skyboxMat.SetColor("_BotColor", dayColor[0]);
            starAmount = starAmountDay;
            skyboxMat.SetFloat("_StarAmount", starAmount);
            skyboxMat.SetColor("_AlphaColor", Color.black);
            return;
        }

        else
        {
            playerVisionController.FogColor(isDay);
            playerVisionController.isDayTime = false;
            skyboxMat.SetColor("_BotColor", dayColor[1]);            
            starAmount = starAmountNight;
            skyboxMat.SetFloat("_StarAmount", starAmount);
            skyboxMat.SetColor("_AlphaColor", Color.black);
        }
    }
}
