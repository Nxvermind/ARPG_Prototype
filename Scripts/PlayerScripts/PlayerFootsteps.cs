using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioSource footstepSource;

    [SerializeField] private AudioClip[] footsteps; 

    public void FootSteps()
    {
        footstepSource.PlayOneShot(footsteps[0]);
    }

    public void FootStep_1()
    {
        footstepSource.PlayOneShot(footsteps[1]);
    }
}
