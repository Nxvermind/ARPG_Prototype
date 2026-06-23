using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public Animator Anim { get; private set; }

    private void Awake()
    {
        Anim = GetComponent<Animator>();     
    }

    public void Play(string animationName, int layer = -1, float normalizedTime = 0)
    {
        Anim.Play(animationName, layer, normalizedTime);
    }

    public void Play(int animationHashName, int layer = -1, float normalizedTime = 0)
    {
        Anim.Play(animationHashName, layer, normalizedTime);
    }

    public void CrossFade(string animationName, float fadeTime, int layer = -1)
    {
        Anim.CrossFade(animationName, fadeTime, layer);
    }

    public void CrossFade(int animationHashName, float fadeTime, int layer = -1)
    {
        Anim.CrossFade(animationHashName, fadeTime, layer);
    }

    public bool IsPlaying(string animationName)
    {
        return Anim.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    public float NormalizedTime()
    {
        return Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public bool IsInTransition(int layer = 0)
    {
        return Anim.IsInTransition(layer);
    }
    public AnimatorStateInfo StateInfo(int layer = 0)
    {
        return Anim.GetCurrentAnimatorStateInfo(layer);
    }
}
