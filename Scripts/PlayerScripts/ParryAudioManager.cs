using UnityEngine;

public class ParryAudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip audioClip; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        PlayerEvents.OnSuccessfulParryEvent += PlayParrySFX;
    }

    void OnDisable()
    {
        PlayerEvents.OnSuccessfulParryEvent -= PlayParrySFX;
    }

    private void PlayParrySFX()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
