using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class DataBaseLoaderScript : MonoBehaviour
{
    [HideInInspector] public string playerId;
    //private string partialFileEndpoint = "https://localhost:44335/"; // TO CHANGE 
    private string partialFileEndpoint = "https://misteriosaquaticos.pt/";
    [HideInInspector] public PlayerData playerData;
    [HideInInspector] public bool loaded = false;
    public int maxDayStreak = 3;
    [HideInInspector] public bool errorGettingPlayer = false;
    [HideInInspector] private HttpClient _client;

    [SerializeField] private TMP_Text LBTexts;
    [SerializeField] private bool menu = false;

    public TMP_Text[] RewardsNames;
    public TMP_Text[] RewardsPrices;

    public TMP_Text ChallengeEndDate;
    public TMP_Text ChallengeStartDate;
    public TMP_Text ChallengeDescription;
    public TMP_Text Desafio;

    public TMP_Text LastWinnerText;
    public TMP_Text RunnerUps;


    public GameObject[] DesafioPanels;

    private void Start()
    {
        if (menu)
        {
            StartCoroutine("GetPlayersForLB");
        }
    }

    private IEnumerator GetPlayersForLB()
    {
        var playersLB = new List<PlayerData>();
        int eventType = -1;
        string fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/";
        Debug.Log("Calling endpoint: " + fullEndpoint);

        UnityWebRequest data = UnityWebRequest.Get(fullEndpoint);
        yield return data.SendWebRequest();

        string jsonResponse = data.downloadHandler.text;

        if (data.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching data: " + data.error);
            errorGettingPlayer = true;
        }
        else
        {
            PlayerData[] playersArray = JsonHelper.FromJson<PlayerData>(jsonResponse);
            playersLB = playersArray.ToList();
            playersLB = playersLB.OrderByDescending(p => p.fishCaught).Take(5).ToList();
            WriteLB(playersLB);
        }

        // REWARDS NO LONGER WILL BE AVAILABLE! (CHALLENGES IS BETTER)
        /*
        var rewards = new List<Reward>();

        fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/rewards";
        Debug.Log("Calling endpoint: " + fullEndpoint);

        data = UnityWebRequest.Get(fullEndpoint);
        yield return data.SendWebRequest();

        jsonResponse = data.downloadHandler.text;

        if (data.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching data: " + data.error);
            errorGettingPlayer = true;
        }
        else
        {
            Reward[] rewardArray = JsonHelper.FromJson<Reward>(jsonResponse);
            rewards = rewardArray.ToList();


            for (int i = 0; i < rewards.Count; i++)
            {
                RewardsPrices[i].text = rewards[i].price.ToString();
                RewardsNames[i].text = rewards[i].name;
            }
        }
        */

        DesafioPanels[0].SetActive(true);
        DesafioPanels[1].SetActive(false);

        fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/challenges/latest";
        Debug.Log("Calling endpoint: " + fullEndpoint);

        data = UnityWebRequest.Get(fullEndpoint);
        yield return data.SendWebRequest();

        jsonResponse = data.downloadHandler.text;

        if (data.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching data: " + data.error);
            errorGettingPlayer = true;
        }
        else
        {
            ChallengeDTO challenge = JsonUtility.FromJson<ChallengeDTO>(jsonResponse);

            if (challenge.description == string.Empty)
            {
                ChallengeStartDate.text = "";
                ChallengeEndDate.text = "";
                Desafio.text = "Challenge coming soon!";
                DesafioPanels[1].GetComponent<TMP_Text>().text = string.Empty;
                DesafioPanels[1].SetActive(false);
            }

            else
            {
                ChallengeDescription.text = challenge.description;
                ChallengeEndDate.text = challenge.endDate;
                ChallengeStartDate.text = challenge.startDate;
                eventType = challenge.eventType;
                DesafioPanels[1].GetComponent<TMP_Text>().text = "to";
                DesafioPanels[1].SetActive(true);
            }
        }

        Debug.Log("Event type is: "+eventType);

        LastWinnerText.text = string.Empty;

        fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/lastChallengeWinners";
        Debug.Log("Calling endpoint: " + fullEndpoint);

        data = UnityWebRequest.Get(fullEndpoint);
        yield return data.SendWebRequest();

        jsonResponse = data.downloadHandler.text;

        if (data.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching data: " + data.error);
            errorGettingPlayer = true;
        }
        else
        {
            ChallengeProgressDataDTO lastWinner = JsonUtility.FromJson<ChallengeProgressDataDTO>(jsonResponse);

            if (lastWinner.nick_Name == string.Empty)
            {
                LastWinnerText.text = "NO WINNER";
            }

            else
            {
                LastWinnerText.text = lastWinner.nick_Name;
            }
        }

        if (eventType >= 0)
        {
            RunnerUps.text = string.Empty;

            fullEndpoint = partialFileEndpoint + $"api/misteriosaquaticos/challengeProgressEvent/{eventType}";
            Debug.Log("Calling endpoint: " + fullEndpoint);

            data = UnityWebRequest.Get(fullEndpoint);
            yield return data.SendWebRequest();

            jsonResponse = data.downloadHandler.text;

            if (data.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching data: " + data.error);
                errorGettingPlayer = true;
            }
            else
            {
                var runnerUps = new List<ChallengeProgressDataDTO>();

                ChallengeProgressDataDTO[] runnerUpsArray = JsonHelper.FromJson<ChallengeProgressDataDTO>(jsonResponse);
                runnerUps = runnerUpsArray.ToList();

                WriteRunnerUps(runnerUps);
            }
        }

        else
        {
            RunnerUps.text = "";
        }

        fullEndpoint = partialFileEndpoint + $"api/misteriosaquaticos/checkwin";
        Debug.Log("Calling endpoint: " + fullEndpoint);

        data = UnityWebRequest.Get(fullEndpoint);
        yield return data.SendWebRequest();

        //Debug.Log(data.result);
        //if(data.result != UnityWebRequest.Result.Success)

        yield return new WaitForSeconds(15);

        StartCoroutine("GetPlayersForLB");
    }

    private void WriteLB(List<PlayerData> playersList)
    {
        LBTexts.text = string.Empty;
        foreach (PlayerData playerData in playersList)
        {
            LBTexts.text += playerData.studentNick + " - " + playerData.fishCaught + " fish" + "\n";
        }
    }

    private void WriteRunnerUps(List<ChallengeProgressDataDTO> runnerUps)
    {
        RunnerUps.text = string.Empty;
        foreach (ChallengeProgressDataDTO runnerUpPlayer in runnerUps)
        {
            RunnerUps.text += runnerUpPlayer.nick_Name + " | " + runnerUpPlayer.coins + " coins " + "| " + runnerUpPlayer.credits + " credits" + " | " + runnerUpPlayer.fishCaught + " fish" +"\n";
        }
    }
    IEnumerator UpdatePlayerProgressFromAPi(ChallengeProgressData progress)
    {
        var fullEndpoint = "https://misteriosaquaticos.pt/" + $"api/misteriosaquaticos/challengeProgress/player";

        Debug.Log("Calling endpoint: " + fullEndpoint);


        var progressDTO = new ChallengeProgressDataDTO()
        {
            coins = progress.coins,
            credits = progress.credits,
            fishCaught = progress.fishCaught,
            nick_Name = progress.nick_Name,
            playerId = progress.playerId,
        };

        string jsonBody = JsonUtility.ToJson(progressDTO);

        UnityWebRequest www = new UnityWebRequest(fullEndpoint, UnityWebRequest.kHttpVerbPUT);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        progressDTO = new ChallengeProgressDataDTO()
        {
            coins = 0,
            credits = 0,
            fishCaught = 0,
        };

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating data: " + www.error);
        }
        else
        {
            Debug.Log("Update successful: " + www.downloadHandler.text);
        }
    }


    IEnumerator GetDataFromApi(string playerId)
    {
        errorGettingPlayer = false;
        Debug.Log("Calling playerId: " + playerId);

        string fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/" + playerId;
        Debug.Log("Calling endpoint: " + fullEndpoint);

        UnityWebRequest data = UnityWebRequest.Get(fullEndpoint);

        yield return data.SendWebRequest();

        string jsonResponse = data.downloadHandler.text;
        Debug.Log(jsonResponse);
        if (data.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching data: " + data.error);
            errorGettingPlayer = true;
        }
        else 
        {
            playerData = JsonUtility.FromJson<PlayerData>(jsonResponse);
            PlayerDataScript.playerDataInstance.PlayerId = playerData.playerId;
            PlayerDataScript.playerDataInstance.StudentNick = playerData.studentNick;
            PlayerDataScript.playerDataInstance.Coins = playerData.coins;
            PlayerDataScript.playerDataInstance.TimePlayed = playerData.timePlayed;
            PlayerDataScript.playerDataInstance.FishCaught = playerData.fishCaught;
            PlayerDataScript.playerDataInstance.Tutorial = playerData.tutorial;
            PlayerDataScript.playerDataInstance.Flashlight = playerData.flashlight;
            PlayerDataScript.playerDataInstance.DepthModule = playerData.depthModule;
            PlayerDataScript.playerDataInstance.StorageModule = playerData.storageModule;
            PlayerDataScript.playerDataInstance.ReelModule = playerData.reelModule;
            PlayerDataScript.playerDataInstance.TempModule = playerData.tempModule;
            PlayerDataScript.playerDataInstance.Days = playerData.days;
            PlayerDataScript.playerDataInstance.Credits = playerData.credits;
            PlayerDataScript.playerDataInstance.LastLogin = DateTime.Parse(playerData.lastLogin);

            PlayerDataScript.playerDataInstance.Treasure = playerData.treasure;
            PlayerDataScript.playerDataInstance.AncientCoral = playerData.ancientCoral;
            PlayerDataScript.playerDataInstance.LostResearch = playerData.lostResearch;
            PlayerDataScript.playerDataInstance.BoatJewel = playerData.boatJewel;
            PlayerDataScript.playerDataInstance.TempleJewel = playerData.templeJewel;
            PlayerDataScript.playerDataInstance.OldIce = playerData.oldIce;

            HandleDayStreak();
        }
    }

    IEnumerator UpdateTutorialFromApi(string id)
    {
        string fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/"+id+"/tutorial";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);

        UnityWebRequest www = new UnityWebRequest(fullEndpoint, UnityWebRequest.kHttpVerbPUT);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes($"\"{id}\"");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating data: " + www.error);
        }
        else
        {
            Debug.Log("Update successful: " + www.downloadHandler.text);
        }
    }

    IEnumerator UpdateDaysFromApi(string id, int days, int fishCaught, int money, int creditos, TimeSpan timePlayed)
    {
        string fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/" + id + "/day";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);

        var dayResponse = new DayData()
        {
            days = days,
            coins = money,
            fishCaught = fishCaught,
            credits = creditos,
            timePlayed = (int)timePlayed.TotalSeconds
        };

        string jsonBody = JsonUtility.ToJson(dayResponse);

        UnityWebRequest www = new UnityWebRequest(fullEndpoint, UnityWebRequest.kHttpVerbPUT);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");


        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating data: " + www.error);
        }
        else
        {
            Debug.Log("Update successful: " + www.downloadHandler.text);
        }
    }

    IEnumerator UpdateModulesFromApi(string id, int depth, int temp, int reel, int storage, int flashlight, int money)
    {
        string fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/" + id + "/modules";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);

        var moduleResponse = new ModuleData()
        {
            depthModule = depth,
            reelModule = reel,
            storageModule = storage,
            flashlight = flashlight,
            tempModule = temp,
            coins = money
        };

        string jsonBody = JsonUtility.ToJson(moduleResponse);

        UnityWebRequest www = new UnityWebRequest(fullEndpoint, UnityWebRequest.kHttpVerbPUT);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating data: " + www.error);
        }
        else
        {
            Debug.Log("Update successful: " + www.downloadHandler.text);
        }
    }

    IEnumerator UpdateAchievFromApi(string id, bool[] array)
    {
        string fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/" + id + "/achievements";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);

        var achievData = new AchievementData()
        {
            treasure = array[0],
            ancientCoral = array[1],
            lostResearch = array[2],
            templeJewel = array[3],
            boatJewel = array[4],
            oldIce = array[5],
        };

        string jsonBody = JsonUtility.ToJson(achievData);

        UnityWebRequest www = new UnityWebRequest(fullEndpoint, UnityWebRequest.kHttpVerbPUT);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating data: " + www.error);
        }
        else
        {
            Debug.Log("Update successful: " + www.downloadHandler.text);
        }
    }

    IEnumerator UpdateDayStreakFromApi(string id, int dayStreak, string newDate)
    {
        string fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/" + id + "/daystreak";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);

        var dayStreakData = new DayStreakData()
        {
            lastLogin = newDate,
            daysStreak = dayStreak,
        };

        string jsonBody = JsonUtility.ToJson(dayStreakData);

        UnityWebRequest www = new UnityWebRequest(fullEndpoint, UnityWebRequest.kHttpVerbPUT);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating data: " + www.error);
        }
        else
        {
            Debug.Log("Update successful: " + www.downloadHandler.text);
        }
    }

    public void CallUpdateModules(int depth, int temp, int reel, int storage, int flashlight, int money)
    {
        StartCoroutine(UpdateModulesFromApi(PlayerDataScript.playerDataInstance.PlayerId, depth, temp, reel, storage, flashlight, money));
    }

    public void CallUpdateDays(int days, int fishCaught, int money, int creditos, TimeSpan timePlayed)
    {
        StartCoroutine(UpdateDaysFromApi(PlayerDataScript.playerDataInstance.PlayerId, days, fishCaught, money, creditos, timePlayed));
    }

    public void CallUpdateTutorial()
    {
        StartCoroutine(UpdateTutorialFromApi(PlayerDataScript.playerDataInstance.PlayerId));
    }

    public async Task CallData(string playerId)
    {
        await CoRoutineAwaiterWrapper.AsTask(GetDataFromApi(playerId), this);
    }

    public void SaveAchievements(string id, bool[] array)
    {
        StartCoroutine(UpdateAchievFromApi(id, array));
    }

    public void UpdatePlayerProgress(ChallengeProgressData progress)
    {
        StartCoroutine(UpdatePlayerProgressFromAPi(progress));
    }

    private void HandleDayStreak()
    {
        if(DateTime.Now.Day - PlayerDataScript.playerDataInstance.LastLogin.Day == 1 &&
            DateTime.Now.Month - PlayerDataScript.playerDataInstance.LastLogin.Month == 0 &&
            DateTime.Now.Year - PlayerDataScript.playerDataInstance.LastLogin.Year == 0)
        {
            if (PlayerDataScript.playerDataInstance.DayStreak >= 3)
            {
                PlayerDataScript.playerDataInstance.LastLogin = DateTime.Now;
                StartCoroutine(UpdateDayStreakFromApi(PlayerDataScript.playerDataInstance.PlayerId, PlayerDataScript.playerDataInstance.DayStreak, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                return;
            }

            else
            {
                PlayerDataScript.playerDataInstance.LastLogin = DateTime.Now;
                PlayerDataScript.playerDataInstance.DayStreak++;
                StartCoroutine(UpdateDayStreakFromApi(PlayerDataScript.playerDataInstance.PlayerId, PlayerDataScript.playerDataInstance.DayStreak, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
        }

        else
        {
            if (DateTime.Now.Year - PlayerDataScript.playerDataInstance.LastLogin.Year > 0 || DateTime.Now.Month - PlayerDataScript.playerDataInstance.LastLogin.Month > 0 || DateTime.Now.Day - PlayerDataScript.playerDataInstance.LastLogin.Day > 1)
            {
                PlayerDataScript.playerDataInstance.DayStreak = 0;
            }

            StartCoroutine(UpdateDayStreakFromApi(PlayerDataScript.playerDataInstance.PlayerId, PlayerDataScript.playerDataInstance.DayStreak, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }
}

[System.Serializable] // torna visivel no inspector
public class ChallengeProgressData
{
    public string playerId { get; set; }
    public string nick_Name { get; set; } = string.Empty;
    public int coins { get; set; }
    public int fishCaught { get; set; }
    public int credits { get; set; }
}

[Serializable]
public class PlayersData
{
    public List<PlayerData> playersData;
}

[System.Serializable] // torna visivel no inspector
public class ChallengeWinnerData
{
    public int Id { get; set; }
    public string Player_Id { get; set; } = string.Empty;
    public int ChallengeId { get; set; }
    public string Nick_Name { get; set; } = string.Empty;
}

[System.Serializable] // torna visivel no inspector
public class PlayerData
{
    public string playerId;
    public string studentNick;
    public int coins;            
    public int timePlayed;      
    public int fishCaught;      
    public int tutorial;        
    public int flashlight;       
    public int depthModule;    
    public int storageModule;  
    public int reelModule;    
    public int tempModule;      
    public int days;             
    public float multiplier;   
    public int daysStreak;    
    public int credits;      
    public string lastLogin;   

    public bool treasure;    
    public bool ancientCoral;  
    public bool lostResearch;  
    public bool templeJewel; 
    public bool boatJewel;      
    public bool oldIce;          
}

[System.Serializable] // torna visivel no inspector
public class Reward
{
    public int id;
    public string name;
    public int price;
}

[System.Serializable] // torna visivel no inspector
public class Challenge
{
    public int id;
    public string description;
    public string startDate;
    public string endDate;
    public bool started;
    public bool ended;
    public int eventType;
    public int quantityx;
    public int quantityy;
    public int quantityz;
}

public class ChallengeDTO
{
    public int id;
    public string description;
    public string startDate;
    public string endDate;
    public bool started;
    public bool ended;
    public int eventType;
    public int quantityx;
    public int quantityy;
    public int quantityz;
}


public class AchievementData
{
    public bool treasure;
    public bool ancientCoral;
    public bool lostResearch;
    public bool templeJewel;
    public bool boatJewel;
    public bool oldIce;
}

public class DayStreakData
{
    public string lastLogin;
    public int daysStreak;
}

public class DayData
{
    public int days;
    public int credits;
    public int fishCaught;
    public int timePlayed;
    public int coins;
}

public class ModuleData
{
    public int coins;
    public int flashlight;
    public int depthModule;
    public int storageModule;
    public int reelModule;
    public int tempModule;
}

[System.Serializable]
public class ChallengeProgressDataDTO
{
    public string playerId;
    public string nick_Name;
    public int coins;
    public int fishCaught;
    public int credits;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = "{ \"items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}