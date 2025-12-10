using UnityEngine;

public class EnemyGotHitSFX : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        EventBus.OnEnemyHitEvent += PlaySFX;
    }

    private void OnDestroy()
    {
        EventBus.OnEnemyHitEvent -= PlaySFX;
    }

    private void PlaySFX()
    {
        audioSource.Play();
    }

}
