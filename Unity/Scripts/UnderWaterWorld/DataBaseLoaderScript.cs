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
    //public string partialFileEndpoint = "http://88.198.115.159/"; // TO CHANGE
    //private string partialFileEndpoint = "https://localhost:44335/"; // TO CHANGE 
    private string partialFileEndpoint = "https://misteriosaquaticos.pt/";
    [HideInInspector] public PlayerData playerData;
    [HideInInspector] public bool loaded = false;
    public int maxDayStreak = 3;
    [HideInInspector] public bool errorGettingPlayer = false;
    [HideInInspector] private HttpClient _client;

    [SerializeField] private TMP_Text LBTexts;
    [SerializeField] private bool menu = false;
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
            playersLB = playersLB.OrderByDescending(p => p.fishCaught).Take(10).ToList();
            WriteLB(playersLB);
        }
    }

    private void WriteLB(List<PlayerData> playersList)
    {
        LBTexts.text = string.Empty;
        foreach (PlayerData playerData in playersList)
        {
            LBTexts.text += playerData.studentNick + " - " + playerData.fishCaught + " peixes" + "\n";
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

    #region OLD_PHP_WAY
    IEnumerator GetData(string playerId)
    {
        errorGettingPlayer = false;
        Debug.Log("Calling playerId: " + playerId);

        string fullEndpoint = partialFileEndpoint + "getPlayerData.php?player_id=" + playerId;
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
            playerData = JsonUtility.FromJson<PlayerData>(jsonResponse);
            PlayerDataScript.playerDataInstance.PlayerId = playerData.playerId;
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

    IEnumerator UpdateTutorial(string id)
    {
        string fullEndpoint = partialFileEndpoint + "updateTutorial.php";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);

        WWWForm form = new WWWForm();
        form.AddField("player_id", id);

        UnityWebRequest www = UnityWebRequest.Post(fullEndpoint, form);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating data: " + www.error);
        }
        else
        {
            // Success response (handle JSON if needed)
            Debug.Log("Update successful: " + www.downloadHandler.text);
        }
    }

    IEnumerator UpdateDays(string id, int days, int fishCaught, int money, int creditos)
    {
        string fullEndpoint = partialFileEndpoint + "updateDayFishMoney.php";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);

        WWWForm form = new WWWForm();

        form.AddField("player_id", id);
        form.AddField("days", days);
        form.AddField("fish_caught", fishCaught);
        form.AddField("coins", money);
        form.AddField("credits", creditos);

        UnityWebRequest www = UnityWebRequest.Post(fullEndpoint, form);

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

    IEnumerator UpdateModules(string id, int depth, int temp, int reel, int storage, int flashlight, int money)
    {
        string fullEndpoint = partialFileEndpoint + "updateModules.php";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);
        WWWForm form = new WWWForm();
        form.AddField("player_id", id);
        form.AddField("depth_module", depth);
        form.AddField("temp_module", temp);
        form.AddField("reel_module", reel);
        form.AddField("storage_module", storage);
        form.AddField("flashlight", flashlight);
        form.AddField("coins", money);

        UnityWebRequest www = UnityWebRequest.Post(fullEndpoint, form);

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

    IEnumerator SaveAchiev(string id, bool[] array)
    {
        string fullEndpoint = partialFileEndpoint + "updateAchievements.php";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);
        WWWForm form = new WWWForm();
        form.AddField("player_id", id);
        form.AddField("treasure", Convert.ToInt32(array[0]));
        form.AddField("ancient_coral", Convert.ToInt32(array[1]));
        form.AddField("lost_research", Convert.ToInt32(array[2]));
        form.AddField("temple_jewel", Convert.ToInt32(array[3]));
        form.AddField("boat_jewel", Convert.ToInt32(array[4]));
        form.AddField("old_ice", Convert.ToInt32(array[5]));

        UnityWebRequest www = UnityWebRequest.Post(fullEndpoint, form);

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
    #endregion

    #region Obsolete_VR

    /*
    public void CallSaveDataCircle(int id, string jsonData)
    {
        StartCoroutine(SaveMovement(id,jsonData));
    }
    */

    /*IEnumerator SaveMovement(int id, string jsonData)
    {
        string fullEndpoint = partialFileEndpoint + "saveCircleData.php";
        Debug.Log("Calling playerId: " + id);
        Debug.Log("Calling endpoint: " + fullEndpoint);
        WWWForm form = new();
        form.AddField("PlayerId", id);
        form.AddField("CircleData",id);

        UnityWebRequest www = UnityWebRequest.Post(fullEndpoint, form);

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
    */
    #endregion
}

[Serializable]
public class PlayersData
{
    public List<PlayerData> playersData;
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