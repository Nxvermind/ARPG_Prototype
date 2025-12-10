using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SkillSmash : MonoBehaviour
{
    private AnimationHandler animationHandler;

    [Header("Skill Cooldown")]
    public int minCooldown;
    public int maxCooldown;

    private int skillCooldown;

    public bool skillSmashReady;

    [Header("Skill Parameters")]
    [Tooltip("Force applied to elevate the enemy, it needs to be enough to elevate the enemy until its maxHeight")]
    public float upForce;
    [Tooltip("When the max height is reached, the time needed to land since start falling")]
    public float timeToLand;
    public float maxHeight;

    [Header("Skill Damage")]
    public float radius;
    public float skillDamage;
    public LayerMask playerLayer;

    [Header("Skill SFX")]
    public AudioClip skillAudioClip;
    public AudioSource audioSource;

    [Header("Impulse Source")]
    public CinemachineImpulseSource skillImpulseSource;

    private readonly Collider[] coll = new Collider[3];

    private IDamageable cachedTarget;

    public bool AllowedToJump { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animationHandler = GetComponent<AnimationHandler>();
    }

    public void PlaySkillSFX()
    {
        audioSource.PlayOneShot(skillAudioClip);
    }

    public void SkillSmashCameraShake()
    {
        float x = Random.value < 0.5f ? -0.75f : 0.75f;
        float y = Random.value < 0.5f ? -0.75f : 0.75f;

        Vector3 vel = new Vector3(x, y, 0f);
        skillImpulseSource.GenerateImpulseWithVelocity(vel);
    }

    public IEnumerator SkillCD()
    {
        skillCooldown = Random.Range(minCooldown, maxCooldown);
        //Debug.Log($"the cooldown is {skillCooldown}");
        yield return new WaitForSecondsRealtime(skillCooldown);
        skillSmashReady = true;
    }

    public void Elevate(Rigidbody rb)
    {
        rb.AddForce(Vector3.up * upForce, ForceMode.VelocityChange);
    }

    public IEnumerator LerpPosition(Vector3 target, Rigidbody rb)
    {
        float timeElapsed = 0;

        Vector3 start = transform.position;
        Vector3 destiny = target - Vector3.back * 2;

        while (timeElapsed < timeToLand)
        {
            timeElapsed += Time.deltaTime;

            float t = timeElapsed / timeToLand;
            t = Mathf.SmoothStep(0, 1, t);

            Vector3 newPos = Vector3.Lerp(start, destiny, t);

            rb.MovePosition(newPos);

            yield return null;
        }

        rb.MovePosition(destiny);
    }

    //called in an animation event
    public void ChangeAnimationSpeed(float speed)
    {
        animationHandler.Anim.SetFloat("speed", speed);
    }

    //called in an animation event
    public IEnumerator LetJumpCor()
    {
        AllowedToJump = true;
        yield return null;
        AllowedToJump = false;
    }


    public void SkillDamage()
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, radius, coll, playerLayer);

        if(hits > 0)
        {
            cachedTarget ??= coll[0].GetComponentInParent<IDamageable>();

            cachedTarget.TakeDamage(skillDamage, true);
        }
    }

}
