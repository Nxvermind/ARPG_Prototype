using System.Collections;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseState : State<Enemy>
{
    protected Transform target;
    protected EnemyParameters enemyParameters;
    protected EnemyStateFactory enemyStateFactory;
    protected AnimationHandler animationHandler;
    protected EnemyBlackboard enemyBlackboard;
    protected EnemyVision enemyVision;
    protected NavMeshAgent agent;
    protected EnemyGroundDetection enemyGroundDetection;
    protected EnemySearchSystem enemySearchSystem;
    protected EnemyCombatStatus enemyCombatStatus;

    #region Enemy Subclasses
    protected BasicEnemy basicEnemy;
    protected EliteEnemy eliteEnemy;

    protected BasicEnemyStateFactory basicEnemyStateFactory;
    protected EliteEnemyStateFactory eliteEnemyStateFactory;
    #endregion

    #region Bools

    protected bool stopLookingAtPlayer;

    protected bool hasRecentlyAttacked;

    protected bool inStunState;

    #endregion

    public EnemyBaseState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
        target = entity.target;
        enemyParameters = entity.EnemyParameters;
        animationHandler = entity.AnimationHandler;
        enemyBlackboard = entity.EnemyBlackboard;
        enemyVision = entity.EnemyVision;
        agent = entity.Agent;
        enemyGroundDetection = entity.EnemyGroundDetection;
        enemySearchSystem = entity.EnemySearchSystem;
        this.enemyStateFactory = entity.EnemyStateFactory;
        enemyCombatStatus = entity.EnemyCombatStatus;
        basicEnemy = entity as BasicEnemy;
        eliteEnemy = entity as EliteEnemy;
        basicEnemyStateFactory = enemyStateFactory as BasicEnemyStateFactory;
        eliteEnemyStateFactory = enemyStateFactory as EliteEnemyStateFactory;
    }

    public override void Enter()
    {
        if (basicEnemy)
        {
            if (stateMachine.CurrentState != enemyStateFactory.StaggerState)
            {
                basicEnemy.executionImageGO.SetActive(false);
            }
        }
    }
    public override void Exit()
    {
    }
    public override void Update()
    {
        entity.UpdateSpecificLogic();

        GotHit();

        if (enemyParameters.currentStaggerValue >= enemyParameters.maxStaggerValue)
        {
            enemyBlackboard.onlyTakeDamage = true;
            enemyBlackboard.canUpdateStaggerValue = false;
        }

        if (enemyParameters.currentStaggerValue >= enemyParameters.maxStaggerValue && stateMachine.CurrentState != enemyStateFactory.StaggerState && entity.EnemyGroundDetection.isGrounded)
        {
            if (basicEnemy)
            {
                stateMachine.ChangeState(enemyStateFactory.StaggerState);
            }
            else if (eliteEnemy && !inStunState)
            {
                stateMachine.ChangeState(eliteEnemyStateFactory.StunState);
            }
        }

        if (enemyParameters.currentHp <= 0)
        {
            stateMachine.ChangeState(enemyStateFactory.DeathState);
        }

        if (enemyVision.playerDisappeared && stateMachine.CurrentState != enemyStateFactory.StaggerState && entity.EnemyGroundDetection.isGrounded)
        {
            if (basicEnemy)
            {
                stateMachine.ChangeState(enemyStateFactory.StaggerState);
            }
        }

        if (Time.time >= entity.EnemyBlackboard.gotHitLastTime + entity.EnemyBlackboard.timeToStartStaggerRegeneration)
        {
            entity.StaggerSystem.BackToZeroStaggerValue(Time.deltaTime * entity.EnemyParameters.regenStaggerValue);
        }

        if (enemyVision.playerInSight)
        {
            enemyVision.playerDisappeared = false;

            if (basicEnemy)
            {
                enemyVision.visionRange = 35;
            }

            if (eliteEnemy)
            {
                enemyVision.visionRange = 70;
            }
            
        }

        if (enemyVision.playerInSight && !stopLookingAtPlayer)
        {
            Vector3 lookTarget = target.position;
            lookTarget.y = entity.transform.position.y;
            entity.transform.LookAt(lookTarget);
        }

        if (enemyBlackboard.gotHitByUltimateSkill && enemyParameters.currentHp > 0) 
        {
            if (basicEnemy && stateMachine.CurrentState != enemyStateFactory.StaggerState)
            {
                stateMachine.ChangeState(enemyStateFactory.GroundGotHitState);
            }

            if (eliteEnemy && stateMachine.CurrentState != eliteEnemyStateFactory.StunState)
            {
                stateMachine.ChangeState(enemyStateFactory.GroundGotHitState);
            }

        }
    }

    private void GotHit()
    {
        if (enemyBlackboard.gotHit)
        {
            AttackNode currentAttack = entity.attackProvider.CurrentAttackNode;

            enemyBlackboard.gotHitLastTime = Time.time;

            if (enemyBlackboard.gotHitByDashSkill)
            {
                enemyBlackboard.gotHit = false;
                stateMachine.ChangeState(enemyStateFactory.GroundGotHitState);
                return;
            }

            if (entity.EnemyBlackboard.onlyTakeDamage)
            {
                entity.LastAttackNode = currentAttack;
                enemyBlackboard.gotHit = false;

                if (currentAttack != null)
                {
                    enemyParameters.currentHp -= currentAttack.damage;

                }

                if (enemyBlackboard.canUpdateStaggerValue)
                {
                    entity.StaggerSystem.UpdateStaggerValue();
                }
            }
            else
            {
                if (entity.EnemyGroundDetection.isGrounded)
                {
                    if (currentAttack.upStrongAttack)
                    {
                        entity.LastAttackNode = currentAttack;
                        enemyBlackboard.gotHit = false;

                        if (currentAttack != null)
                        {
                            enemyParameters.currentHp -= currentAttack.damage;
                            entity.StaggerSystem.UpdateStaggerValue();
                        }

                        stateMachine.ChangeState(enemyStateFactory.GroundGotHitState);
                    }
                    else if (currentAttack.botStrongAttack)
                    {
                        entity.LastAttackNode = currentAttack;
                        enemyBlackboard.gotHit = false;

                        if (currentAttack != null)
                        {
                            enemyParameters.currentHp -= currentAttack.damage;
                            entity.StaggerSystem.UpdateStaggerValue();
                        }

                        stateMachine.ChangeState(enemyStateFactory.GroundGotHitState);
                    }
                    else
                    {
                        entity.LastAttackNode = currentAttack;
                        enemyBlackboard.gotHit = false;

                        if (currentAttack != null)
                        {

                            enemyParameters.currentHp -= currentAttack.damage;
                            entity.StaggerSystem.UpdateStaggerValue();
                        }

                        stateMachine.ChangeState(enemyStateFactory.GroundGotHitState);
                    }
                }
                else
                {
                    enemyBlackboard.gotHit = false;

                    if (currentAttack != null)
                    {
                        enemyParameters.currentHp -= currentAttack.damage;
                        entity.StaggerSystem.UpdateStaggerValue();
                    }

                    //entity.LastAttackNode = entity.attackProvider.CurrentAttackNode;

                    stateMachine.ChangeState(enemyStateFactory.AirGotHitState);
                }
            }

            if(currentAttack != null || entity.LastAttackNode != null)
            {
                TimeScaleManager.Instance.ApplyHitstop(0.1f, 0.03f);
            }
        }
    }

    protected bool InRangeToAttack()
    {
        bool inRange;

        Vector3 distance = target.position - entity.transform.position;

        float sqrDistanceToPlayer = distance.sqrMagnitude;

        float sqrDistanceToAttack = enemyParameters.distanceToAttack * enemyParameters.distanceToAttack;

        Vector3 enemyVector = entity.transform.forward;
        Vector3 d = (target.position - entity.transform.position).normalized;

        float dot = Vector3.Dot(enemyVector, d);

        if (sqrDistanceToPlayer <= sqrDistanceToAttack && dot > .96f)
        {
            inRange = true;
            //Debug.Log("in range to attack");
        }
        else
        {
            inRange = false;
        }


        return inRange;
    }

    protected bool HasReachedDestination()
    {
        if (agent.pathPending) return false;

        // llegó si está muy cerca del destino
        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            // y si ya casi no se mueve
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
            {
                return true;
            }
        }

        return false;
    }

}

