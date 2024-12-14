using UnityEngine.UI;
using UnityEngine;

public class ShopProxyScript : MonoBehaviour
{
    public Button currentButton;
    public bool firedButton = false;

    private void Update()
    {
        if (!firedButton)
            return;

        firedButton = false;
        ClickButton();       
    }

    public void ClickButton()
    {
        if (currentButton != null)
        {
            currentButton.onClick.Invoke();
        }
    }
}
