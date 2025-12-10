using UnityEngine;

public class EnemyRootMotion : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();

    }

    public void ActivateRootMotion()
    {
        anim.applyRootMotion = true;
    }

    public void DeactivateRootMotion()
    {
        anim.applyRootMotion = false;
    }
}
