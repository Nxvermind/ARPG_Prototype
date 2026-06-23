using INab.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour , IEnemyDamageable
{
    #region Showed in Inspector

    public Transform target;

    public Transform body;

    public BoxCollider BodyCollider;

    public BoxCollider hurtBox;

    public string currentState;

    public float deathDelayDissolveFX;

    public bool ignorePlayer;

    #endregion

    #region Components

    public CharacterController CharacterController { get; private set; }
    public EnemyCombatBehavior EnemyCombatBehavior { get; private set; }

    public AnimationHandler AnimationHandler { get; private set; }

    public NavMeshAgent Agent { get; set; }

    public EnemyParameters EnemyParameters { get; private set; }

    public EnemyBlackboard EnemyBlackboard { get; private set; }

    public EnemyCombatSystem EnemyCombatSystem { get; private set; }

    public EnemyVision EnemyVision { get; private set; }

    public StaggerSystem StaggerSystem { get; private set; }

    public EnemyGroundDetection EnemyGroundDetection { get; private set; }

    public EnemyHitBox EnemyHitBox { get; private set; }

    public EnemySearchSystem EnemySearchSystem { get; private set; }

    public EnemyAttackRules EnemyAttackRules { get; private set; }

    public EnemyNavigation EnemyNavigation { get; private set; }

    public FacingSystem FacingSystem { get; private set; }

    public EnemyDissolveFXs EnemyDissolveFXs { get; private set; }
    public EnemyVerticalMovement EnemyVerticalMovement { get; private set; }
    public EnemySoundsFX SoundsFX { get; private set; }
    public EnemyLocomotionBehavior EnemyLocomotionBehavior { get; private set; }
    public EnemySpecificBehavior EnemySpecificBehavior { get; private set; }
    public EnemyHitReceiver EnemyHitReciever { get; protected set; }
    public EnemyRootMotion EnemyRootMotion { get; private set; }
    public MotionSystem MotionSystem { get; private set; }
    public GotHitReaction GotHitReaction { get; private set; }
    public EnemyMovementHandler EnemyMovementHandler { get; private set; }
    public EnemyHorizontalMovement EnemyHorizontalMovement { get; private set; }
    public ImpulseSystem ImpulseSystem { get; private set; }

    public WeaponTrailEffect WeaponTrailEffect { get; private set; }

    public ShowBloodVFX ShowBloodVFX { get; private set; }

    public EnemyBones EnemyBones { get; private set; }

    public FollowXZ FollowXZ { get; private set; }

    #endregion

    #region CanvasRelated

    public CanvasVanish canvasVanish;
    #endregion

    #region StateMachine

    public StateMachine<Enemy> StateMachine { get; private set; }

    public EnemyStateFactory EnemyStateFactory { get; set; }
    #endregion

    public bool syncronizeAgentAndAnimation;

    public virtual void Awake()
    {
        StateMachine = new StateMachine<Enemy>();

        EnemyBlackboard = GetComponent<EnemyBlackboard>();
        EnemyVision = GetComponentInChildren<EnemyVision>();
        Agent = GetComponent<NavMeshAgent>();
        EnemyParameters = GetComponent<EnemyParameters>();
        AnimationHandler = GetComponent<AnimationHandler>();
        StaggerSystem = GetComponent<StaggerSystem>();
        EnemyRootMotion = GetComponent<EnemyRootMotion>();
        CharacterController = GetComponent<CharacterController>();

        EnemySearchSystem = GetComponent<EnemySearchSystem>();
        EnemyCombatBehavior = GetComponent<EnemyCombatBehavior>();
        EnemyLocomotionBehavior = GetComponent<EnemyLocomotionBehavior>();
        EnemySpecificBehavior = GetComponent<EnemySpecificBehavior>();
        GotHitReaction = GetComponent<GotHitReaction>();
        EnemyAttackRules = new EnemyAttackRules(this);
        EnemyNavigation = new EnemyNavigation(this);
        FacingSystem = new FacingSystem(transform);
        MotionSystem = new MotionSystem();
        ImpulseSystem = new ImpulseSystem();
        EnemyVerticalMovement = new EnemyVerticalMovement();
        EnemyHorizontalMovement = new EnemyHorizontalMovement(Agent, transform);
        EnemyGroundDetection = GetComponentInChildren<EnemyGroundDetection>();
        SoundsFX = GetComponent<EnemySoundsFX>();
        WeaponTrailEffect = GetComponent<WeaponTrailEffect>();

        EnemyCombatSystem = GetComponent<EnemyCombatSystem>();

        EnemyHitBox = GetComponent<EnemyHitBox>();

        EnemyDissolveFXs = GetComponent<EnemyDissolveFXs>();
        ShowBloodVFX = GetComponentInChildren<ShowBloodVFX>();
        EnemyBones = GetComponentInChildren<EnemyBones>();
        FollowXZ = GetComponentInChildren<FollowXZ>();
        EnemyMovementHandler = new EnemyMovementHandler(
            CharacterController, 
            EnemyVerticalMovement,
            EnemyHorizontalMovement,
            MotionSystem, 
            EnemyRootMotion, 
            ImpulseSystem, 
            EnemyBlackboard,
            EnemyGroundDetection);
    }

    public virtual void Start()
    {
        EnemyBlackboard.isDead = false;
        EnemyRootMotion.ActivateRootMotion();
        Agent.updatePosition = false;
        syncronizeAgentAndAnimation = true;

        EnemyVerticalMovement.EnableGravity(true);
    }

    public void Update()
    {
        StateMachine.CurrentState?.Update();

        currentState = $"{StateMachine.CurrentState}";
    }

    public void LateUpdate()
    {
        StateMachine.CurrentState.LateUpdate();
    }

    public virtual void UpdateSpecificLogic()
    {
        SyncronizeAgentAndRootMotion();
        EnemySpecificBehavior.UpdateSpecificBehavior();
    }

    protected void SyncronizeAgentAndRootMotion()
    {
        //if (!syncronizeAgentAndAnimation) return;

        if (!EnemyRootMotion.useRootMotion) return;

        Vector3 worldDeltaPosition = Agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);

        if (worldDeltaPosition.magnitude > Agent.radius / 2)
        {
            transform.position = Vector3.Lerp(AnimationHandler.Anim.rootPosition, Agent.nextPosition, smooth);
        }

    }

    public bool IsPlayerInRange(float range)
    {
        Vector3 distance = target.position - transform.position;

        float sqrDistance = distance.sqrMagnitude;

        float sqrDistanceToPlayer = range * range;

        return sqrDistance <= sqrDistanceToPlayer;
    }

    public void TakeDamage(float damage)
    {
        EnemyParameters.currentHp -= damage;
        CheckHealthThresholds();
    }

    private void CheckHealthThresholds()
    {
        if (EnemyBlackboard.healthThresholds.Count == 0) return;

        foreach(var healthThreshold in EnemyBlackboard.healthThresholds)
        {
            if(!healthThreshold.triggered && EnemyParameters.currentHp/ EnemyParameters.maxHp * 100 <= healthThreshold.percentage)
            {
                healthThreshold.onThresholdReached.Invoke();
                healthThreshold.triggered = true;
            }
        }
    }
}
