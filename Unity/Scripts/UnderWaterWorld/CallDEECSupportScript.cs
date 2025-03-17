using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CallDEECSupportScript : MonoBehaviour
{
    public bool callingSupport = false;

    [SerializeField] private Image filler;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform holder;
    private bool supportCalled = false;
    [SerializeField] private float increment = 0.001f;

    void Start()
    {
        filler.transform.localScale = new Vector3(0, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(supportCalled)
        {
            return;
        }

        if (callingSupport || Input.GetKey(KeyCode.M))
        {
            holder.gameObject.SetActive(true);

            filler.transform.localScale = new Vector3(filler.transform.localScale.x + increment, 1, 1);

            if (filler.transform.localScale.x > 1)
            {
                filler.transform.localScale = new Vector3(1, 1, 1);
                supportCalled = true;
                StartCoroutine(CallSupport());
            }
        }

        else
        {
            filler.transform.localScale = new Vector3(filler.transform.localScale.x - increment, 1, 1);

            if (filler.transform.localScale.x < 0)
            {
                filler.transform.localScale = new Vector3(0, 1, 1);
                holder.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator CallSupport()
    {
        callingSupport = false;
        holder.gameObject.SetActive(false);
        filler.transform.localScale = Vector3.zero;
        playerHealth.CallDEECPickUp();
        yield return new WaitForSeconds(2);
        supportCalled = false;
    }
}
