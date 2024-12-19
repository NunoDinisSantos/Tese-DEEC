using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DataBaseLoaderScript : MonoBehaviour
{
    public string playerId;
    //public string partialFileEndpoint = "http://localhost/Tese/"; // TO CHANGE
    private string partialFileEndpoint = "https://localhost:44335/"; // TO CHANGE
    public PlayerData playerData;
    public PlayerDataScript playerDataScript;
    public bool loaded = false;
    public int maxDayStreak = 3;
    public bool errorGettingPlayer = false;

    IEnumerator GetDataFromApi(string playerId)
    {
        errorGettingPlayer = false;
        Debug.Log("Calling playerId: " + playerId);

        string fullEndpoint = partialFileEndpoint + "api/misteriosaquaticos/" + playerId;
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
            playerDataScript.PlayerId = playerData.playerId;
            playerDataScript.Coins = playerData.coins;
            playerDataScript.TimePlayed = playerData.timePlayed;
            playerDataScript.FishCaught = playerData.fishCaught;
            playerDataScript.Tutorial = playerData.tutorial;
            playerDataScript.Flashlight = playerData.flashlight;
            playerDataScript.DepthModule = playerData.depthModule;
            playerDataScript.StorageModule = playerData.storageModule;
            playerDataScript.ReelModule = playerData.reelModule;
            playerDataScript.TempModule = playerData.tempModule;
            playerDataScript.Days = playerData.days;
            playerDataScript.Credits = playerData.credits;
            playerDataScript.LastLogin = DateTime.Parse(playerData.lastLogin);

            playerDataScript.Treasure = playerData.treasure;
            playerDataScript.AncientCoral = playerData.ancientCoral;
            playerDataScript.LostResearch = playerData.lostResearch;
            playerDataScript.BoatJewel = playerData.boatJewel;
            playerDataScript.TempleJewel = playerData.templeJewel;
            playerDataScript.OldIce = playerData.oldIce;

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
        if(DateTime.Now.Day - playerDataScript.LastLogin.Day == 1 &&
            DateTime.Now.Month - playerDataScript.LastLogin.Month == 0 &&
            DateTime.Now.Year - playerDataScript.LastLogin.Year == 0)
        {
            if (playerDataScript.DayStreak >= 3)
            {
                playerDataScript.LastLogin = DateTime.Now;
                StartCoroutine(UpdateDayStreakFromApi(playerDataScript.PlayerId, playerDataScript.DayStreak, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                return;
            }

            else
            {
                playerDataScript.LastLogin = DateTime.Now;
                playerDataScript.DayStreak++;
                StartCoroutine(UpdateDayStreakFromApi(playerDataScript.PlayerId, playerDataScript.DayStreak, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
        }

        else
        {
            if (DateTime.Now.Year - playerDataScript.LastLogin.Year > 0 || DateTime.Now.Month - playerDataScript.LastLogin.Month > 0 || DateTime.Now.Day - playerDataScript.LastLogin.Day > 1)
            {
                playerDataScript.DayStreak = 0;
            }

            StartCoroutine(UpdateDayStreakFromApi(playerDataScript.PlayerId, playerDataScript.DayStreak, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
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
            playerDataScript.PlayerId = playerData.playerId;
            playerDataScript.Coins = playerData.coins;
            playerDataScript.TimePlayed = playerData.timePlayed;
            playerDataScript.FishCaught = playerData.fishCaught;
            playerDataScript.Tutorial = playerData.tutorial;
            playerDataScript.Flashlight = playerData.flashlight;
            playerDataScript.DepthModule = playerData.depthModule;
            playerDataScript.StorageModule = playerData.storageModule;
            playerDataScript.ReelModule = playerData.reelModule;
            playerDataScript.TempModule = playerData.tempModule;
            playerDataScript.Days = playerData.days;
            playerDataScript.Credits = playerData.credits;
            playerDataScript.LastLogin = DateTime.Parse(playerData.lastLogin);

            playerDataScript.Treasure = playerData.treasure;
            playerDataScript.AncientCoral = playerData.ancientCoral;
            playerDataScript.LostResearch = playerData.lostResearch;
            playerDataScript.BoatJewel = playerData.boatJewel;
            playerDataScript.TempleJewel = playerData.templeJewel;
            playerDataScript.OldIce = playerData.oldIce;

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

[System.Serializable] // torna visivel no inspector
public class PlayerData
{
    public string playerId;     
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