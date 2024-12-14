using System.Collections;
using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    public bool isPlayingMusic = false;
    private AudioSource audioSource;
    public AudioClip[] songs;
    public int timeUntilNewSong = 0;
    private float currentVolume = 0;
    [SerializeField] private float volumeFadeInOut = 0.5f;

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
            currentVolume = 0.35f;
        }

        yield return new WaitForSeconds(songLenght-3);
        isPlayingMusic = false;
        StartCoroutine(Rest());
    }

    private IEnumerator Rest()
    {
        audioSource.volume = 0.0f;      
        timeUntilNewSong = Random.Range(120, 240);
        yield return new WaitForSeconds(timeUntilNewSong);

        StartCoroutine(PlayMusic());
    }
}
