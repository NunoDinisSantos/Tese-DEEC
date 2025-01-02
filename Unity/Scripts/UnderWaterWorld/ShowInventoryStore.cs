using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInventoryStore : MonoBehaviour
{
    [HideInInspector][SerializeField] private Image[] fishesCaught;
    [HideInInspector][SerializeField] private Sprite[] fishSprites;
    [HideInInspector][SerializeField] private List<int> fishTypeList;
    [HideInInspector][SerializeField] private List<int> achievTypeList;
    [HideInInspector][SerializeField] private AchievementManager achievementManager;

    public int HandleStoreInventory(Inventory inventory)
    {
        int soma = 0;

        if (inventory.fishListMoneyWorth.Count <= 0)
        {
            soma = HandleAchievStore(inventory);
            return soma; // no fish caught
        }

        for (int i = 0; i<inventory.fishListMoneyWorth.Count; i++)
        {
            soma += inventory.fishListMoneyWorth[i];
        }

        fishTypeList.Add(inventory.fishListType[0]);

        for (int i = 1; i<inventory.fishListType.Count; i++)
        {
            bool equal = false;

            for (int j = 0; j < fishTypeList.Count; j++)
            {
                if (inventory.fishListType[i] == fishTypeList[j])
                {
                    equal = true;
                    break;
                }
            }

            if(!equal)
                fishTypeList.Add(inventory.fishListType[i]);
        }

        foreach (var fish in fishesCaught)
        {
            fish.sprite = fishSprites[0];
        }

        soma += HandleAchievStore(inventory);

        return soma;
    }

    public int HandleStoreInventory(UnderwaterInventory inventory)
    {
        int soma = 0;

        if (inventory.fishListMoneyWorth.Count <= 0)
        {
            soma = HandleAchievStore(inventory);
            return soma; // no fish caught
        }

        for (int i = 0; i < inventory.fishListMoneyWorth.Count; i++)
        {
            soma += inventory.fishListMoneyWorth[i];
        }

        fishTypeList.Add(inventory.fishListType[0]);

        for (int i = 1; i < inventory.fishListType.Count; i++)
        {
            bool equal = false;

            for (int j = 0; j < fishTypeList.Count; j++)
            {
                if (inventory.fishListType[i] == fishTypeList[j])
                {
                    equal = true;
                    break;
                }
            }

            if (!equal)
                fishTypeList.Add(inventory.fishListType[i]);
        }

        foreach (var fish in fishesCaught)
        {
            fish.sprite = fishSprites[0];
        }

        soma += HandleAchievStore(inventory);

        return soma;
    }
    
    private int HandleAchievStore(Inventory inventory)
    {        
        int soma = 0;
        if (inventory.achievListType.Count <= 0) { return 0; }
        else
        {
            for (int i = 0; i < inventory.achievListMoneyWorth.Count; i++)
            {
                achievementManager.GetAchievements(inventory.achievListType[i]);
                soma += inventory.achievListMoneyWorth[i];
            }
        }

        return soma;
    }

    private int HandleAchievStore(UnderwaterInventory inventory)
    {
        int soma = 0;
        if (inventory.achievListType.Count <= 0) { return 0; }
        else
        {
            for (int i = 0; i < inventory.achievListMoneyWorth.Count; i++)
            {
                achievementManager.GetAchievements(inventory.achievListType[i]);
                soma += inventory.achievListMoneyWorth[i];
            }
        }

        return soma;
    }

    public int HandleAchievCreditsStore(UnderwaterInventory inventory)
    {
        int somaCredits = 0;
        if (inventory.achievListType.Count <= 0) { return 0; }
        else
        {
            for (int i = 0; i < inventory.achievListMoneyWorth.Count; i++)
            {
                somaCredits += inventory.achievListCreditWorth[i];
            }
        }

        return somaCredits;
    }

    public int HandleAchievCreditsStore(Inventory inventory)
    {
        int somaCredits = 0;
        if (inventory.achievListType.Count <= 0) { return 0; }
        else
        {
            for (int i = 0; i < inventory.achievListMoneyWorth.Count; i++)
            {
                somaCredits += inventory.achievListCreditWorth[i];
            }
        }

        return somaCredits;
    }
}
