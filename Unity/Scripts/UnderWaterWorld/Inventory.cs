using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private PlayerProgress progress;
    [SerializeField] private int StorageSize;
    [SerializeField] private int currentStorageSpace;
    [SerializeField] private int money;
    [SerializeField] private int Gems;
    [SerializeField] private TMP_Text StorageSizeText;
    public bool fullStorage = false;
    [SerializeField] public List<int> fishListMoneyWorth;
    [SerializeField] public List<int> fishListType;
    [SerializeField] public List<int> achievListMoneyWorth;
    [SerializeField] public List<int> achievListType;
    [SerializeField] public List<int> achievListCreditWorth;

    public int fishDayCatched = 0;
    private void Start()
    {
        int storageModule = progress.shipStorageModule;
        switch(storageModule)
        {
            case 0:
                StorageSize = 20;
                break;
            case 1:
                StorageSize = 50;
                break;
            case 2:
                StorageSize = 150;
                break;
        }

        currentStorageSpace = 0;
        StorageSizeText.text = "0/" + StorageSize;
    }

    public bool CheckIfGoingFull(int fishSize)
    {
        int storageSpace = currentStorageSpace + fishSize;
        if (storageSpace > StorageSize)
        {
            fullStorage = true;
            return fullStorage;
        }

        else
        {
            fullStorage = false;
            return fullStorage;
        }
    }

    public void ControlStorageSpace(Fish fish)
    {
        fishDayCatched++;
        int storageSpace = currentStorageSpace + fish.fishSpaceSize;
        currentStorageSpace = storageSpace;
        StorageSizeText.text = currentStorageSpace+"/" + StorageSize;
        fishListMoneyWorth.Add(fish.moneyWorth);
        fishListType.Add(fish.fishType);
    }

    public void ControlStorageSpaceAchiev(AchievementObject achievObject)
    {
        int storageSpace = currentStorageSpace + achievObject.Size;
        currentStorageSpace = storageSpace;
        StorageSizeText.text = currentStorageSpace + "/" + StorageSize;
        achievListMoneyWorth.Add(achievObject.moneyWorth);
        achievListCreditWorth.Add(achievObject.creditWorth);
        achievListType.Add(achievObject.type);
    }

    public void ResetStorage()
    {
        currentStorageSpace = 0;
        StorageSizeText.text = "0/" + StorageSize;
    }

    public void UpdateStorageSpace()
    {
        int storageModule = progress.shipStorageModule;
        switch (storageModule)
        {
            case 0:
                StorageSize = 15;
                break;
            case 1:
                StorageSize = 30;
                break;
            case 2:
                StorageSize = 100;
                break;
        }

        currentStorageSpace = 0;
        StorageSizeText.text = "0/" + StorageSize;
    }
}
