using UnityEngine;

public class ShopStickPicker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<ShopButtonScript>().SelectButton();
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<ShopButtonScript>().DeSelect();
    }
}
