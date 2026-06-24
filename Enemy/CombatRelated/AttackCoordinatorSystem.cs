using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCoordinatorSystem : MonoBehaviour
{
    [Tooltip("if enemy doesnt attack in this range of time then will change the aggressive attacker")]
    public float limitTimeWithoutAttacking;

    [SerializeField] private int maxNumOfMeleeAttackers;

    [SerializeField] private float executionTime;

    [Tooltip("time in seconds to evaluate which enemy is allowed to attack")]
    [SerializeField] private float evaluateTime;
    private float lastEvaluatedTime;
    [Space]
    [SerializeField] private float engageTime;
    private float OriginalEngageTime => 4.5f;
    public bool CanUpdateEngageTime { get; set; }

    private bool nobodyAllowedToAttack;

    public bool canAddNewAttacker = true;

    Coroutine delayedMeleeAssignment;
    Coroutine delayedRangedAssignment;

    public List<EnemyAttackParticipant> TotalEnemies= new();

    public List<EnemyAttackParticipant> meleeEnemies = new();
    public List<EnemyAttackParticipant> rangedEnemies = new();

    private readonly HashSet<EnemyAttackParticipant> meleeActiveAttackers = new();

    private readonly HashSet<EnemyAttackParticipant> rangedActiveAttackers = new();

    private readonly HashSet<EnemyAttackParticipant> pendingAttackers = new();

    private void Start()
    {
        engageTime = OriginalEngageTime;
    }

    private void OnEnable()
    {
        EventBus.OnExecutionStarted += Execution;
    }
    private void OnDisable()
    {
        EventBus.OnExecutionStarted -= Execution;
    }

    private void Update()
    {
        if (TotalEnemies.Count == 0 || nobodyAllowedToAttack) return;

        if(Time.time >= lastEvaluatedTime + evaluateTime)
        {
            CanUpdateEngageTime = false;

            foreach (EnemyAttackParticipant attacker in meleeActiveAttackers)
            {
                if (attacker.isEngagingPlayer && !attacker.IsAttackingPlayer)
                {
                    CanUpdateEngageTime = true;
                    break;
                }
            }

            Evaluate();

            lastEvaluatedTime = Time.time;
        }

        if (CanUpdateEngageTime)
        {
            engageTime -= Time.deltaTime;

            if (engageTime <= 0)
            {
                CanUpdateEngageTime = false;

                TryAddNewMeleeAttacker();

                engageTime = OriginalEngageTime;
            }
        }
    }

    public void Register(EnemyAttackParticipant participant)
    {
        if (!TotalEnemies.Contains(participant))
        {
            TotalEnemies.Add(participant);

            if(participant.EnemyType == EnemyType.melee)
            {
                meleeEnemies.Add(participant);
            }

            if(participant.EnemyType == EnemyType.ranged)
            {
                rangedEnemies.Add(participant);
            }

            participant.SetWaitingRole();

            Evaluate();
        }
    }

    private void Evaluate()
    {
        foreach (var enemy in TotalEnemies)
        {
            if (enemy.AttackRole != AttackRole.none) continue;

            if (enemy.ctx.IsPlayerInSight && !meleeActiveAttackers.Contains(enemy) && !rangedActiveAttackers.Contains(enemy))
            {
                enemy.SetWaitingRole();
            }
        }

        if (meleeActiveAttackers.Count == 0)
        {         
            AssignAttacker();
        }

        if(rangedActiveAttackers.Count == 0)
        {
            AssignRangedAttacker();
        }

        if (TotalEnemies.Count == 1) return;

        RemoveInvalidAttackers();
        
    }

    private void AssignAttacker()
    {
        if(meleeEnemies.Count == 0) return;

        if(meleeEnemies.Count == 1)
        {
            var attacker = GetBestMeleeAttackerParticipant();

            if (attacker != null)
            {
                meleeActiveAttackers.Add(attacker);
                attacker.SetAttackerRole();
                engageTime = OriginalEngageTime;

            }
            return;
        }

        delayedMeleeAssignment ??= StartCoroutine(DelayMeleeAttackerAssignment());
    }

    private void AssignRangedAttacker()
    {
        if (rangedEnemies.Count == 0) return;

        if (rangedEnemies.Count == 1)
        {
            var attacker = GetBestRangedAttackerParticipant();

            if (attacker != null)
            {
                rangedActiveAttackers.Add(attacker);
                attacker.SetAttackerRole();
            }
            return;
        }

        delayedRangedAssignment ??= StartCoroutine(DelayRangedAttackerAssignment());
    }
    private EnemyAttackParticipant GetBestMeleeAttackerParticipant()
    {
        EnemyAttackParticipant bestAttacker = null;
        float bestScore = float.MinValue;

        foreach (var enemy in meleeEnemies)
        {
            if (meleeActiveAttackers.Contains(enemy) || pendingAttackers.Contains(enemy) ||
                enemy.IsAttackOnCooldown || !enemy.ctx.IsPlayerInSight) continue;

            float score = enemy.GetScore();

            if (score > bestScore)
            {
                bestScore = score;

                bestAttacker = enemy;
            }
        }

        return bestAttacker;
    }

    private EnemyAttackParticipant GetBestRangedAttackerParticipant()
    {
        EnemyAttackParticipant best = null;
        float bestScore = float.MinValue;

        foreach (var enemy in rangedEnemies)
        {
            if (rangedActiveAttackers.Contains(enemy) || enemy.IsAttackOnCooldown ||
                !enemy.ctx.IsPlayerInSight) continue;

            float score = enemy.GetScore();
            if (score > bestScore)
            {
                bestScore = score;
                best = enemy;
            }
        }
        return best;
    }

    private void TryAddNewMeleeAttacker()
    {
        if(meleeActiveAttackers.Count < maxNumOfMeleeAttackers)
        {
            delayedMeleeAssignment ??= StartCoroutine(DelayMeleeAttackerAssignment());
        }
    }

    public void RemoveSpecificAttacker(EnemyAttackParticipant attacker)
    {
        if(meleeActiveAttackers.Contains(attacker)) meleeActiveAttackers.Remove(attacker);
        if(rangedActiveAttackers.Contains(attacker)) rangedActiveAttackers.Remove(attacker);    
    }

    private void RemoveInvalidAttackers()
    {
        meleeActiveAttackers.RemoveWhere(IsInvalidAttacker);
        rangedActiveAttackers.RemoveWhere(IsInvalidAttacker);
    }

    private bool IsInvalidAttacker(EnemyAttackParticipant attacker)
    {
        if (!attacker.ctx.IsPlayerInSight)
        {
            attacker.SetNoneRole();

            if (pendingAttackers.Contains(attacker))
            {
                pendingAttackers.Remove(attacker);
            }

            return true;
        }

        if (attacker.isPushedBack ||
            attacker.TimeWithoutAttacking > limitTimeWithoutAttacking)
        {
            attacker.SetWaitingRole();

            if (pendingAttackers.Contains(attacker))
            {
                pendingAttackers.Remove(attacker);
            }

            return true;
        }

        return false;
    }

    public void OnEnemyDeath(EnemyAttackParticipant participant)
    {
        TotalEnemies.Remove(participant);

        meleeEnemies.Remove(participant);
        meleeActiveAttackers.Remove(participant);
        rangedEnemies.Remove(participant);
        rangedActiveAttackers.Remove(participant);

        pendingAttackers.Remove(participant);

        if (TotalEnemies.Count <= 0)
        {
            engageTime = OriginalEngageTime;
        }
    }

    public void ParticipantAttackComplete(EnemyAttackParticipant attacker)
    {
        engageTime = OriginalEngageTime;

        if (meleeActiveAttackers.Contains(attacker))
        {
            attacker.StartAttackCooldown();
            meleeActiveAttackers.Remove(attacker);
            attacker.SetWaitingRole();
        }

        if (rangedActiveAttackers.Contains(attacker))
        {
            attacker.StartAttackCooldown();
            rangedActiveAttackers.Remove(attacker);
            attacker.SetWaitingRole();
        }
    }

    public void Execution()
    {
        StartCoroutine(ExecutionCor());
    }

    IEnumerator ExecutionCor()
    {
        nobodyAllowedToAttack = true;
        foreach (var enemy in TotalEnemies)
        {
            enemy.SetWaitingRole();
        }

        if(delayedRangedAssignment != null)
        {
            StopCoroutine(delayedRangedAssignment);
            delayedRangedAssignment = null;
        }

        yield return new WaitForSecondsRealtime(executionTime);

        nobodyAllowedToAttack = false;

        foreach(var enemy in meleeActiveAttackers)
        {
            enemy.SetAttackerRole();
        }

        foreach(var enemy in rangedActiveAttackers)
        {
            enemy.SetAttackerRole();
        }
    }

    IEnumerator DelayMeleeAttackerAssignment()
    {     
        var enemy = GetBestMeleeAttackerParticipant();

        if (enemy == null)
        {
            delayedMeleeAssignment = null;
            yield break;
        }

        pendingAttackers.Add(enemy);

        float rndTime = Random.Range(3, 6) * .1f;

        yield return new WaitForSeconds(rndTime);

        if (enemy == null || !TotalEnemies.Contains(enemy) || !enemy.ctx.IsPlayerInSight)
        {
            pendingAttackers.Remove(enemy);
            delayedMeleeAssignment = null;
            yield break;
        }

        pendingAttackers.Remove(enemy);
        meleeActiveAttackers.Add(enemy);

        enemy.SetAttackerRole();

        engageTime = OriginalEngageTime;
        delayedMeleeAssignment = null;
    }

    IEnumerator DelayRangedAttackerAssignment()
    {
        var enemy = GetBestRangedAttackerParticipant();

        if (enemy == null)
        {
            delayedRangedAssignment = null;
            yield break;
        }

        pendingAttackers.Add(enemy);

        int rndTime = Random.Range(4, 6);

        yield return new WaitForSeconds(rndTime);

        if (enemy == null || !TotalEnemies.Contains(enemy) || !enemy.ctx.IsPlayerInSight)
        {
            pendingAttackers.Remove(enemy);
            delayedRangedAssignment = null;
            yield break;
        }

        pendingAttackers.Remove(enemy);
        rangedActiveAttackers.Add(enemy);

        enemy.SetAttackerRole();

        delayedRangedAssignment = null;
    }
}
