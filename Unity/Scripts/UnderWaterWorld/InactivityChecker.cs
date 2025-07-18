using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InactivityChecker : MonoBehaviour
{
    [SerializeField]
    private int InactivityTime = 60;
    [SerializeField]
    private int Timer;
    
    [SerializeField]
    private ShopManager shopManager;
    [SerializeField]
    private PlayerMovementWater playerMovementWater;
    [SerializeField]
    private HarpoonTrigger harpoonTrigger;
    [SerializeField]
    private TMP_Text InactivityWarningText;

    [SerializeField]
    private PlayerLookSimpleEDITOR playerLookSimpleEditor;


    private float previousX = 0;
    private float previousY = 0;

    [SerializeField]
    private bool InTutorial = false;

    public bool inStore = false;

    private void Start()
    {
        Timer = InactivityTime;
        if (!InTutorial)
        {
            StartCoroutine("MyUpdate");
        }

        else
        {
            StartCoroutine("MyUpdateTut");
        }
    }

    IEnumerator MyUpdate()
    {
        previousX = playerMovementWater.lookX;
        previousY = playerMovementWater.lookY;
        yield return new WaitForSeconds(1);

        if (previousX != playerMovementWater.lookX || previousY != playerMovementWater.lookY || harpoonTrigger.isReeling)
        {
            previousX = playerMovementWater.lookX;
            previousY = playerMovementWater.lookY;
            Timer = InactivityTime;
            InactivityWarningText.text = string.Empty;
            StartCoroutine("MyUpdate");
            yield return null;
        }

        else
        {
            if (!inStore)
            {
                Timer--;
            }

            if (Timer <= 30)
            {
                InactivityWarningText.text = "Inactivity detected. Ending session in " + Timer + " seconds";
            }
            else
            {
                InactivityWarningText.text = string.Empty;
            }

            if (Timer <= 0)
            {
                shopManager.BackToMenu(2);
            }

            else
            {
                StartCoroutine("MyUpdate");
            }
        }
    }

    IEnumerator MyUpdateTut()
    {
        previousX = playerLookSimpleEditor.horizontalLook;
        previousY = playerLookSimpleEditor.verticalLook;
        yield return new WaitForSeconds(1);

        if (previousX != playerLookSimpleEditor.horizontalLook || previousY != playerLookSimpleEditor.verticalLook)
        {
            previousX = playerLookSimpleEditor.horizontalLook;
            previousY = playerLookSimpleEditor.verticalLook;
            Timer = InactivityTime;
            InactivityWarningText.text = string.Empty;
            StartCoroutine("MyUpdateTut");
            yield return null;
        }

        else
        {
            Timer--;

            if (Timer <= 30)
            {
                InactivityWarningText.text = "Inactivity detected. Ending session in " + Timer + " seconds";
            }
            else
            {
                InactivityWarningText.text = string.Empty;
            }

            if (Timer <= 0)
            {
                SceneManager.LoadScene(0);
            }

            else
            {
                StartCoroutine("MyUpdateTut");
            }
        }
    }
}
