using UnityEngine;

public class StoreSubDecorator : MonoBehaviour
{
    [HideInInspector] public int shipFlashlight = 0;
    [HideInInspector] public int shipDepthModule = 0;
    [HideInInspector] public int shipStorageModule = 0;
    [HideInInspector] public int shipReelStrenghtModule = 0;
    [HideInInspector] public int shipTemperatureModule = 0;

    [HideInInspector] public GameObject[] DeepModule;
    [HideInInspector] public GameObject[] TempModule;
    [HideInInspector] public GameObject[] ReelModule;
    [HideInInspector] public GameObject[] StorageModule;
    [HideInInspector] public GameObject FlashLight;

    void Start()
    {
        shipDepthModule = PlayerDataScript.playerDataInstance.DepthModule;
        shipTemperatureModule = PlayerDataScript.playerDataInstance.TempModule;
        shipFlashlight = PlayerDataScript.playerDataInstance.Flashlight;
        shipStorageModule = PlayerDataScript.playerDataInstance.StorageModule;
        shipReelStrenghtModule = PlayerDataScript.playerDataInstance.ReelModule;

        DecorateSub();
    }

    private void DecorateSub()
    {
        DeepModule[0].SetActive(false);
        DeepModule[1].SetActive(false);
        FlashLight.SetActive(true);
        TempModule[0].SetActive(false);
        TempModule[1].SetActive(false);
        ReelModule[1].SetActive(false);
        ReelModule[0].SetActive(false);
        StorageModule[1].SetActive(false);
        StorageModule[0].SetActive(false);

        if (shipFlashlight > 0)
        {
            FlashLight.SetActive(false);
        }

        if (shipDepthModule > 0)
        {
            if (shipDepthModule == 1)
                DeepModule[0].SetActive(true);
            else
            {
                DeepModule[1].SetActive(true);
            }
        }

        if (shipTemperatureModule > 0)
        {
            TempModule[shipTemperatureModule - 1].SetActive(true);
        }

        if (shipReelStrenghtModule > 0)
        {
            ReelModule[shipReelStrenghtModule - 1].SetActive(true);
        }

        if (shipStorageModule > 0)
        {
            StorageModule[shipStorageModule - 1].SetActive(true);
        }
    }
}
