using System.Collections;
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
    protected EnemyCombatBehavior enemyCombatBehavior;
    protected EnemyRootMotion rootMotion;

    #region Enemy Subclasses
    protected BasicEnemy basicEnemy;
    protected EliteEnemy eliteEnemy;

    protected BasicEnemyStateFactory basicEnemyStateFactory;
    protected EliteEnemyStateFactory eliteEnemyStateFactory;
    #endregion

    #region Bools

    protected bool inStaggerState;

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
        basicEnemy = entity as BasicEnemy;
        eliteEnemy = entity as EliteEnemy;
        basicEnemyStateFactory = enemyStateFactory as BasicEnemyStateFactory;
        eliteEnemyStateFactory = enemyStateFactory as EliteEnemyStateFactory;
        enemyCombatBehavior = entity.EnemyCombatBehavior;
        rootMotion = entity.EnemyRootMotion;
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

        //Debug.Log($"i enter {stateMachine.CurrentState} and {stateMachine.PreviousState} was my last state");
    }
    public override void Exit()
    {
        if (eliteEnemy)
        {
            foreach(var t in eliteEnemy.overrideTransforms)
            {
                t.weight = 0;
            }
        }
    }
    public override void Update()
    {
        //Debug.Log($"im {entity.transform.name} and im in {stateMachine.CurrentState}");

        if (enemyBlackboard.canDie && enemyParameters.currentHp <= 0)
        {
            stateMachine.ChangeState(enemyStateFactory.DeathState);
            return;
        }

        entity.UpdateSpecificLogic();
        enemyGroundDetection.CheckGround();
        entity.EnemyRootMotion.UpdateRootMotion();
        entity.EnemyMovementHandler.UpdateMovement();
        enemyVision.CheckVision();

        if (Time.time >= entity.EnemyBlackboard.gotHitLastTime + entity.EnemyBlackboard.timeToStartStaggerRegeneration)
        {
            entity.StaggerSystem.BackToZeroStaggerValue(Time.deltaTime * entity.EnemyParameters.regenStaggerValue);
        }

        if (enemyVision.isPlayerInSight)
        {
            enemyVision.playerDisappeared = false;

            if (eliteEnemy)
            {
                enemyVision.visionRange = 200;
            }

            if (basicEnemy) enemyVision.visionRange = 30;

            HandleFacingDirection();
        }
    }

    private void HandleFacingDirection()
    {
        if (!enemyBlackboard.canLookAtPlayer) return;

        if (enemyBlackboard.lookDirectlyAtPlayer)
        {
            entity.FacingSystem.FaceTarget(target);
            return;
        }

        Vector3 moveDir = agent.steeringTarget - entity.transform.position;
        Vector3 toTarget = target.position - entity.transform.position;

        if (DirectionUtility.Dot(moveDir, toTarget) >= .93f)
        {
            entity.FacingSystem.FaceTarget(target);
            return;
        }

        moveDir.y = 0f;

        if (moveDir.sqrMagnitude < 0.01f)
        {
            moveDir = entity.transform.forward;
        }
        else
        {
            moveDir.Normalize();
        }

        toTarget.y = 0f;
        toTarget.Normalize();

        float distance = Vector3.Distance(entity.transform.position, target.position);
        float t = Mathf.Clamp01(1f - distance / 10f);
        float targetWeight = Mathf.Lerp(0.5f, 1f, t);

        Vector3 blendedDir = (moveDir * 2 + toTarget * targetWeight).normalized;

        entity.FacingSystem.RotateTowardsDirection(blendedDir, 30f);
    }
}

