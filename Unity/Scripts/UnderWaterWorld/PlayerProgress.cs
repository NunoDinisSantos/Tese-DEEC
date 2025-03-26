using System;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    [HideInInspector] public DataBaseLoaderScript database;
    [Header("Creditos & Divisor")]
    public int Creditos;
    public int CreditosGainedDay;
    [Header("Valor convertido de moedas convertidas a créditos. 0.05 => 5%")]
    [SerializeField] private float convertor = 0.05f; // 5%
    [HideInInspector] public float MultiplierStreak = 1.0f;

    [HideInInspector] public int Money;
    [HideInInspector] public TimeSpan TimePlayed;
    //[HideInInspector] public float PatientYHelp;
    //[HideInInspector] public float PatientXHelp;
    [HideInInspector] public bool AutomaticReel;
    [HideInInspector] public int FishCaught;
    [HideInInspector] public int TreasuresCaught;
    [HideInInspector] public int Days;
    [HideInInspector] public int TUTORIAL;

    [HideInInspector] public int shipFlashlight = 0;
    [HideInInspector] public int shipDepthModule = 0;
    [HideInInspector] public int shipStorageModule = 0;
    [HideInInspector] public int shipReelStrenghtModule = 0;
    [HideInInspector] public int shipTemperatureModule = 0;

    [HideInInspector] public Inventory inventory;
    [HideInInspector] public PlayerMovementWater PlayerMovementWater;
    [HideInInspector] public PlayerVisionController PlayerVisionController;
    [HideInInspector] public HarpoonTrigger HarpoonTrigger;
    [HideInInspector] public PlayerHealth playerHealth;

    [SerializeField] private float multiPlierDay0 = 1.00f;
    [SerializeField] private float multiPlierDay1 = 1.25f;
    [SerializeField] private float multiPlierDay2 = 1.5f;
    [SerializeField] private float multiPlierDay3 = 2.0f;


    void Awake()
    {
        //Screen.sleepTimeout = SleepTimeout.NeverSleep;

        #region GetPlayerData
        shipDepthModule = PlayerDataScript.playerDataInstance.DepthModule;
        shipTemperatureModule = PlayerDataScript.playerDataInstance.TempModule;
        shipFlashlight = PlayerDataScript.playerDataInstance.Flashlight;
        shipStorageModule = PlayerDataScript.playerDataInstance.StorageModule;
        shipReelStrenghtModule = PlayerDataScript.playerDataInstance.ReelModule;
        Money = PlayerDataScript.playerDataInstance.Coins;
        FishCaught = PlayerDataScript.playerDataInstance.FishCaught;
        TUTORIAL = PlayerDataScript.playerDataInstance.Tutorial;
        Creditos = PlayerDataScript.playerDataInstance.Credits;
        Days = PlayerDataScript.playerDataInstance.Days;

        switch (PlayerDataScript.playerDataInstance.DayStreak)
        {
            case 0:
                MultiplierStreak = multiPlierDay0;
                break;
            case 1:
                MultiplierStreak = multiPlierDay1;
                break;
            case 2:
                MultiplierStreak = multiPlierDay2;
                break;
            case 3:
                MultiplierStreak = multiPlierDay3;
                break;
        }
        #endregion

        /*
                //############################################################
                #region Modules
                if (PlayerPrefs.HasKey("DModule"))
                {
                    shipDepthModule = PlayerPrefs.GetInt("DModule");
                }
                else
                {
                    PlayerPrefs.SetInt("DModule",0);
                    shipDepthModule = 0;
                    playerHealth.UpdateModules();
                }
                //############################################################
                //############################################################

                if (PlayerPrefs.HasKey("TModule"))
                {
                    shipTemperatureModule = PlayerPrefs.GetInt("TModule");
                }
                else
                {
                    PlayerPrefs.SetInt("TModule", 0);
                    shipTemperatureModule = 0;
                }
                //############################################################
                //############################################################

                if (PlayerPrefs.HasKey("FModule"))
                {
                    shipFlashlight = PlayerPrefs.GetInt("FModule");
                }
                else
                {
                    PlayerPrefs.SetInt("FModule", 0);
                    shipFlashlight = 0;
                }
                //############################################################
                //############################################################

                if (PlayerPrefs.HasKey("STModule"))
                {
                    shipStorageModule = PlayerPrefs.GetInt("STModule");
                }
                else
                {
                    PlayerPrefs.SetInt("STModule", 0);
                    shipStorageModule = 0;
                }
                //############################################################
                //############################################################

                if (PlayerPrefs.HasKey("RSModule"))
                {
                    shipReelStrenghtModule = PlayerPrefs.GetInt("RSModule");
                }
                else
                {
                    PlayerPrefs.SetInt("RSModule", 0);
                    shipReelStrenghtModule = 0;
                }
                //############################################################
                #endregion
                #region Progress
                if (PlayerPrefs.HasKey("Money"))
                {
                    Money = PlayerPrefs.GetInt("Money");
                }
                else
                {
                    PlayerPrefs.SetInt("Money",0);
                    Money = 0;
                }
                //############################################################
                //############################################################

                if (PlayerPrefs.HasKey("FishCaught"))
                {
                    FishCaught = PlayerPrefs.GetInt("FishCaught");
                }
                else
                {
                    PlayerPrefs.SetInt("FishCaught", 0);
                    FishCaught = 0;
                }
                //############################################################
                //############################################################
                if (PlayerPrefs.HasKey("TRCAUGHT"))
                {
                    TreasuresCaught = PlayerPrefs.GetInt("TRCAUGHT");
                }
                else
                {
                    PlayerPrefs.SetInt("TRCAUGHT", 0);
                    TreasuresCaught = 0;
                }

                if (PlayerPrefs.HasKey("Days"))
                {
                    Days = PlayerPrefs.GetInt("Days");
                }
                else
                {
                    PlayerPrefs.SetInt("Days", 0);
                    Days = 0;
                }

                if (PlayerPrefs.HasKey("TUTORIAL"))
                {
                    TUTORIAL = PlayerPrefs.GetInt("TUTORIAL");
                }
                else
                {
                    PlayerPrefs.SetInt("TUTORIAL", 0);
                    TUTORIAL = 0;
                }
                #endregion*/
    }

    public void IncrementDays()
    {
        PlayerDataScript.playerDataInstance.Days++;
        CallSaveDay(Days++);
    }

    public void BuyItemIndex(int index)
    {
        switch (index)
        {
            case 0:
                shipDepthModule++;
                PlayerDataScript.playerDataInstance.DepthModule = shipDepthModule;
               
                break;
            case 1:
                shipTemperatureModule++;
                PlayerDataScript.playerDataInstance.TempModule = shipTemperatureModule;
                //playerHealth.UpdateModules();

                break;
            case 2:
                shipReelStrenghtModule++;
                PlayerDataScript.playerDataInstance.ReelModule = shipReelStrenghtModule;
                //HarpoonTrigger.UpdateModule();
                break;
            case 3:
                shipStorageModule++;
                PlayerDataScript.playerDataInstance.StorageModule = shipStorageModule;
                //inventory.UpdateStorageSpace();
                break;
            case 4:
                shipFlashlight++;
                PlayerDataScript.playerDataInstance.Flashlight = shipFlashlight;
                PlayerVisionController.UpdateFlashlight();
                break;
        }

        database.CallUpdateModules(shipDepthModule, shipTemperatureModule, shipReelStrenghtModule, shipStorageModule, shipFlashlight, Money);
        inventory.UpdateStorageSpace();
        HarpoonTrigger.UpdateModule();
        playerHealth.UpdateModules();
        PlayerVisionController.UpdateFlashlight();
    }

    public void UpdateMoney(int money)
    {
        Money += money;
        if (money > 0)
        {
            Creditos += Mathf.RoundToInt(money * convertor);
        }

        PlayerDataScript.playerDataInstance.Coins = Money;
    }

    public int CheckCreditsGainedFromMoney(int money)
    {
        return Mathf.RoundToInt(money * convertor);
    }

    public void UpdateCredits(int credits)
    {
        credits = Mathf.RoundToInt(credits);
        Creditos = Mathf.RoundToInt(Creditos + credits * MultiplierStreak);
        CreditosGainedDay += credits;
        PlayerDataScript.playerDataInstance.Credits = Creditos;
    }

    public void UpdateFishCaught(int nFish)
    {
        FishCaught += nFish;
        PlayerDataScript.playerDataInstance.FishCaught = FishCaught;
    }

    public void CallSaveDay(int days)
    {
        database.CallUpdateDays(days,FishCaught,Money,Creditos,TimePlayed);
    }
}