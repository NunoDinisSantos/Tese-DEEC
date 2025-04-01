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
    public bool isDay = false;
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

    private LightmapData[] dayLightmaps;
    private LightmapData[] nightLightmaps;

    [SerializeField] private Texture2D[] dayLights;
    [SerializeField] private Texture2D[] dayDir;
    [SerializeField] private Texture2D[] nightLights;
    [SerializeField] private Texture2D[] nightDir;
    private void Start()
    {
        InitializeBakedLights();
        RandomWeather();
    }

    private void InitializeBakedLights()
    {
        dayLightmaps = LoadTexturesLights(dayLights, dayDir, "GameScene/Day/");
        nightLightmaps = LoadTexturesLights(nightLights, nightDir, "GameScene/Night/");
    }

    private LightmapData[] LoadTexturesLights(Texture2D[] colorMaps, Texture2D[] dirMaps, string path)
    {
        int count = Mathf.Min(colorMaps.Length, dirMaps.Length);
        LightmapData[] lightmaps = new LightmapData[count];

        for (int i = 0; i < count; i++)
        {         
            lightmaps[i] = new LightmapData
            {
                lightmapColor = colorMaps[i],
                lightmapDir = dirMaps[i]
            };
        }

        return lightmaps;
    }

    private void RandomWeather()
    {
        int i = Random.Range(0, 100);
        if (!isDay)
        {
            isDay = true;
            LightmapSettings.lightmaps = dayLightmaps;
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
            LightmapSettings.lightmaps = nightLightmaps;
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
        RandomWeather();
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
