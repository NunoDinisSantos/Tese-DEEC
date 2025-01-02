using System.Collections;
using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    [HideInInspector][SerializeField] private bool isPlayingMusic = false;
    private AudioSource audioSource;
    [SerializeField] private float musicVolume = 0.1f;
    [SerializeField] private AudioClip[] songs;
    [HideInInspector][SerializeField] private int timeUntilNewSong = 0;
    [HideInInspector][SerializeField] private int maxWaitingTime = 240;
    [HideInInspector][SerializeField] private int minWaitingTime = 120;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(Rest());
    }

    private IEnumerator PlayMusic()
    {
        float songLenght = 0;

        if (!isPlayingMusic)
        {
            int i = Random.Range(0, songs.Length);
            audioSource.clip = songs[i];
            audioSource.Play();
            songLenght = songs[i].length;
            audioSource.volume = musicVolume;
            isPlayingMusic = true;
        }

        yield return new WaitForSeconds(songLenght-3);

        float timer = 3f;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            audioSource.volume = audioSource.volume-audioSource.volume*0.001f;
        }


        isPlayingMusic = false;
        StartCoroutine(Rest());
    }

    private IEnumerator Rest()
    {
        audioSource.volume = 0.0f;      
        timeUntilNewSong = Random.Range(minWaitingTime, maxWaitingTime);
        yield return new WaitForSeconds(timeUntilNewSong);

        StartCoroutine(PlayMusic());
    }
}