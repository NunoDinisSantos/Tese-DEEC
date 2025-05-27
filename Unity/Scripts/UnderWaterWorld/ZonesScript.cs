using UnityEngine;

public class ZonesScript : MonoBehaviour
{
    public bool HotZone = false;
    public bool ColdZone = false;
    public bool DarkZone = false;
    public bool GreenZone = false;
    public bool SafeZone = false;
    public bool Temple = false;
    public bool Base = false;

    [HideInInspector] public GameObject SafeZoneObject;
    [HideInInspector] public GameObject GreenZoneObject;
    [HideInInspector] public GameObject DarkZoneObject;
    [HideInInspector] public GameObject TempleZoneObject;
    [HideInInspector] public GameObject HotZoneObject;
    [HideInInspector] public GameObject BaseZoneObject;
    [HideInInspector] public GameObject ColdZoneObject;

    [HideInInspector] public PlayerHealth PlayerHealth;
    [HideInInspector][SerializeField] FishControllerScript fishControllerScript;
    [HideInInspector][SerializeField] private PlayerVisionController playerVisionController;

    public void OnTriggerEnter(Collider other)
    {
        if(HotZone)
        {
            PlayerHealth.EnterHotZone(2);
            Debug.Log("Entered hot zone...");
            fishControllerScript.ActivateSpawner("Hot");
            playerVisionController.ChangeColorView(8);
            playerVisionController.ChangeColorViewWater(4);
            ColdZoneObject.SetActive(true);
            TempleZoneObject.SetActive(false);

            return;
        }

        if (ColdZone)
        {
            PlayerHealth.EnterColdZone(1);
            Debug.Log("Entered Cold zone...");
            fishControllerScript.ActivateSpawner("Ice");
            playerVisionController.ChangeColorView(5);
            playerVisionController.ChangeColorViewWater(3);
            TempleZoneObject.SetActive(false);
            return;
        }

        if (DarkZone)
        {
            Debug.Log("Entered dark zone...");
            playerVisionController.ChangeColorView(4);
            fishControllerScript.ActivateSpawner("Deep");
            playerVisionController.ChangeColorViewWater(2);
            return;
        }

        if (GreenZone)
        {
            Debug.Log("Entered green zone...");
            playerVisionController.ChangeColorView(1);
            playerVisionController.ChangeColorViewWater(1);
            fishControllerScript.ActivateSpawner("Green");
            return;
        }

        if (SafeZone)
        {
            Debug.Log("Entered safe zone...");
            fishControllerScript.ActivateSpawner("Safe");
            playerVisionController.ChangeColorView(0);
            playerVisionController.ChangeColorViewWater(0);
            ColdZoneObject.SetActive(true);

            return;
        }

        if (Temple)
        {
            Debug.Log("Entered temple zone...");
            fishControllerScript.ActivateSpawner("Temple");
            playerVisionController.ChangeColorView(6);
            TempleZoneObject.SetActive(true);
            ColdZoneObject.SetActive(false);

            return;
        }

        if (Base)
        {
            Debug.Log("Entered base zone...");
            fishControllerScript.ActivateSpawner("Base");
            playerVisionController.ChangeColorView(7);
            playerVisionController.ChangeColorViewWater(0);
            return;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (HotZone)
        {
            PlayerHealth.LeaveHotZone();
            return;
        }

        if (ColdZone)
        {
            PlayerHealth.LeaveColdZone();
            return;
        }

        if (DarkZone)
        {
            Debug.Log("Left dark zone...");
            return;
        }
    }
}
