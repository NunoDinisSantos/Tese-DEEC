using System.Collections;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public PlayerProgress progress;
    public Animation canvasAnimation;
    public GameObject[] typeOfDay;
    [SerializeField] private Material skyboxMat;
    public Color[] dayColor;
    public PlayerHealth playerHealth;
    public PlayerMovementWater playerMovementWater;
    public StoreColliderScript storeCol;
    public bool isDay = true;
    public PlayerVisionController playerVisionController;
    public GameObject StorageGuys;
    public UnderWaterStoreFishScript underWaterStoreFish;
    [SerializeField] private GameObject Fog;
    [SerializeField] private GameObject[] waterDayNight;
    public int starAmount = 6;
    public GameObject shopCamera;
    public GameObject shopCanvas;
    [SerializeField] private Proxy proxy;
    [SerializeField] private GameObject miscStoreObjects;
    [SerializeField] private AudioClip[] surfaceSounds;
    [SerializeField] private AudioSource surfaceAudioSource;

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
            playerVisionController.SetAmbientColors(isDay);
            waterDayNight[0].SetActive(true);
            waterDayNight[1].SetActive(false);
            SetSceneColor(0);
            i = Random.Range(0, 100);
            if (i <= 70)
            {
                typeOfDay[0].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[0];
                return;
            }

            if (i > 70 && i < 90)
            {
                typeOfDay[1].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[0];
                return;
            }
            else
            {
                typeOfDay[2].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[1];
                return;
            }
        }

        else
        {
            isDay = false;
            playerVisionController.SetAmbientColors(isDay);
            waterDayNight[0].SetActive(false);
            waterDayNight[1].SetActive(true);
            SetSceneColor(1);
            i = Random.Range(0, 100);
            if (i <= 70)
            {
                typeOfDay[3].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[0];
                return;
            }

            if (i > 70 && i < 90)
            {
                typeOfDay[4].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[1];
                return;
            }
            else
            {
                typeOfDay[5].SetActive(true);
                surfaceAudioSource.clip = surfaceSounds[0];
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
            starAmount = 2;
            skyboxMat.SetFloat("_StarAmount", starAmount);
            skyboxMat.SetColor("_AlphaColor", Color.black);
            return;
        }

        else
        {
            playerVisionController.FogColor(isDay);
            playerVisionController.isDayTime = false;
            skyboxMat.SetColor("_BotColor", dayColor[1]);            
            starAmount = 7;
            skyboxMat.SetFloat("_StarAmount", starAmount);
            skyboxMat.SetColor("_AlphaColor", Color.black);
        }
    }
}
