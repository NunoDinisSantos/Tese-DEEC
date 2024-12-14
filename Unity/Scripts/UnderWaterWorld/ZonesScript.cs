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

    public GameObject SafeZoneObject;
    public GameObject GreenZoneObject;
    public GameObject DarkZoneObject;
    public GameObject TempleZoneObject;
    public GameObject HotZoneObject;
    public GameObject BaseZoneObject;
    public GameObject ColdZoneObject;

    public PlayerHealth PlayerHealth;
    [SerializeField] FishControllerScript fishControllerScript;
    [SerializeField] private PlayerVisionController playerVisionController;

    public void OnTriggerEnter(Collider other)
    {
        if(HotZone)
        {
            PlayerHealth.EnterHotZone(1);
            Debug.Log("Entered hot zone...");
            fishControllerScript.ActivateSpawner("Hot");
            playerVisionController.ChangeColorView(8);
            /*
            SafeZoneObject.SetActive(false);
            GreenZoneObject.SetActive(false);
            ColdZoneObject.SetActive(false);
            BaseZoneObject.SetActive(false);
            DarkZoneObject.SetActive(false);
            TempleZoneObject.SetActive(false);
            HotZoneObject.SetActive(true);*/
            ColdZoneObject.SetActive(true);
            TempleZoneObject.SetActive(false);

            return;
        }

        if (ColdZone)
        {
            PlayerHealth.EnterColdZone(2);
            Debug.Log("Entered Cold zone...");
            fishControllerScript.ActivateSpawner("Ice");
            playerVisionController.ChangeColorView(5);
            TempleZoneObject.SetActive(false);
            /*SafeZoneObject.SetActive(false);
            GreenZoneObject.SetActive(false);
            ColdZoneObject.SetActive(false);
            BaseZoneObject.SetActive(false);
            DarkZoneObject.SetActive(false);
            HotZoneObject.SetActive(false);*/
            return;
        }

        if (DarkZone)
        {
            Debug.Log("Entered dark zone...");
            playerVisionController.ChangeColorView(4);
            fishControllerScript.ActivateSpawner("Deep");
            //other.GetComponent<VisionController>().EnterDarkZone();
            /*TempleZoneObject.SetActive(false);
            SafeZoneObject.SetActive(false);
            GreenZoneObject.SetActive(false);
            ColdZoneObject.SetActive(false);
            BaseZoneObject.SetActive(false);
            DarkZoneObject.SetActive(true);
            HotZoneObject.SetActive(false);*/
            return;
        }

        if (GreenZone)
        {
            Debug.Log("Entered green zone...");
            playerVisionController.ChangeColorView(1);
            //playerVisionController.ChangeColorViewWater(2);
            fishControllerScript.ActivateSpawner("Green");
            /*TempleZoneObject.SetActive(false);
            SafeZoneObject.SetActive(false);
            GreenZoneObject.SetActive(true);
            ColdZoneObject.SetActive(false);
            BaseZoneObject.SetActive(false);
            DarkZoneObject.SetActive(false);
            HotZoneObject.SetActive(false);*/
            return;
        }

        if (SafeZone)
        {
            Debug.Log("Entered safe zone...");
            fishControllerScript.ActivateSpawner("Safe");
            playerVisionController.ChangeColorView(0);
            //playerVisionController.ChangeColorViewWater(3);
            //other.GetComponent<VisionController>().EnterDarkZone();
           /* TempleZoneObject.SetActive(false);
            SafeZoneObject.SetActive(true);
            GreenZoneObject.SetActive(false);
            ColdZoneObject.SetActive(false);
            BaseZoneObject.SetActive(false);
            DarkZoneObject.SetActive(false);
            HotZoneObject.SetActive(false);*/
            return;
        }

        if (Temple)
        {
            Debug.Log("Entered temple zone...");
            fishControllerScript.ActivateSpawner("Temple");
            //other.GetComponent<VisionController>().EnterDarkZone();
            playerVisionController.ChangeColorView(6);
            TempleZoneObject.SetActive(true);
            ColdZoneObject.SetActive(false);

            return;
        }

        if (Base)
        {
            /*TempleZoneObject.SetActive(false);
            SafeZoneObject.SetActive(true);
            GreenZoneObject.SetActive(true);
            ColdZoneObject.SetActive(false);
            BaseZoneObject.SetActive(true);
            DarkZoneObject.SetActive(false);
            HotZoneObject.SetActive(false);*/
            Debug.Log("Entered base zone...");
            fishControllerScript.ActivateSpawner("Base");
            playerVisionController.ChangeColorView(7);
            //other.GetComponent<VisionController>().EnterDarkZone();
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
            //other.GetComponent<VisionController>().LeaveDarkZone();
            return;
        }
    }
}
