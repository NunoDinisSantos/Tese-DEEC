using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public List<string> DialogStrings = new();
    public List<string> DialogStrings2 = new();

    public TMP_Text textComponent;
    public float textSpeed = 0.01f;
    public GameObject ButaoCutscene;
    //public GameObject[] CanvasIconsNText;

    [SerializeField] private bool canPressAgain = true;
    [SerializeField] private bool wait = false;
    [SerializeField] private Animator _animatorManPresentor;
    private Animator _animator;
    [SerializeField] private int index = 0;
    [SerializeField] private int currentLimit = -1;
    [SerializeField] private int currentMaxLimit = 3;
    [SerializeField] private float animationSpeed = 1f;
    public bool stop = false;
    public Animation cameraFadeAnim;
    public GameObject player;
    public Transform spawnPosition;

    public GameObject SpeechObject;
    public Slider loadingSlider;
    [SerializeField] private Material skyboxMat;
    [SerializeField] private Color startColorBot;
    public Color dayColor;
    public DataBaseLoaderScript database;

    void Start()
    {
        skyboxMat.SetColor("_BotColor", dayColor);
        skyboxMat.SetFloat("_StarAmount", 2);
        skyboxMat.SetColor("_AlphaColor", Color.black);
        loadingSlider.gameObject.SetActive(false);
        SpeechObject.SetActive(true);
        _animator = GetComponent<Animator>();
        DialogStrings.AddRange(DialogStrings2);
        ButaoCutscene.SetActive(false);
        textComponent.text = string.Empty;
        canPressAgain = false;
        NextLine();
    }

    void Update()
    {
        CheckAnimStop();
        if (Input.GetMouseButton(0) && canPressAgain) // alterar para qualquer butao no joystick
        {
            canPressAgain = false;
            ButaoCutscene.SetActive(false);
            NextLine();
        }
    }

    public void ClickedButtonProxy()
    {
        if (canPressAgain)
        {
            canPressAgain = false;
            ButaoCutscene.SetActive(false);
            NextLine();
        }
    }

    void NextLine()
    {
        if (index < DialogStrings.Count-1)
        {
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            FinishTutorial();
        }
    }

    private void FinishTutorial()
    {
        StartCoroutine(FinishTutorialAndStartGame());
    }

    IEnumerator FinishTutorialAndStartGame()
    {
        database.CallUpdateTutorial();

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(2);
        loadingSlider.gameObject.SetActive(true);
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }


    IEnumerator TypeLine()
    {
        if (index == 9)
        {
            _animatorManPresentor.Play("Jump");
        }

        if (index == 16)
        {
            _animatorManPresentor.Play("Idle2");
        }

        if(index == 22)
        {
            _animator.SetBool("show",true);
            _animatorManPresentor.SetBool("Walking", true);
        }

        if (index == 24)
        {
            ContinueMan();
            StartManWalking();
        }

        if (index == 26)
        {
            ContinueMan();
            StartManWalking();
        }

        if (index == 28)
        {
            ContinueMan();
            StartManWalking();
        }

        foreach (char c in DialogStrings[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        index++;

        if (!(index < currentLimit))
        {
            canPressAgain = true;
            ButaoCutscene.SetActive(true);
        }
    }

    public void CheckIfAnimationShouldStop(int minLimit)
    {
        currentLimit = minLimit;
    }

    public void NewMaxLimit(int maxLimit)
    {
        if (index <= currentMaxLimit)
        {
            canPressAgain = true;
            ButaoCutscene.SetActive(true);
            currentMaxLimit = maxLimit;
        }
    }

    public void SkipTutorial()
    {
        FinishTutorial();
    }

    private void CheckAnimStop()
    {
        if(stop)
        {
            _animator.speed = 0;
        }

        if (index <= currentLimit)
        {
            _animator.speed = 0;
        }

        else
        {
            if(!stop)
                _animator.speed = animationSpeed;
        }

        if (index == currentMaxLimit)
        {
            canPressAgain = false;
            ButaoCutscene.SetActive(false);
        }
    }

    private void ContinueMan()
    {
        stop = false;
    }

    public void StopMan(int x)
    {
        if (x == 0)
        {
            stop = false;
        }
        else
        {
            stop = true;
        }
    }

    public void StartManAnimation()
    {
        _animatorManPresentor.SetBool("ChegouChao",true);       
    }

    public void StartManIdle()
    {
        _animatorManPresentor.SetBool("Stairs", true);
        _animatorManPresentor.SetBool("Walking", false);
    }

    public void StartManWalking()
    {
        _animatorManPresentor.SetBool("Stairs", false);
        _animatorManPresentor.SetBool("Walking", true);
    }
}
