using System.Collections;
using UnityEngine;

public enum AttackRole
{
    none,
    attacker,
    waiting
}

public enum EnemyType
{
    none,
    melee,
    ranged
}

public class EnemyAttackParticipant : MonoBehaviour
{
    [SerializeField] private AttackCoordinatorSystem attackCoordinatorSystem;
    public EnemyAggressionScore AggressionScore { get; private set; }
    public EnemyCombatContext ctx { get; private set; }

    public EnemyType EnemyType;
    public AttackRole AttackRole;

    public float attackCooldown;
    public bool IsAttackOnCooldown { get; private set; }

    private Coroutine attackCooldownRoutine;

    public float TimeWithoutAttacking;

    public bool IsAttackingPlayer { get; set; }

    [Header("Engaging")]
    public bool isEngagingPlayer;

    public bool isPushedBack;

    private float waitingAttackTime;
    public float TimeSinceWaitingForAttackPermission => AttackRole == AttackRole.waiting ? Time.time - waitingAttackTime : 0;

    private void Awake()
    {
        AggressionScore = GetComponent<EnemyAggressionScore>();
        ctx = GetComponent<EnemyCombatContext>();
    }

    private void Start()
    {
        AttackRole = AttackRole.none;
    }

    public float GetScore() => AggressionScore.CalculateScore();

    public void SetAttackerRole()
    {
        AttackRole = AttackRole.attacker;
        TimeWithoutAttacking = 0;
    }

    public void SetWaitingRole()
    {
        AttackRole = AttackRole.waiting;

        waitingAttackTime = Time.time;
    }

    public void SetNoneRole()
    {
        AttackRole = AttackRole.none;
        TimeWithoutAttacking = 0;
    }

    public void UpdateTimeWithoutAttacking(float deltaTime) => TimeWithoutAttacking += deltaTime;

    public void ZeroTimeWithoutAttacking() => TimeWithoutAttacking = 0;

    public void StartAttackCooldown()
    {
        if (attackCooldownRoutine != null) return;

        attackCooldownRoutine = StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        IsAttackOnCooldown = true;

        yield return new WaitForSeconds(attackCooldown);

        IsAttackOnCooldown = false;

        attackCooldownRoutine = null;
    }
}
