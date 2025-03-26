using UnityEngine;
using UnityEngine.UI;

public class SkipTutorialExercise : MonoBehaviour
{

    [SerializeField] private Image filler;
    private bool skipped = false;
    public bool tryingToSkip = false;
    [SerializeField] private float increment = 0.001f;
    [SerializeField] private DialogController dialogController;
    
    void Start()
    {
        filler.transform.localScale = new Vector3(0, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (skipped)
        {
            return;
        }

        if (tryingToSkip || Input.GetKey(KeyCode.M))
        {

            filler.transform.localScale = new Vector3(filler.transform.localScale.x + increment, 1, 1);

            if (filler.transform.localScale.x > 1)
            {
                filler.transform.localScale = new Vector3(1, 1, 1);
                skipped = true;
                dialogController.SkipTutorial();
            }
        }

        else
        {
            filler.transform.localScale = new Vector3(filler.transform.localScale.x - increment, 1, 1);

            if (filler.transform.localScale.x < 0)
            {
                filler.transform.localScale = new Vector3(0, 1, 1);
            }
        }
    }
}
