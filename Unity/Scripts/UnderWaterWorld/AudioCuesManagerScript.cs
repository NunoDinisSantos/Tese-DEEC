using UnityEngine;

public class AudioCuesManagerScript : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip(int index)
    {
        audioSource.PlayOneShot(audioClips[index]);
    }
}
