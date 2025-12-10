using System.Collections;
using UnityEngine;

public class SkillOnCooldownSFX : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip[] clips;

    public static SkillOnCooldownSFX instance;

    private bool b;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX()
    {
        if (b) return;

        StartCoroutine(PlaySFX_Cor());
    }

    IEnumerator PlaySFX_Cor()
    {
        b= true;

        float rnd = Random.value;

        if(rnd > 0.6f)
        {
            audioSource.PlayOneShot(clips[0]);
        }
        else
        {
            audioSource.PlayOneShot(clips[1]);
        }

        yield return new WaitForSecondsRealtime(2);

        b = false;

    }
}
