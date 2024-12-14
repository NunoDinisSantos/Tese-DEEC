using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisionController : MonoBehaviour
{
    [SerializeField] private Color[] Colors;
    [SerializeField] private Color[] ColorsNight;
    private Color currentColorLerp;
    private Camera mainCamera;
    [SerializeField] private GameObject water;
    [SerializeField] private Material skyboxMat;
    [SerializeField] private Material waterMat;
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
            StartCoroutine(MyUpdate());
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
        StopCoroutine(MyUpdate());
        NoFlashLightObject.gameObject.SetActive(false);
    }

    private IEnumerator MyUpdate()
    {
        if(transform.position.y < -25)
        {
            flashLightColor.a = Mathf.Lerp(flashLightColor.a, 0.999f, 0.03f);
            NoFlashLightObject.color = flashLightColor;
            inDeep = true;
        }

        else
        {
            flashLightColor.a = Mathf.Lerp(flashLightColor.a, 0, 0.03f);
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

        yield return new WaitForSeconds(0.05f);
        StartCoroutine(MyUpdate());
    }

    public void ChangeColorView(int colorIndex)
    {
        if(isDayTime) 
            currentColorLerp = Colors[colorIndex];
        else
            currentColorLerp = ColorsNight[colorIndex];
        Color lerpedColor = skyboxMat.GetColor("_FogColor");
        StartCoroutine(LerpColor(lerpedColor));
    }

    public void ChangeColorViewWater(int colorIndex)
    {
        currentColorLerp = Colors[colorIndex];
        Color lerpedWaterColor = waterMat.GetColor("_BaseColor");
        StartCoroutine(LerpColorWater(lerpedWaterColor));
    }

    IEnumerator LerpColorWater(Color lerpedColor)
    {
        if (lerpedColor == currentColorLerp)
        {
            yield return null;
        }

        lerpedColor = Color.Lerp(lerpedColor, currentColorLerp, 0.1f);
        waterMat.SetColor("_BaseColor", lerpedColor);
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(LerpColor(lerpedColor));
    }

    IEnumerator LerpColor(Color lerpedColor)
    {
        if (lerpedColor == currentColorLerp)
        {
            StopCoroutine(LerpColor(lerpedColor));
            yield return null;
        }

        lerpedColor = Color.Lerp(lerpedColor, currentColorLerp, 0.07f);
        RenderSettings.fogColor = lerpedColor;
        skyboxMat.SetColor("_FogColor",lerpedColor);
        yield return new WaitForSeconds(0.25f);

        StartCoroutine(LerpColor(lerpedColor));
    }
}
