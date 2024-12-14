using UnityEngine;

public class ScreenSleepScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void OnApplicationQuit()
    {
        // Restore the default screen timeout when the app is closed
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}
