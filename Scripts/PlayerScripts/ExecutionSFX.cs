using UnityEngine;

public class ExecutionSFX : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;

    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventBus.OnExecutionStarted += PlaySFX;
    }

    private void OnDisable()
    {
        EventBus.OnExecutionStarted -= PlaySFX;
    }

    private void PlaySFX()
    {
        int rnd = Random.Range(0, clips.Length);

        audioSource.PlayOneShot(clips[rnd]);
    }

}
