using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [HideInInspector][SerializeField] private PlayerProgress progress;
    [HideInInspector][SerializeField] private int StorageSize;
    [HideInInspector][SerializeField] private int currentStorageSpace;
    [HideInInspector][SerializeField] private int money;
    [HideInInspector][SerializeField] private int Gems;
    [HideInInspector][SerializeField] private TMP_Text StorageSizeText;
    [HideInInspector]public bool fullStorage = false;
    [HideInInspector][SerializeField] public List<int> fishListMoneyWorth;
    [HideInInspector][SerializeField] public List<int> fishListType;
    [HideInInspector][SerializeField] public List<int> achievListMoneyWorth;
    [HideInInspector][SerializeField] public List<int> achievListType;
    [HideInInspector][SerializeField] public List<int> achievListCreditWorth;
    [HideInInspector] public int fishDayCatched = 0;

    private void Start()
    {
        int storageModule = progress.shipStorageModule;
        switch(storageModule)
        {
            case 0:
                StorageSize = 10;
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
                StorageSize = 10;
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
}
