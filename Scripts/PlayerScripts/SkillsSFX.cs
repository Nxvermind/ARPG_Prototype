using UnityEngine;

public class SkillsSFX : MonoBehaviour
{
    [SerializeField] private AudioSource skillSfxAudioSource;

    public AudioClip skillDashClip;

    public void PlaySoundFX()
    {
        skillSfxAudioSource.PlayOneShot(skillDashClip);
    }
}
