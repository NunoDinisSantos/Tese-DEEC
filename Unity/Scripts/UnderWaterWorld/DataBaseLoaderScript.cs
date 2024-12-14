using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DataBaseLoaderScript : MonoBehaviour
{
    public string playerId;
    public string partialFileEndpoint = "http://localhost/Tese/"; // TO CHANGE
    public PlayerData playerData;
    public PlayerDataScript playerDataScript;
    public bool loaded = false;
    public int maxDayStreak = 3;
    public bool errorGettingPlayer = false;

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
            playerDataScript.PlayerId = playerData.player_id;
            playerDataScript.Coins = playerData.coins;
            playerDataScript.TimePlayed = playerData.time_played;
            playerDataScript.FishCaught = playerData.fish_caught;
            playerDataScript.Tutorial = playerData.tutorial;
            playerDataScript.Flashlight = playerData.flashlight;
            playerDataScript.DepthModule = playerData.depth_module;
            playerDataScript.StorageModule = playerData.storage_module;
            playerDataScript.ReelModule = playerData.reel_module;
            playerDataScript.TempModule = playerData.temp_module;
            playerDataScript.Days = playerData.days;
            playerDataScript.Credits = playerData.credits;
            playerDataScript.LastLogin = playerData.last_login;

            playerDataScript.Treasure = playerData.treasure;
            playerDataScript.AncientCoral = playerData.ancient_coral;
            playerDataScript.LostResearch = playerData.lost_research;
            playerDataScript.BoatJewel = playerData.boat_jewel;
            playerDataScript.TempleJewel = playerData.temple_jewel;
            playerDataScript.OldIce = playerData.old_ice;

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

    public void CallUpdateModules(int depth, int temp, int reel, int storage, int flashlight, int money)
    {
        StartCoroutine(UpdateModules(PlayerDataScript.playerDataInstance.PlayerId, depth, temp, reel, storage, flashlight, money));
    }

    public void CallUpdateDays(int days, int fishCaught, int money, int creditos)
    {
        StartCoroutine(UpdateDays(PlayerDataScript.playerDataInstance.PlayerId, days, fishCaught, money, creditos));
    }

    public void CallUpdateTutorial()
    {
        StartCoroutine(UpdateTutorial(PlayerDataScript.playerDataInstance.PlayerId));
    }

    public async Task CallData(string playerId)
    {
        await CoRoutineAwaiterWrapper.AsTask(GetData(playerId), this);
    }

    public void SaveAchievements(string id, bool[] array)
    {
        StartCoroutine(SaveAchiev(id, array));
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
                StartCoroutine(UpdateDayStreak(playerDataScript.PlayerId, playerDataScript.DayStreak, DateTime.Now));
                return;
            }

            else
            {
                playerDataScript.LastLogin = DateTime.Now;
                playerDataScript.DayStreak++;
                StartCoroutine(UpdateDayStreak(playerDataScript.PlayerId, playerDataScript.DayStreak, DateTime.Now));
            }
        }

        else
        {
            playerDataScript.DayStreak = 0;
            StartCoroutine(UpdateDayStreak(playerDataScript.PlayerId, playerDataScript.DayStreak, DateTime.Now));
        }
    }

    IEnumerator UpdateDayStreak(string playerId, int dayStreak, DateTime newDate)
    {
        string fullEndpoint = partialFileEndpoint + "updateDayStreak.php";
        Debug.Log("Calling playerId: " + playerId);
        Debug.Log("Calling endpoint: " + fullEndpoint);
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);
        form.AddField("days_streak", dayStreak);
        form.AddField("last_login", newDate.ToString("yyyy-MM-dd HH:mm:ss"));

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
    public string player_id;     
    public int coins;            
    public int time_played;      
    public int fish_caught;      
    public int tutorial;        
    public int flashlight;       
    public int depth_module;    
    public int storage_module;  
    public int reel_module;    
    public int temp_module;      
    public int days;             
    public float multiplier;   
    public int days_streak;    
    public int credits;      
    public DateTime last_login;   

    public bool treasure;    
    public bool ancient_coral;  
    public bool lost_research;  
    public bool temple_jewel; 
    public bool boat_jewel;      
    public bool old_ice;          
}