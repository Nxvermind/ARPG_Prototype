using Unity.VisualScripting;
using UnityEngine;

public class EliteEnemy : Enemy
{
    public Transform startPos;

    public SkillSmash skillSmash;

    public int hitCounter;

    public int maxHitCounter;

    public float aggressiveThreshold;

    public bool secondPhaseActive;

    public EnemyAttackSettings startAttackSetting;

    #region StateFactory
    public EliteEnemyStateFactory eliteEnemyStateFactory;
    #endregion
    public override void Awake()
    {
        base.Awake();

        eliteEnemyStateFactory = new EliteEnemyStateFactory();
        EnemyStateFactory = eliteEnemyStateFactory;

        skillSmash = GetComponent<SkillSmash>();



        eliteEnemyStateFactory.InitializeState(this, eliteEnemyStateFactory ,stateMachine);
    }

    public override void Start()
    {
        stateMachine.Initialize(eliteEnemyStateFactory.IdleState);
        base.Start();
    }

    private void OnEnable()
    {
        PlayerEvents.OnPlayerAttackButtonPressed += GoToEvadeState;
        RespawnPlayer.OnPlayerRespawn += ResetStats;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerAttackButtonPressed -= GoToEvadeState;
        RespawnPlayer.OnPlayerRespawn -= ResetStats;

    }

    public override void Update()
    {
        base.Update();
    }
    private void OnAnimatorMove()
    {
        rb.rotation *= AnimationHandler.Anim.deltaRotation;

        if(AnimationHandler.Anim.applyRootMotion)
        {
            rb.position += AnimationHandler.Anim.deltaPosition;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void UpdateSpecificLogic()
    {
        base.UpdateSpecificLogic();

        if (skillSmash.skillSmashReady && Time.time >= EnemyBlackboard.gotHitLastTime + 3)
        {
            stateMachine.ChangeState(eliteEnemyStateFactory.SkillSmashState);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            stateMachine.ChangeState(EnemyStateFactory.TurnAroundState);
        }

        if(EnemyParameters.currentHp <= EnemyParameters.maxHp / 2)
        {
            EnemyCombatSystem.attackSettings = EnemyCombatSystem.aggresiveSettings;
        }

        if(hitCounter >= maxHitCounter)
        {
            EnemyBlackboard.onlyTakeDamage = true;
            EnemyBlackboard.canUpdateStaggerValue = true;
        }


        if (stateMachine.CurrentState == EnemyStateFactory.AttackState)
        {
            if (!IsPlayerInRange(1))
            {
                AnimationHandler.Anim.applyRootMotion = true;
            }
        }

        if (EnemyParameters.currentHp < EnemyParameters.maxHp * aggressiveThreshold && !secondPhaseActive)
        {
            stateMachine.ChangeState(eliteEnemyStateFactory.SecondPhaseState);
        }
    }

    private void GoToEvadeState()
    {
        if (stateMachine.CurrentState == eliteEnemyStateFactory.StunState || !EnemyVision.playerInSight) return;

        float rnd = Random.value;

        if(rnd <= 0.1f && IsPlayerInRange(8))
        {
            stateMachine.ChangeState(eliteEnemyStateFactory.EvadeState);
        }
    }

    private void ResetStats()
    {
        EnemyParameters.currentHp = EnemyParameters.maxHp;
        secondPhaseActive = false;
        maxHitCounter = 5;

        EnemyCombatSystem.attackSettings = startAttackSetting;

        rb.position = startPos.position;
    }
}
