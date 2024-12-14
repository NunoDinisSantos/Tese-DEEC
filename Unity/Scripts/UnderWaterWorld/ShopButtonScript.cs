using UnityEngine;
using UnityEngine.UI;

public class ShopButtonScript : MonoBehaviour
{
    [SerializeField] private Image myButtonImage;
    [SerializeField] private ShopProxyScript shopProxyScript;
    [SerializeField] private Button myButton;

    public void SelectButton()
    {
        myButtonImage.color = Color.green;
        shopProxyScript.currentButton = myButton;
    }

    public void DeSelect()
    {
        myButtonImage.color = new Color(28, 200, 255);
        shopProxyScript.currentButton = null;

    }
}
