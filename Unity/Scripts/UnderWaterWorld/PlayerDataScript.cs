using System;
using UnityEngine;

public class PlayerDataScript : MonoBehaviour
{
    public static PlayerDataScript playerDataInstance;

    private void Awake()
    {
        if (playerDataInstance != null && playerDataInstance != this)
        {
            Destroy(this);
        }
        else
        {
            playerDataInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public string PlayerId;
    public string StudentNick;
    public int Coins;
    public int TimePlayed;
    public int FishCaught;
    public int Tutorial;
    public int Flashlight;
    public int DepthModule;
    public int StorageModule;
    public int ReelModule;
    public int TempModule;
    public int Days;
    public float Multiplier;
    public int DayStreak;
    public int Credits;
    public DateTime LastLogin;

    public bool Treasure;
    public bool AncientCoral;
    public bool LostResearch;
    public bool TempleJewel;
    public bool BoatJewel;
    public bool OldIce;
}
