using System;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public Transform executionPoint;

    public Transform[] patrolPoints;
    public GameObject executionImageGO;

    public ExecutionManager executionManager;

    public BasicEnemyStateFactory basicEnemyStateFactory;

    public EnemyCurves EnemyCurves { get; private set; }

    public bool isPraying;

    public bool isPatroling;

    #region

    public static event Action<Enemy> OnExecutionRequest;

    #endregion
    public override void Awake()
    {
        base.Awake();

        EnemyCurves = GetComponent<EnemyCurves>();

        basicEnemyStateFactory = new BasicEnemyStateFactory();

        EnemyStateFactory = basicEnemyStateFactory;

        basicEnemyStateFactory.InitializeState(this, basicEnemyStateFactory, stateMachine);
    }

    public override void Start()
    {
        if (isPraying)
        {
            stateMachine.Initialize(basicEnemyStateFactory.PrayingState);

        }
        else if (isPatroling)
        {
            stateMachine.Initialize(basicEnemyStateFactory.PatrolState);
        }
        else
        {
            stateMachine.Initialize(basicEnemyStateFactory.IdleState);
        }

        foreach (var transform in patrolPoints)
        {
            transform.SetParent(null);
        }

        base.Start();
    }

    private void OnEnable()
    {
        EventBus.OnExecutionStarted += GoToWaitingForAttackState;
    }

    private void OnDisable()
    {
        EventBus.OnExecutionStarted -= GoToWaitingForAttackState;
    }

    private void OnAnimatorMove()
    {
        rb.rotation *= AnimationHandler.Anim.deltaRotation;

        if (AnimationHandler.Anim.applyRootMotion)
        {
            rb.position += AnimationHandler.Anim.deltaPosition;
        }

    }

    public void ThisEnemyIsReadyToBeExecuted(Enemy enemy)
    {
        OnExecutionRequest?.Invoke(enemy);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("EnemyDetector"))
        {
            if (!coordinator.enemyAttackWaitList.Contains(this))
            {
                coordinator.RegisterEnemy(this);
            }
        }
    }

    public override void UpdateSpecificLogic()
    {
        base.UpdateSpecificLogic();

        //Debug.Log($"im in {stateMachine.CurrentState}");

        if(enemyType == EnemyType.Melee)
        {
            MeleeEnemyLogic();
        }

        if (EnemyBlackboard.isBeingExecuted && !EnemyBlackboard.wasExecuted)
        {
            EnemyBlackboard.wasExecuted = true;
            stateMachine.ChangeState(basicEnemyStateFactory.ExecutedState);
        }

        if (EnemyParryStatus != null && EnemyParryStatus.isParried)
        {
            stateMachine.ChangeState(EnemyStateFactory.ParriedState);
        }

        if (EnemyCombatStatus.isActiveAttacker)
        {
            EnemyCombatStatus.waitingForAttackPermission = false;
        }

        if (EnemyCombatStatus.waitingForAttackPermission)
        {
            EnemyCombatStatus.timeSinceWaitingForAttackPermission += Time.deltaTime;
        }
    }

    private void GoToWaitingForAttackState()
    {
        if (EnemyBlackboard.isBeingExecuted || !EnemyCombatStatus.waitingForAttackPermission) return;

        stateMachine.ChangeState(EnemyStateFactory.WaitingForAttackState);
    }

    private void MeleeEnemyLogic()
    {
        if (coordinator.meleeWaitList.Count == 1) return;

        if (IsPlayerInRange(3) && EnemyCombatStatus.isActiveAttacker)
        {
            EnemyCombatStatus.limitTimeWithoutAttacking -= Time.deltaTime;

            if (EnemyCombatStatus.limitTimeWithoutAttacking <= 0)
            {
                coordinator.ChangeActiveAttacker(this);
                EnemyCombatStatus.limitTimeWithoutAttacking = EnemyCombatStatus.originalLimitTime;
            }
        }
        else
        {
            EnemyCombatStatus.limitTimeWithoutAttacking = EnemyCombatStatus.originalLimitTime;
        }
    }
}

