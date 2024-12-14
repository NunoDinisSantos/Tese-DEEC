using System.Collections.Generic;
using UnityEngine;

public class UnderwaterInventory : MonoBehaviour
{
    public List<int> fishListMoneyWorth;
    public List<int> achievListMoneyWorth;
    public List<int> achievListCreditWorth;
    public List<int> achievListType;
    public List<int> fishListType;

    public void AddFishListMoneyWorth(List<int> list)
    {
        if (list.Count <= 0)
        {
            return;
        }

        fishListMoneyWorth.AddRange(list);
    }

    public void AddFishListType(List<int> list)
    {
        if (list.Count <= 0)
        {
            return;
        }

        fishListType.AddRange(list);
    }

    public void AddAchievListMoneyWorth(List<int> list)
    {
        if (list.Count <= 0)
        {
            return;
        }

        achievListMoneyWorth.AddRange(list);
    }

    public void AddAchievListType(List<int> list)
    {
        if (list.Count <= 0)
        {
            return;
        }

        achievListType.AddRange(list);
    }

    public void AddAchievCreditWorth(List<int> list)
    {
        if (list.Count <= 0)
        {
            return;
        }

        achievListCreditWorth.AddRange(list);
    }

    public void ClearList()
    {
        fishListType.Clear();
        fishListMoneyWorth.Clear();
        achievListMoneyWorth.Clear();
        achievListType.Clear();
        achievListCreditWorth.Clear();
    }
}
