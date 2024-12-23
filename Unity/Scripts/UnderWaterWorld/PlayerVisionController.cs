using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisionController : MonoBehaviour
{
    [SerializeField] private float WaterColorLerpSpeed = 0.1f;
    [SerializeField] private float AmbientColorLerpSpeed = 0.1f;
    [SerializeField] private Color[] EnvironmentColors;
    [SerializeField] private Color[] Colors;
    [SerializeField] private Color[] ColorsNight;
    private Color currentColorLerp;
    private Color currentWaterColorLerp;
    private Camera mainCamera;
    [SerializeField] private GameObject[] water;
    [SerializeField] private Color[] waterColors;
    [SerializeField] private Color[] waterColorsNight;
    [SerializeField] private Material skyboxMat;
    [SerializeField] private Material[] waterMat;
    private Material currentWaterMat;
    [SerializeField] private Image NoFlashLightObject;
    private Color flashLightColor;
    [SerializeField] private PlayerProgress progress;
    public int flashlight = 0;
    public bool isDayTime = true;
    private bool warned = false;
    private bool inDeep = false;
    [SerializeField] private GameObject needFlashlightWarning;
    [SerializeField] private AudioCuesManagerScript cuesManagerScript;
    //0 - 0.35 - 0.57 - blue
    //0.05 - 0.57 - 0.40 - Green

    private void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        flashLightColor = new Color(0,0,0,0);
        NoFlashLightObject.color = flashLightColor;
        flashlight = progress.shipFlashlight;
        if (flashlight == 0)
            StartCoroutine("MyUpdate");
        else
            NoFlashLightObject.gameObject.SetActive(false);
    }

    public void FogColor(bool day)
    {
        if(!day)
            skyboxMat.SetColor("_AlphaColor", Color.black);
        else
            skyboxMat.SetColor("_AlphaColor", Color.white);
    }

    public void UpdateFlashlight()
    {
        flashlight = 1;
        StopCoroutine("MyUpdate");
        NoFlashLightObject.gameObject.SetActive(false);
    }

    private IEnumerator MyUpdate()
    {
        if(transform.position.y < -50)
        {
            flashLightColor.a = flashLightColor.a + 0.01f;
            if (flashLightColor.a < 0.01f)
                flashLightColor.a = 0.01f;
            NoFlashLightObject.color = flashLightColor;
            inDeep = true;
        }

        else 
        {
            flashLightColor.a = flashLightColor.a - 0.01f;
            if (flashLightColor.a > 1f)
                flashLightColor.a = 1;
            NoFlashLightObject.color = flashLightColor;
            needFlashlightWarning.SetActive(false);
            warned = false;
            inDeep = false;
        }

        if (!warned && inDeep)
        {
            needFlashlightWarning.SetActive(true);
            warned = true;
            cuesManagerScript.PlayAudioClip(2);
        }

        yield return new WaitForSeconds(0.02f);
        StartCoroutine("MyUpdate");
    }

    public void ChangeColorView(int colorIndex)
    {
        if (isDayTime)
        {
            currentColorLerp = Colors[colorIndex];
        }
        else
        {
            currentColorLerp = ColorsNight[colorIndex];
        }
        Color lerpedColor = skyboxMat.GetColor("_FogColor");
        StartCoroutine(LerpColor(lerpedColor));
    }

    public void SetWaterOriginalColor()
    {
        if (isDayTime)
        {
            waterMat[0].SetColor("_BaseColor", waterColors[0]);
            currentWaterMat = waterMat[0];
            skyboxMat.SetColor("_FogColor", Colors[0]);
            RenderSettings.fogColor = Colors[0];
            RenderSettings.ambientSkyColor = EnvironmentColors[0];
            RenderSettings.ambientEquatorColor = EnvironmentColors[1];
        }
        else
        {
            waterMat[1].SetColor("_BaseColor", waterColorsNight[0]);
            currentWaterMat = waterMat[1];
            skyboxMat.SetColor("_FogColor", ColorsNight[0]);
            RenderSettings.fogColor = ColorsNight[0];
            RenderSettings.ambientSkyColor = EnvironmentColors[2];
            RenderSettings.ambientEquatorColor = EnvironmentColors[3];
        }
    }

    public void SetAmbientColors(bool day)
    {
        if (day)
        {
            RenderSettings.ambientSkyColor = EnvironmentColors[0];
            RenderSettings.ambientEquatorColor = EnvironmentColors[1];
        }
        else
        {
            if (isDayTime)
                return;
            RenderSettings.ambientSkyColor = EnvironmentColors[2];
            RenderSettings.ambientEquatorColor = EnvironmentColors[3];
        }
    }

    public void ChangeColorViewWater(int colorIndex)
    {
        if (isDayTime)
        {
            currentWaterMat = waterMat[0];
            currentWaterColorLerp = waterColors[colorIndex];
        }
        else
        {
            currentWaterMat = waterMat[1];
            currentWaterColorLerp = waterColorsNight[colorIndex];
        }

        Color lerpedColor = currentWaterMat.GetColor("_BaseColor");
        StartCoroutine(LerpColorWater(lerpedColor));
    }

    IEnumerator LerpColor(Color lerpedColor)
    {
        if (lerpedColor != currentColorLerp)
        {
            lerpedColor = Color.Lerp(lerpedColor, currentColorLerp, AmbientColorLerpSpeed);
            RenderSettings.fogColor = lerpedColor;
            skyboxMat.SetColor("_FogColor", lerpedColor);
            yield return new WaitForSeconds(0.03f);

            StartCoroutine(LerpColor(lerpedColor));
        }
    }

    IEnumerator LerpColorWater(Color lerpedColor)
    {
        if (lerpedColor != currentWaterColorLerp)
        {
            lerpedColor = Color.Lerp(lerpedColor, currentWaterColorLerp, WaterColorLerpSpeed);
            currentWaterMat.SetColor("_BaseColor", lerpedColor);
            yield return new WaitForSeconds(0.03f);

            StartCoroutine(LerpColorWater(lerpedColor));
        }
    }
}
