using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] PlayerProgress _PlayerProgress;
    [SerializeField] private int shipFlashlight;
    [SerializeField] private int shipDepthModule;
    [SerializeField] private int shipStorageModule;
    [SerializeField] private int shipReelStrenghtModule;
    [SerializeField] private int shipTemperatureModule;

    [SerializeField] private GameObject[] Panels;
    [SerializeField] private Button[] Buttons;

    [SerializeField] private int itemIndex;
    [SerializeField] int itemCost = 0;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text comprarCusto;
    [SerializeField] private TMP_Text[] playerMoney;
    [SerializeField] private TMP_Text stats;

    private string itemDescription;
    [SerializeField] private Inventory inventory;
    [SerializeField] private UnderwaterInventory underwaterInventory;

    [SerializeField] private Button BuyButton;

    [SerializeField] private Animation animationStore;
    [SerializeField] private Animation playerCameraAnimation;
    private ShowInventoryStore showInventoryStore;
    [SerializeField] private TMP_Text ShowMesssageStore;
    [SerializeField] private TMP_Text GainSpentMoneyText;
    [SerializeField] private AchievementManager AchievementManager;
    private AudioSource audioSource;
    [SerializeField] private DayManager dayManager;
    int moneyGained;
    int creditsGained;
    TimeSpan timePlayed;
    public DateTime dateNow;

    [Header("Modules Shop")]
    public GameObject[] DeepModule;
    public GameObject[] TempModule;
    public GameObject[] ReelModule;
    public GameObject[] StorageModule;
    public GameObject FlashLight;


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
                itemCost = 1500 + shipDepthModule*2000;
                itemDescription = "Este módulo garante mais protecção ao submarino podendo ir até mais fundo!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                if (shipDepthModule < 2)
                    DeepModule[shipDepthModule].SetActive(true);
                break;
            case 1:
                itemCost = 1500 + shipTemperatureModule * 2000;
                itemDescription = "Este módulo garante mais protecção ao submarino permitindo aguentar temperaturas mais baixas (nível 1) ou elevadas (nível 2)!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                if (shipTemperatureModule < 2)
                    TempModule[shipTemperatureModule].SetActive(true);
                break;
            case 2:
                itemCost = 600 + shipReelStrenghtModule * 600;
                itemDescription = "Este módulo equipa o submarino com uma corda mais comprida e um motor que permite disparar a ventosa mais rápido!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                if (shipReelStrenghtModule < 2)
                    ReelModule[shipReelStrenghtModule].SetActive(true);
                break;
            case 3:
                itemCost = 600 + shipStorageModule * 1900; ;
                itemDescription = "Este módulo aumenta o tamanho do reservatório. Assim pode apanhar mais peixes, ou peixes maiores!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                if(shipStorageModule<2)
                    StorageModule[shipStorageModule].SetActive(true);
                break;
            case 4:
                itemCost = 5000;
                itemDescription = "Este módulo adiciona uma lanterna para conseguir ver nas zonas mais escuras!";
                descriptionText.text = itemDescription;
                comprarCusto.text = "Comprar: " + itemCost;
                FlashLight.SetActive(false);
                break;
        }

        if (itemIndex == 4 && shipFlashlight > 0)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "Nível máximo!";
            return;
        }

        if (itemIndex == 0 && shipDepthModule == 2)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "Nível máximo!";
            return;
        }

        if (itemIndex == 1 && shipTemperatureModule == 2)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "Nível máximo!";
            return;
        }

        if (itemIndex == 2 && shipReelStrenghtModule == 2)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "Nível máximo!";
            return;
        }

        if (itemIndex == 3 && shipStorageModule == 2)
        {
            BuyButton.interactable = false;
            comprarCusto.text = "Nível máximo!";
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

    private string CreateFunnyMessages(int moneyGained)
    {
        string[] messages = { "", "", "", "" };

        string message = "";
        if (AchievementManager.achievementsGainedType.Count > 0)
        {
            message = "Uau! Parabéns! Encontrou algo raro! Os chefes vão ficar contentes! Vale: "+moneyGained;
            return message;
        }

        else
        {
            if (moneyGained > 2000)
            {
                messages[0] = "Eláááááá! Isto vale " + moneyGained + " moedas! O próximo jantar fica por sua conta!";
                messages[1] = "Melhor só o euromilhões! Isto vale " + moneyGained + " moedas!";
                messages[2] = "Como diria o Fernando Mendes, ESPETÁCULOOO! Isto vale " + moneyGained + " moedas!";
                messages[3] = "Ena pá! Mais uns dias assim e deixamos de trabalhar! Isto vale " + moneyGained + " moedas!";
            }

            if (moneyGained > 1000 && moneyGained < 2000)
            {
                messages[0] = "Uau! Que qualidade! Isto vale " + moneyGained + " moedas! A investigação está a dar lucro!";
                messages[1] = "Uau! Que qualidade! Isto vale " + moneyGained + " moedas! A investigação está a dar lucro!";
                messages[2] = "Uau! Que qualidade! Isto vale " + moneyGained + " moedas! A investigação está a dar lucro!";
                messages[3] = "Uau! Que qualidade! Isto vale " + moneyGained + " moedas! A investigação está a dar lucro!";
            }

            if (moneyGained < 1000 && moneyGained > 300)
            {
                messages[0] = "Bom trabalho! Isto vale " + moneyGained + " moedas! Continue assim!";
                messages[1] = "Foi um bom dia hoje! Isto vale " + moneyGained + " moedas!";
                messages[2] = "Os chefes vão ficar satisfeitos! Isto vale " + moneyGained + " moedas!";
                messages[3] = "Boa! Assim não nos cortam o financiamento! Isto vale " + moneyGained + " moedas!";
            }

            if (moneyGained < 300)
            {
                messages[0] = "Nada mau! Isto vale " + moneyGained + " moedas!";
                messages[1] = "Mais um dia de trabalho! Isto vale " + moneyGained + " moedas!";
                messages[2] = "Mais uma semana a comer atum! Isto vale " + moneyGained + " moedas!";
                messages[3] = "A continuar assim vamos à falência! Isto vale " + moneyGained + " moedas!";
            }


            int i = UnityEngine.Random.Range(0, 4);
            message = messages[i];

            return message;
        }
    }

    public void SellFish()
    {
        _PlayerProgress.TimePlayed = timePlayed;
        _PlayerProgress.UpdateFishCaught(inventory.fishDayCatched);
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
    }
}
