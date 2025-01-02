using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [HideInInspector] public List<int> achievementsGainedType = new();
    PlayerProgress progress;
    [HideInInspector] public bool hasCoralAchiev;
    [HideInInspector] public bool hasTempleAchiv;
    [HideInInspector] public bool hasShipAchiev;
    [HideInInspector] public bool hasBaseAchiv;
    [HideInInspector] public bool hasIceAchiv;
    [HideInInspector] public bool hasTreasureAchiev;
    [HideInInspector] public Animation canvasAchievAnimation;
    [HideInInspector] public TMP_Text AchievementTextTitle;
    [HideInInspector] public TMP_Text AchievementText;
    [HideInInspector] public Image achievImage;
    int counter = 0;
    [HideInInspector] public GameObject[] AchievementsObjects;
    [HideInInspector] public DataBaseLoaderScript database;

    void Start()
    {
        hasCoralAchiev = PlayerDataScript.playerDataInstance.AncientCoral;
        hasTempleAchiv = PlayerDataScript.playerDataInstance.TempleJewel;
        hasShipAchiev = PlayerDataScript.playerDataInstance.BoatJewel;
        hasBaseAchiv = PlayerDataScript.playerDataInstance.LostResearch;
        hasIceAchiv = PlayerDataScript.playerDataInstance.OldIce;
        hasTreasureAchiev = PlayerDataScript.playerDataInstance.Treasure;

        ActivateDeActivateAchievObjects();
    }

    private void ActivateDeActivateAchievObjects()
    {
        if(hasCoralAchiev)
        {
            AchievementsObjects[0].SetActive(false);
        }

        if (hasTempleAchiv)
        {
            AchievementsObjects[1].SetActive(false);
        }

        if (hasShipAchiev)
        {
            AchievementsObjects[2].SetActive(false);
        }

        if (hasBaseAchiv )
        {
            AchievementsObjects[3].SetActive(false);
        }

        if (hasTreasureAchiev)
        {
            AchievementsObjects[4].SetActive(false);
        }


        if (hasIceAchiv)
        {
            AchievementsObjects[5].SetActive(false);
        }
    }

    public void GetAchievements(int indexType)
    {
        achievementsGainedType.Add(indexType);

        //StartCoroutine(GainAchievement());
    }

    public void ShowAchievementGained()
    {

        if (achievementsGainedType.Count <= 0 || counter >= achievementsGainedType.Count)
        {
            return;
        }
            StartCoroutine(GainAchievement());
    }

    IEnumerator GainAchievement()
    {
        int currentType = achievementsGainedType[counter];        
        switch(currentType)
        {
            case 2:
                if(!hasTempleAchiv)
                {
                    hasTempleAchiv  = true;
                    canvasAchievAnimation.Play("Achiev");
                    AchievementTextTitle.text = "O templo esquecido";
                    AchievementText.text = "Encontre o templo esquecido!";                    
                }
                break;
            case 3:
                if (!hasBaseAchiv)
                {
                    hasBaseAchiv = true;
                    canvasAchievAnimation.Play("Achiev");
                    AchievementTextTitle.text = "A pesquisa perdida!";
                    AchievementText.text = "Encontre a preciosa pesquisa perdida no fundo do mar!";
                }
                break;
            case 0:
                if (!hasTreasureAchiev)
                {
                    Debug.Log("acieve treasure");
                    hasTreasureAchiev = true;
                    canvasAchievAnimation.Play("Achiev");
                    AchievementTextTitle.text = "Caça ao tesouro!";
                    AchievementText.text = "Encontre um tesouro!";
                }
                break;
            case 5:
                if (!hasIceAchiv)
                {
                    hasIceAchiv = true;
                    canvasAchievAnimation.Play("Achiev");
                    AchievementTextTitle.text = "Segredos congelados!";
                    AchievementText.text = "Encontre o gelo com milhões de anos!";
                }
                break;

            case 4:
                if (!hasCoralAchiev)
                {
                    hasCoralAchiev = true;
                    canvasAchievAnimation.Play("Achiev");
                    AchievementTextTitle.text = "O coral ancestral";
                    AchievementText.text = "Encontre o coral ancestral! Dizem que tem poderes curativos!";
                }
                break;
            case 1:
                if (!hasShipAchiev)
                {
                    hasShipAchiev = true;
                    canvasAchievAnimation.Play("Achiev");
                    AchievementTextTitle.text = "O barco perdido";
                    AchievementText.text = "Encontre o barco perdido!";
                }
                break;
        }
        GetComponent<AudioSource>().Play();
        counter++;
        yield return new WaitForSeconds(2.5f);
        if (counter < achievementsGainedType.Count)
        {
            StartCoroutine(GainAchievement());
        }
        else
        {
            counter = 0;

            bool[] achievArray =
            {
                hasTreasureAchiev,
                hasCoralAchiev,
                hasBaseAchiv,
                hasTempleAchiv,
                hasShipAchiev,
                hasIceAchiv
            };

            database.SaveAchievements(PlayerDataScript.playerDataInstance.PlayerId, achievArray);
        }
    }

    /*public void CatchedAchievement(int index)
    {
        switch (index)
        {
            case 0:
                hasTreasure++;
                PlayerPrefs.SetInt("A", shipDepthModule);

                break;
            case 1:
                shipJewel++;
                PlayerPrefs.SetInt("TModule", shipTemperatureModule);
                playerHealth.UpdateModules();

                break;
            case 2:
                templeJewel++;
                PlayerPrefs.SetInt("RSModule", shipReelStrenghtModule);
                HarpoonTrigger.UpdateModule();
                break;
            case 3:
                lostResearch++;
                PlayerPrefs.SetInt("STModule", shipStorageModule);
                inventory.UpdateStorageSpace();
                break;
            case 4:
                ancientCoral++;
                PlayerPrefs.SetInt("AC", shipFlashlight);
                PlayerVisionController.UpdateFlashlight();
                break;
            case 5:
                millionYearIce++;
                PlayerPrefs.SetInt("AC", shipFlashlight);
                PlayerVisionController.UpdateFlashlight();
                break;
        }
    }*/
}
