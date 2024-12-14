using UnityEngine;
using UnityEngine.Rendering;

public class ControlVolumes : MonoBehaviour
{
    [SerializeField] private Volume Volume;
    [SerializeField] private VolumeProfile[] VolumesProfiles;
    [SerializeField] private PlayerMovementWater playerMovementWater;
    [SerializeField] GameObject[] WaterModels;
    public void SwitchVolumes(int volumeProfile)
    {
        switch (volumeProfile)
        {
            case 0:
                Volume.profile = VolumesProfiles[volumeProfile];
                RenderSettings.fog = false;
                WaterModels[0].SetActive(true);
                WaterModels[1].SetActive(false);
                break;
            case 1:
                Volume.profile = VolumesProfiles[volumeProfile];
                RenderSettings.fog = true;
                WaterModels[1].SetActive(true);
                WaterModels[0].SetActive(false);
                break;
            case 2:
                Volume.profile = VolumesProfiles[volumeProfile];
                RenderSettings.fog = true;
                WaterModels[1].SetActive(true);
                WaterModels[0].SetActive(false);
                break;
        }
    }
}
