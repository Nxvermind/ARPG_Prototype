using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [HideInInspector] public Animator Anim;

    [HideInInspector] public AnimatorStateInfo stateInfo;

    private void Awake()
    {
        Anim = GetComponent<Animator>();     
    }

    public void Play(string animationName, int layer = -1, float normalizedTime = 0.0f)
    {
        Anim.Play(animationName);
    }

    public void CrossFade(string animationName, float fadeTime, int layer = -1, float normalizedTimeOffset = 0.0f)
    {
        Anim.CrossFade(animationName, fadeTime);
    }

    public void CrossFadeInFixedTime(string animationName, float fixedTransitionDuration, int layer = -1, float fixedTimeOffset = 0.0f, float normalizedTransitionTime = 0.0f)
    {
        Anim.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }

    public bool IsPlaying(string animationName)
    {
        return Anim.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    public float NormalizedTime()
    {
        return Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public float AnimationLength()
    {
        return Anim.GetCurrentAnimatorStateInfo(0).length;
    }

    public float AnimationSpeed()
    {
        return Anim.GetCurrentAnimatorStateInfo(0).speed;
    }
}
