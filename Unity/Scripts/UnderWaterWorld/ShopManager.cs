using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Item cost = baseCost + moduleLevel*multiplier")]

    [Header("Depth module cost")]
    [SerializeField] private int depthModuleBaseCost = 1500;
    [SerializeField] private int depthModuleLevelMultiplier = 2000;

    [Header("Temperature module cost")]
    [SerializeField] private int tempModuleBaseCost = 1500;
    [SerializeField] private int tempModuleLevelMultiplier = 2000;

    [Header("Reel module cost")]
    [SerializeField] private int reelModuleBaseCost = 600;
    [SerializeField] private int reelModuleLevelMultiplier = 1900;

    [Header("Storage module cost")]
    [SerializeField] private int storageModuleBaseCost = 600;
    [SerializeField] private int storageModuleLevelMultiplier = 1900;

    [Header("Flashlight module cost")]
    [SerializeField] private int flashlightBaseCost = 5000;

    [HideInInspector][SerializeField] PlayerProgress _PlayerProgress;
    [HideInInspector][SerializeField] private int shipFlashlight;
    [HideInInspector][SerializeField] private int shipDepthModule;
    [HideInInspector][SerializeField] private int shipStorageModule;
    [HideInInspector][SerializeField] private int shipReelStrenghtModule;
    [HideInInspector][SerializeField] private int shipTemperatureModule;

    [HideInInspector][SerializeField] private GameObject[] Panels;
    [HideInInspector][SerializeField] private Button[] Buttons;

    [HideInInspector][SerializeField] private int itemIndex;
    [HideInInspector][SerializeField] int itemCost = 9999999;
    [HideInInspector][SerializeField] private TMP_Text descriptionText;
    [HideInInspector][SerializeField] private TMP_Text comprarCusto;
    [HideInInspector][SerializeField] private TMP_Text[] playerMoney;
    [HideInInspector][SerializeField] private TMP_Text stats;

    private string itemDescription;
    [HideInInspector][SerializeField] private Inventory inventory;
    [HideInInspector][SerializeField] private UnderwaterInventory underwaterInventory;

    [HideInInspector][SerializeField] private Button BuyButton;

    [HideInInspector][SerializeField] private Animation animationStore;
    [HideInInspector][SerializeField] private Animation playerCameraAnimation;
    private ShowInventoryStore showInventoryStore;
    [HideInInspector][SerializeField] private TMP_Text ShowMesssageStore;
    [HideInInspector][SerializeField] private TMP_Text GainSpentMoneyText;
    [HideInInspector][SerializeField] private AchievementManager AchievementManager;
    private AudioSource audioSource;
    [HideInInspector][SerializeField] private DayManager dayManager;
    int moneyGained;
    int creditsGained;
    private bool newDayWaiting = false;
    TimeSpan timePlayed;
    [HideInInspector] public DateTime dateNow;

    [HideInInspector] public GameObject[] DeepModule;
    [HideInInspector] public GameObject[] TempModule;
    [HideInInspector] public GameObject[] ReelModule;
    [HideInInspector] public GameObject[] StorageModule;
    [HideInInspector] public GameObject FlashLight;

    public GameObject ConfirmBackToMenu;

    private void Start()
    {
        dateNow = DateTime.Now;
        audioSource = GetComponent<AudioSource>();
        showInventoryStore = GetComponent<ShowInventoryStore>();
        shipDepthModule = _PlayerProgress.shipDepthModule;
        shipFlashlight = _PlayerProgress.shipFlashlight;
        shipReelStrenghtModule = _PlayerProgress.shipReelStrenghtModule;
        shipStorageModule = _PlayerProgress.shipStorageModule;
        shipTemperatureModule = _PlayerProgress.shipTemperatureModule;
        BuyButton.interactable = false;
        Cursor.lockState = CursorLockMode.None;
        playerMoney[0].text = "Moedas: " + _PlayerProgress.Money;
        playerMoney[1].text = "Creditos: " + _PlayerProgress.Creditos;
        //playerMoney[1].text = "Moedas: " + _PlayerProgress.Money;
        DecorateSub();
    }

    public void ChooseItem(int x)
    {
        shipDepthModule = _PlayerProgress.shipDepthModule;
        shipFlashlight = _PlayerProgress.shipFlashlight;
        shipReelStrenghtModule = _PlayerProgress.shipReelStrenghtModule;
        shipStorageModule = _PlayerProgress.shipStorageModule;
        shipTemperatureModule = _PlayerProgress.shipTemperatureModule;
        DecorateSub();
        itemIndex = x;

        switch (itemIndex)
        {
            case 0:
                itemCost = depthModuleBaseCost + shipDepthModule*depthModuleLevelMultiplier;
                itemDescription = "Este m�dulo garante mais protec��o ao submarino podendo ir at� mais fundo!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                if (shipDepthModule < 2)
                    DeepModule[shipDepthModule].SetActive(true);
                break;
            case 1:
                itemCost = tempModuleBaseCost + shipTemperatureModule * tempModuleLevelMultiplier;
                itemDescription = "Este m�dulo garante mais protec��o ao submarino permitindo aguentar temperaturas mais baixas (n�vel 1) ou elevadas (n�vel 2)!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                if (shipTemperatureModule < 2)
                    TempModule[shipTemperatureModule].SetActive(true);
                break;
            case 2:
                itemCost = reelModuleBaseCost + shipReelStrenghtModule * reelModuleLevelMultiplier;
                itemDescription = "Este m�dulo equipa o submarino com uma corda mais comprida e um motor que permite disparar a ventosa mais r�pido!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                if (shipReelStrenghtModule < 2)
                    ReelModule[shipReelStrenghtModule].SetActive(true);
                break;
            case 3:
                itemCost = storageModuleBaseCost + shipStorageModule * storageModuleLevelMultiplier; ;
                itemDescription = "Este m�dulo aumenta o tamanho do reservat�rio. Assim pode apanhar mais peixes, ou peixes maiores!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                if(shipStorageModule<2)
                    StorageModule[shipStorageModule].SetActive(true);
                break;
            case 4:
                itemCost = flashlightBaseCost;
                itemDescription = "Este m�dulo adiciona uma lanterna para conseguir ver nas zonas mais escuras!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                FlashLight.SetActive(false);
                break;
        }

        if (itemIndex == 4 && shipFlashlight > 0)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "N�vel m�ximo!";
            return;
        }

        if (itemIndex == 0 && shipDepthModule == 2)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "N�vel m�ximo!";
            return;
        }

        if (itemIndex == 1 && shipTemperatureModule == 2)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "N�vel m�ximo!";
            return;
        }

        if (itemIndex == 2 && shipReelStrenghtModule == 2)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "N�vel m�ximo!";
            return;
        }

        if (itemIndex == 3 && shipStorageModule == 2)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "N�vel m�ximo!";
            return;
        }

        if (_PlayerProgress.Money - itemCost < 0)
        {
            BuyButton.interactable = false; 
            return;
        }

        BuyButton.interactable = true;      
    }

    private void DecorateSub()
    {
        DeepModule[0].SetActive(false);
        DeepModule[1].SetActive(false);
        FlashLight.SetActive(true);
        TempModule[0].SetActive(false);
        TempModule[1].SetActive(false);
        ReelModule[1].SetActive(false);
        ReelModule[0].SetActive(false);
        StorageModule[1].SetActive(false);
        StorageModule[0].SetActive(false);

        if (shipFlashlight > 0)
        {
            FlashLight.SetActive(false);
        }

        if (shipDepthModule > 0)
        {
            if(shipDepthModule == 1)
                DeepModule[shipDepthModule-1].SetActive(true);
            else
            {
                DeepModule[shipDepthModule - 1].SetActive(true);
                DeepModule[shipDepthModule].SetActive(true);
            }
        }

        if (shipTemperatureModule > 0)
        {
            TempModule[shipTemperatureModule-1].SetActive(true);
        }

        if (shipReelStrenghtModule > 0)
        {
            ReelModule[shipReelStrenghtModule-1].SetActive(true);
        }

        if (shipStorageModule > 0)
        {
            StorageModule[shipStorageModule - 1].SetActive(true);
        }
    }

    public void BuyItem()
    {

        if (_PlayerProgress.Money - itemCost < 0)
        {
            return;
        }

        _PlayerProgress.UpdateMoney(-itemCost);
        _PlayerProgress.BuyItemIndex(itemIndex);
        animationStore.Play("BuyItemAnimation");
        GainSpentMoneyText.text = "-" + itemCost;
        GainSpentMoneyText.color = Color.red;
        playerMoney[0].text = "Moedas: " + _PlayerProgress.Money;
        playerMoney[1].text = "Creditos: " + _PlayerProgress.Creditos;
        ChooseItem(itemIndex);
    }

    public void StorePanelManager(string anim)
    {
        moneyGained = showInventoryStore.HandleStoreInventory(underwaterInventory);
        moneyGained += showInventoryStore.HandleStoreInventory(inventory);
        creditsGained = showInventoryStore.HandleAchievCreditsStore(inventory);
        creditsGained += showInventoryStore.HandleAchievCreditsStore(underwaterInventory);
        creditsGained += _PlayerProgress.CheckCreditsGainedFromMoney(moneyGained);
        creditsGained += _PlayerProgress.CreditosGainedDay;

        string message = CreateFunnyMessages(moneyGained);
        ShowMesssageStore.text = message;

        int fishCount = inventory.fishListMoneyWorth.Count + underwaterInventory.fishListMoneyWorth.Count;
        int achievCount = inventory.achievListMoneyWorth.Count + underwaterInventory.achievListMoneyWorth.Count;
        timePlayed = DateTime.Now - dateNow;

        string statsString = "- Apanhou " + fishCount + " peixes!\n\n";
        if (timePlayed.Minutes == 0)
        {
            statsString += "- Este dia durou " + timePlayed.Seconds + " segundos!\n\n";
        }
        else
        {
            statsString += "- Este dia durou " + timePlayed.Minutes + " minutos!\n\n";
        }
        
        statsString += "- Apanhou " + achievCount + " objectos raros!";
        stats.text = statsString;       
    }

    public void UpdateShop()
    {
        moneyGained = showInventoryStore.HandleStoreInventory(underwaterInventory);
        moneyGained += showInventoryStore.HandleStoreInventory(inventory);

        creditsGained = showInventoryStore.HandleAchievCreditsStore(inventory);
        creditsGained += showInventoryStore.HandleAchievCreditsStore(underwaterInventory);
        creditsGained += _PlayerProgress.CheckCreditsGainedFromMoney(moneyGained);
        creditsGained += _PlayerProgress.CreditosGainedDay;

        string message = CreateFunnyMessages(moneyGained);
        ShowMesssageStore.text = message;

        int fishCount = inventory.fishListMoneyWorth.Count + underwaterInventory.fishListMoneyWorth.Count;
        int achievCount = inventory.achievListMoneyWorth.Count + underwaterInventory.achievListMoneyWorth.Count;
        timePlayed = DateTime.Now - dateNow;

        string statsString = "- At� agora apanhou " + fishCount + " peixes!\n\n";
        if (timePlayed.Minutes == 0)
        {
            statsString += "- Este dia durou " + timePlayed.Seconds + " segundos!\n\n";
        }
        else
        {
            statsString += "- Este dia durou " + timePlayed.Minutes + " minutos!\n\n";
        }
        statsString += "- At� agora tem " + moneyGained + " moedas em peixe!\n\n";
        statsString += "- At� agora conseguiu " + creditsGained + " cr�ditos!\n\n"; 
        statsString += "- Apanhou " + achievCount + " objectos raros!";

        stats.text = statsString;
    }

    private string CreateFunnyMessages(int moneyGained)
    {
        string[] messages = { "", "", "", "" };

        string message = "";
        if (AchievementManager.achievementsGainedType.Count > 0)
        {
            message = "Uau! Parab�ns! Encontrou algo raro! Os chefes v�o ficar contentes! Vale: "+moneyGained;
            return message;
        }

        else
        {
            if (moneyGained > 2000)
            {
                messages[0] = "El������! Isto vale " + moneyGained + " moedas! O pr�ximo jantar fica por sua conta!";
                messages[1] = "Melhor s� o euromilh�es! Isto vale " + moneyGained + " moedas!";
                messages[2] = "Como diria o Fernando Mendes, ESPET�CULOOO! Isto vale " + moneyGained + " moedas!";
                messages[3] = "Ena p�! Mais uns dias assim e deixamos de trabalhar! Isto vale " + moneyGained + " moedas!";
            }

            if (moneyGained > 1000 && moneyGained < 2000)
            {
                messages[0] = "Uau! Que qualidade! Isto vale " + moneyGained + " moedas! A investiga��o est� a dar lucro!";
                messages[1] = "Uau! Que qualidade! Isto vale " + moneyGained + " moedas! A investiga��o est� a dar lucro!";
                messages[2] = "Uau! Que qualidade! Isto vale " + moneyGained + " moedas! A investiga��o est� a dar lucro!";
                messages[3] = "Uau! Que qualidade! Isto vale " + moneyGained + " moedas! A investiga��o est� a dar lucro!";
            }

            if (moneyGained < 1000 && moneyGained > 300)
            {
                messages[0] = "Bom trabalho! Isto vale " + moneyGained + " moedas! Continue assim!";
                messages[1] = "Foi um bom dia hoje! Isto vale " + moneyGained + " moedas!";
                messages[2] = "Os chefes v�o ficar satisfeitos! Isto vale " + moneyGained + " moedas!";
                messages[3] = "Boa! Assim n�o nos cortam o financiamento! Isto vale " + moneyGained + " moedas!";
            }

            if (moneyGained < 300)
            {
                messages[0] = "Nada mau! Isto vale " + moneyGained + " moedas!";
                messages[1] = "Mais um dia de trabalho! Isto vale " + moneyGained + " moedas!";
                messages[2] = "Mais uma semana a comer atum! Isto vale " + moneyGained + " moedas!";
                messages[3] = "A continuar assim vamos � fal�ncia! Isto vale " + moneyGained + " moedas!";
            }


            int i = UnityEngine.Random.Range(0, 4);
            message = messages[i];

            return message;
        }
    }

    public void BackToMenu(int x)
    {
        if (x == 0)
        {
            ConfirmBackToMenu.SetActive(false);
        }

        if (x == 1)
        {
            ConfirmBackToMenu.SetActive(true);
        }

        if (x == 2)
        {
            SellFish();
            ConfirmBackToMenu.SetActive(false);
            SceneManager.LoadScene(0);
        }
    }
    public void SellFish()
    {
        if (newDayWaiting)
        {
            return;
        }
        
        dateNow = DateTime.Now;
        newDayWaiting = true;
        StartCoroutine("WaitNewDay"); // prevents users from spamming button
        _PlayerProgress.TimePlayed = timePlayed;
        _PlayerProgress.UpdateFishCaught(inventory.fishDayCatched);

        _PlayerProgress.UpdateCreditsCombined(creditsGained);

        inventory.fishDayCatched = 0;
        inventory.fishListMoneyWorth.Clear();
        inventory.achievListMoneyWorth.Clear();
        inventory.fishListType.Clear();
        inventory.achievListType.Clear();
        inventory.ResetStorage();

        audioSource.Play();
        AchievementManager.ShowAchievementGained();

        GainSpentMoneyText.text = "+" + moneyGained;
        GainSpentMoneyText.color = Color.green;

        _PlayerProgress.UpdateMoney(moneyGained);


        playerMoney[0].text = "Moedas: " + _PlayerProgress.Money;
        playerMoney[1].text = "Creditos: " + _PlayerProgress.Creditos;
        animationStore.Play("CoinAnim");
        moneyGained = 0;
        timePlayed = TimeSpan.Zero;
        underwaterInventory.ClearList();
        dayManager.TerminateDay();
        _PlayerProgress.CreditosGainedDay = 0;
    }

    private IEnumerator WaitNewDay()
    {
        yield return new WaitForSeconds(5);
        newDayWaiting = false;
    }
}
