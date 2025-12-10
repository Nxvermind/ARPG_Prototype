using UnityEngine;

public class SlashSoundFX : MonoBehaviour
{
    [SerializeField] private AudioSource slashSoundFXAudioSource;

    private ICurrentAttackNodeProvider attackProvider;

    public void Initialize(ICurrentAttackNodeProvider _attackProvider)
    {
        attackProvider = _attackProvider;
    }

    public void PlaySFX()
    {
        if(attackProvider.CurrentAttackNode != null)
        {
            slashSoundFXAudioSource.PlayOneShot(attackProvider.CurrentAttackNode.attackSoundFX);
        }
    }
}
