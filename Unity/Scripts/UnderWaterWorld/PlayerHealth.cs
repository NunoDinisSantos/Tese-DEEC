using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private bool takingDamageFromDepth = false;
    [SerializeField] private bool takingDamageFromCold = false;
    [SerializeField] private Collider col;
    [SerializeField] private int depthModule;
    [SerializeField] private int temperatureModule;
    [SerializeField] private PlayerProgress progress;
    [SerializeField] private int maxDepth;
    float timer = 3f;
    [SerializeField] float warnTimer = 0f;
    [SerializeField] private int Health = 100;
    [SerializeField] private TMP_Text Profundidade;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private AudioCuesManagerScript AudioWarnings;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Animation _animation;
    [SerializeField] private Animation _healthAnimation;
    [SerializeField] private GameObject PlayerMessageScript;
    [SerializeField] private bool takingDamageFromHot = false;
    [SerializeField] private AudioSource hotAudioSource;
    [SerializeField] private AudioSource coldAudioSource;
    [SerializeField] private GameObject coldWarningText;
    [SerializeField] private GameObject hotWarningText;
    [SerializeField] private bool animatingHearth = false;

    void Start()
    {
        BackToGameAppear();
        UpdateModules();
        healthText.text = Health.ToString();
    }

    private void Update()
    {
        int deepness = Mathf.RoundToInt(transform.position.y - 48) * -1;
        Profundidade.text = deepness.ToString() + "/" + maxDepth + " m";
        CheckIfShouldTakeDamage();
    }

    private void CheckIfShouldTakeDamage()
    {
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
                timer = 3 ;
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
        Health -= 5;
        timer = 3f;
        healthText.text = Health.ToString();

        if (Health < 0)
        {
            CallDeecSoS();
        }
    }

    private void CallDeecSoS()
    {
        PlayerMessageScript.SetActive(true);
        PlayerMessageScript.GetComponent<ShowPlayerMessageScript>().ShowMessage("Submarino avariado! A enviar ajuda!");
        StartCoroutine(Respawn());
    }

    public void JustRespawn()
    {
        PlayerMessageScript.SetActive(true);
        hotAudioSource.enabled = false;
        hotWarningText.SetActive(false);
        coldAudioSource.enabled = false;
        coldWarningText.SetActive(false);
        string line = "Mais um dia de trabalho! Boa!";
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
        yield return new WaitForSeconds(3);
        _animation.Play("FadeOut");
        GetComponent<PlayerMovementWater>().enabled = true;
        Health = 100;
        healthText.text = Health.ToString();
    }

    private void BackToGameAppear()
    {
        /*_animation.Play("FadeIn");
        GetComponent<PlayerMovementWater>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        transform.position = respawnPoint.position;
        yield return new WaitForSeconds(3);*/
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

            warnTimer = 3;
        }
    }
}
