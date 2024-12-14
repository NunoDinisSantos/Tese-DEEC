using UnityEngine;

public class UnderWaterStoreFishScript : MonoBehaviour
{
    public int times = 3;
    public UnderwaterInventory underwaterInventory;
    public Inventory inventory;
    public AudioSource audioSource;
    public GameObject PlayerMessageScript;
    public GameObject StorageGuys;
    private void OnTriggerEnter(Collider other)
    {
        if(inventory.fishListMoneyWorth.Count == 0)
        {
            PlayerMessageScript.SetActive(true);
            PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage("N�o tem nada para guardar!");
            return;
        }

        audioSource.Play();
        //Sell sound?
        times--;

        if (times == 0)
        {
            StorageGuys.SetActive(false);
            PlayerMessageScript.SetActive(true);
            PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage("Acabou o dia para n�s! At� amanh�!");
        }
        else
        {
            PlayerMessageScript.SetActive(true);
            PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage("S� estamos aqui mais " + times + " vezes!");
        }

        underwaterInventory.AddFishListMoneyWorth(inventory.fishListMoneyWorth);
        underwaterInventory.AddAchievListMoneyWorth(inventory.achievListMoneyWorth);
        underwaterInventory.AddFishListType(inventory.fishListType);
        underwaterInventory.AddAchievListType(inventory.achievListType);
        underwaterInventory.AddAchievCreditWorth(inventory.achievListCreditWorth);

        inventory.fishListMoneyWorth.Clear();
        inventory.achievListMoneyWorth.Clear();
        inventory.fishListType.Clear();
        inventory.achievListType.Clear();
        inventory.achievListCreditWorth.Clear();
        inventory.ResetStorage();
    }
}
