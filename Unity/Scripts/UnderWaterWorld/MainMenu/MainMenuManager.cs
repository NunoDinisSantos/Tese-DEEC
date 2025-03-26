using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

public class MainMenuManager : MonoBehaviour
{
    [HideInInspector] public int playerId;
    [HideInInspector] public GameObject[] panels;
    [HideInInspector] public TMP_InputField[] inputFields;
    [HideInInspector] public Button[] Buttons;
    [HideInInspector] public Slider loadingSlider;
    [HideInInspector] public DataBaseLoaderScript database;
    private bool startScene = false;

    void Start()
    {
        startScene = false;
        Buttons[0].interactable = false;
        Buttons[1].interactable = false;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (startScene)
        {
            startScene = false;
            StartGame();
        }
    }

    public void OpenPanels(int x)
    {
        panels[x].SetActive(true);       
    }

    public void ClosePanels(int x)
    {
        panels[x].SetActive(false);
    }

    public void QuiteGame()
    {
        Application.Quit();
    }

    public void AssignPlayerId(int x)
    {

        if (x == 0)
        {
            playerId = Convert.ToInt32(inputFields[x].text);
            Buttons[0].interactable = true;
        }

        if (x == 2) // Cria por ID
        {
            playerId = Convert.ToInt32(inputFields[x].text);
            Buttons[0].interactable = true;
        }
    }

    public void CallStartGame()
    {
        StartGame();
    }

    public void CallStartGameFromProxy(string id)
    {
        playerId = (int)Convert.ToInt64(id);
        startScene = true;
        //StartGame();
    }

    public async Task StartGame()
    {
        startScene = false;
        int level;
        await database.CallData(playerId.ToString());

        if (database.errorGettingPlayer == true)
        {
            return;
        }

        if(PlayerDataScript.playerDataInstance.Tutorial == 0) 
        {
            level = 1;
        }

        else
        {
            level = 2;
        }

        loadingSlider.gameObject.SetActive(true);
        StartCoroutine(LoadLevelAsync(level));
    }

    private IEnumerator LoadLevelAsync(int level)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(level);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
}
