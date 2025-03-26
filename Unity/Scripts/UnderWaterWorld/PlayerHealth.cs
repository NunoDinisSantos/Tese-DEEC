using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector][SerializeField] private bool takingDamageFromDepth = false;
    [HideInInspector][SerializeField] private bool takingDamageFromCold = false;
    [HideInInspector][SerializeField] private Collider col;
    [HideInInspector][SerializeField] private int depthModule;
    [HideInInspector][SerializeField] private int temperatureModule;
    [HideInInspector][SerializeField] private PlayerProgress progress;
    [HideInInspector][SerializeField] private int maxDepth;
    float timer = 1f;
    [HideInInspector][SerializeField] float warnTimer = 0f;
    [HideInInspector][SerializeField] private int Health = 100;
    [HideInInspector][SerializeField] private TMP_Text Profundidade;
    [HideInInspector][SerializeField] private TMP_Text healthText;
    [HideInInspector][SerializeField] private AudioCuesManagerScript AudioWarnings;
    [HideInInspector][SerializeField] private Transform respawnPoint;
    [HideInInspector][SerializeField] private Animation _animation;
    [HideInInspector][SerializeField] private Animation _healthAnimation;
    [HideInInspector][SerializeField] private GameObject PlayerMessageScript;
    [HideInInspector][SerializeField] private bool takingDamageFromHot = false;
    [HideInInspector][SerializeField] private AudioSource hotAudioSource;
    [HideInInspector][SerializeField] private AudioSource coldAudioSource;
    [HideInInspector][SerializeField] private GameObject coldWarningText;
    [HideInInspector][SerializeField] private GameObject hotWarningText;
    [HideInInspector][SerializeField] private bool animatingHearth = false;
    private PlayerMovementWater playerMovementWater;

    private bool forceNoDamage = false;
    private PlayerVisionController _playerVision;

    void Start()
    {
        playerMovementWater = GetComponent<PlayerMovementWater>();
        BackToGameAppear();
        UpdateModules();
        healthText.text = Health.ToString();
        _playerVision = GetComponent<PlayerVisionController>();
        _playerVision.SetWaterOriginalColor();
    }

    private void Update()
    {
        int deepness = Mathf.RoundToInt(transform.position.y - 48) * -1;
        Profundidade.text = deepness.ToString() + "/" + maxDepth + " m";
        CheckIfShouldTakeDamage();
    }

    private void CheckIfShouldTakeDamage()
    {
        if (forceNoDamage)
            return;

        if((transform.position.y - 48)*-1 > maxDepth) // 48 = water surface
        {
            takingDamageFromDepth = true;
            WarnPlayerDamage();
            //Audio a dizer que está a levar dano!
        }

        else
        {          
            takingDamageFromDepth = false;           
        }

        if (takingDamageFromCold || takingDamageFromDepth || takingDamageFromHot)
        {
            if (!animatingHearth)
            {
                animatingHearth = true;
                _healthAnimation.Play("TakeDamage");
            }

            timer -= Time.deltaTime;
            if(timer < 0)
            {
                DecrementHealth();
                timer = 1 ;
            }
        }

        else
        {
            if (animatingHearth)
            {
                _healthAnimation.Play("StopTakeDamage");
                animatingHearth = false;
            }
        }
    }

    private void DecrementHealth()
    {
        Health -= 7;
        timer = 1f;
        healthText.text = Health.ToString();
        if (Health < 0)
        {
            playerMovementWater.speed = 0.0f; // alterar para 10 depois
            forceNoDamage = true;
            healthText.text ="0";
            CallDeecSoS();
        }
    }

    private void CallDeecSoS()
    {
        PlayerMessageScript.SetActive(true);
        PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage("Submarino avariado! A enviar ajuda!");
        StartCoroutine("Respawn");
    }

    public void JustRespawn()
    {
        PlayerMessageScript.SetActive(true);
        hotAudioSource.enabled = false;
        hotWarningText.SetActive(false);
        coldAudioSource.enabled = false;
        coldWarningText.SetActive(false);
        string line = "Mais um dia de trabalho! Boa!";
        _playerVision.SetWaterOriginalColor();

        int i = Random.Range(0, 5);
        switch(i)
        {
            case 0:
                line = "Hoje há churrasco na sala de convívio! Vamos!";
                break;
            case 1:
                line = "Mais um dia de trabalho! Boa!";
                break;
            case 2:
                line = "Hoje foi cansativo! Merecemos descanso!";
                break;
            case 3:
                line = "Hoje é noite de jogos! Vamos até à sala de convívio!";
                break;
            case 4:
                line = "Todos os dias são bons dias!";
                break;
        }

        PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage(line);
        transform.position = respawnPoint.position;
        Health = 100;
        healthText.text = Health.ToString();
    }

    public void CallDEECPickUp()
    {
        StartCoroutine("RespawnPickUp");
    }

    private IEnumerator RespawnPickUp()
    {
        PlayerMessageScript.SetActive(true);
        string line = "Boleia a caminho! Vrum vrum!";
        PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage(line);

        hotAudioSource.enabled = false;
        hotWarningText.SetActive(false);
        coldAudioSource.enabled = false;
        coldWarningText.SetActive(false);
        _animation.Play("FadeIn");
        GetComponent<PlayerMovementWater>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        transform.position = respawnPoint.position;
        _playerVision.SetWaterOriginalColor();
        yield return new WaitForSeconds(3);
        _animation.Play("FadeOut");
        GetComponent<PlayerMovementWater>().enabled = true;
        Health = 100;
        healthText.text = Health.ToString();
        forceNoDamage = false;
        playerMovementWater.speed = 13.0f;
        StopCoroutine("RespawnPickUp");
    }

    private IEnumerator Respawn()
    {
        hotAudioSource.enabled = false;
        hotWarningText.SetActive(false);
        coldAudioSource.enabled = false;
        coldWarningText.SetActive(false);
        _animation.Play("FadeIn");
        GetComponent<PlayerMovementWater>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        transform.position = respawnPoint.position;
        _playerVision.SetWaterOriginalColor();
        yield return new WaitForSeconds(3);
        _animation.Play("FadeOut");
        GetComponent<PlayerMovementWater>().enabled = true;
        Health = 100;
        healthText.text = Health.ToString();
        forceNoDamage = false;
        playerMovementWater.speed = 13.0f; 
        StopCoroutine("Respawn");
    }

    private void BackToGameAppear()
    {
        _animation.Play("FadeOut");
        GetComponent<PlayerMovementWater>().enabled = true;
    }

    public void UpdateModules()
    {
        depthModule = progress.shipDepthModule;
        temperatureModule = progress.shipTemperatureModule;

        switch (depthModule)
        {
            case 0:
                maxDepth = 50;
                break;
            case 1:
                maxDepth = 120;
                break;
            case 2:
                maxDepth = 500; 
                break;
        }

        if(temperatureModule == 1)
        {
            coldAudioSource.gameObject.SetActive(false);
        }

        if (temperatureModule == 2)
        {
            hotAudioSource.gameObject.SetActive(false);
            coldAudioSource.gameObject.SetActive(false);
        }
    }

    public void EnterHotZone(int coldLevel)
    {
        if (temperatureModule >= coldLevel)
            return;

        hotAudioSource.enabled = true;
        hotWarningText.SetActive(true);
        AudioWarnings.PlayAudioClip(2);
        takingDamageFromCold = true;
        _animation.Play("FadeInFire"); 
    }

    public void LeaveHotZone()
    {
        if (takingDamageFromCold)
        {
            _animation.Play("FadeOutFire"); 
        }
        hotAudioSource.enabled = false;
        hotWarningText.SetActive(false);
        takingDamageFromCold = false;
    }

    public void EnterColdZone(int coldLevel)
    {
        if (temperatureModule >= coldLevel)
            return;

        coldAudioSource.enabled = true;
        coldWarningText.SetActive(true);
        AudioWarnings.PlayAudioClip(2);
        takingDamageFromCold = true;
        _animation.Play("FadeInFrost");
    }

    public void LeaveColdZone()
    {
        if(takingDamageFromCold)
        {
            _animation.Play("FadeOutFrost");
        }

        coldAudioSource.enabled = false;
        takingDamageFromCold = false;
        coldWarningText.SetActive(false);
    }

    private void WarnPlayerDamage()
    {
        warnTimer -= Time.deltaTime;
        if(warnTimer < 0)
        {
            if(takingDamageFromDepth)
            {
                AudioWarnings.PlayAudioClip(1);
            }

            warnTimer = timer;
        }
    }
}
