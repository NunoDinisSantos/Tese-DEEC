using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ShopProxyScript : MonoBehaviour
{
    [SerializeField] public Button currentButton;
    [SerializeField] public Button previousButton;

    [HideInInspector] public bool firedButton = false;
    [HideInInspector] public bool canFire = true;

    [HideInInspector] public float verticalLook = 0;
    [HideInInspector] public float horizontalLook = 0;

    [HideInInspector] public bool inStore = false;

    public Transform PickCollider;
    private float multiplier = 5;

    private bool foundButton = false;

    private void Update()
    {
        if(!inStore)
        {
            return;
        }

        MoveCursor();

        if (!canFire)
            return;
        
        else if(firedButton)
        {
            firedButton = false;
            ClickButton();
        }
    }

    public void ClickButton()
    {
        canFire = false;
        Debug.Log("Clicked button");
        if (currentButton != null)
        {
            Debug.Log("entered button");
            currentButton.onClick.Invoke();
        }

        StartCoroutine(HandleButtonClick());
    }

    public void MoveCursorProxy(float horizontalCursor, float verticalCursor)
    {
        horizontalLook = horizontalCursor;
        verticalLook = -verticalCursor;
    }

    public void MoveCursor()
    {
        PickCollider.position = new Vector2(PickCollider.position.x + horizontalLook* multiplier, PickCollider.position.y + verticalLook*multiplier);

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(PickCollider.position.x, PickCollider.position.y);
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results.Count > 0)
        {
            for (int i = 0; i < results.Count; i++)
            {
                currentButton = results[i].gameObject.GetComponent<Button>();

                if (currentButton != null && !foundButton)
                {
                    previousButton = currentButton;
                    previousButton.gameObject.GetComponent<Image>().color = Color.green;
                    foundButton = true;
                    break;
                }
                else
                {
                    foundButton = false;
                }
                
            }
        }

        if(!foundButton && previousButton != null)
        {
            Debug.Log("Entrou");
            previousButton.gameObject.GetComponent<Image>().color = new Color(0.065f,0.515f,0.811f,1);
            //foundButton = false;
        }

    }

    IEnumerator HandleButtonClick()
    {
        yield return new WaitForSeconds(1.5f);
        canFire = true;
    }
}
