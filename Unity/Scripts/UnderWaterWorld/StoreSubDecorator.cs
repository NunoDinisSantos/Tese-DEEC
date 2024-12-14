using UnityEngine;

public class StoreSubDecorator : MonoBehaviour
{
    public int shipFlashlight = 0;
    public int shipDepthModule = 0;
    public int shipStorageModule = 0;
    public int shipReelStrenghtModule = 0;
    public int shipTemperatureModule = 0;

    public GameObject[] DeepModule;
    public GameObject[] TempModule;
    public GameObject[] ReelModule;
    public GameObject[] StorageModule;
    public GameObject FlashLight;

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
                DeepModule[shipDepthModule - 1].SetActive(true);
            else
            {
                DeepModule[shipDepthModule - 1].SetActive(true);
                DeepModule[shipDepthModule].SetActive(true);
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
