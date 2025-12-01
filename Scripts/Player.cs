using System;
using UnityEngine;

public class Player : MonoBehaviour , ICurrentAttackNodeProvider , IDamageable, IHitable
{
    #region PlayerComponents

    public Transform cameraTransform;

    public Collider hurtBox;

    public PlayerParameters Parameters { get; private set; }

    public InputHandler InputHandler { get; private set; }

    public AnimationHandler AnimationHandler { get; private set; }

    public StaminaComponent StaminaComponent { get; private set; }

    public HealthComponent HealthComponent { get; private set; }

    public PostureComponent PostureComponent { get; private set; }

    [HideInInspector] public CharacterController CharacterController;

    public EnemyDetector EnemyDetector { get; private set; }

    public ComboSystem ComboSystem { get; private set; }

    public ParrySystem parrySystem;

    public AttackNode CurrentAttackNode => ComboSystem.CurrentAttackNode; //interface implemented

    [HideInInspector] public AttackNode lastAttackNode;

    public SlashVFX SlashVFX { get; private set; }

    private SlashSoundFX slashSoundFX;

    public SkillsSFX SkillsSFX { get; private set; }

    [HideInInspector] public RootMotionMove rootMotion;

    public SwitchCameras SwitchCameras { get; private set; }

    public SkillUIManager SkillManager { get; private set; }

    public CanvasVanish canvasVanish;

    public PlayerBlackboard PlayerBlackboard { get; private set; }

    public UltimateSkill UltimateSkill { get; private set; }

    public InputBuffer InputBuffer { get; private set; }

    public ExecutionManager executionManager;

    #endregion

    #region StateMachine&Factory
    public StateMachine<Player> StateMachine {  get; private set; }

    public PlayerStateFactory PlayerStateFactory {  get; private set; }
    #endregion

    #region Events

    public static event Action OnPlayerAttackButtonPressed;

    public static event Action OnUltimateSkillCalled;

    #endregion

    private void Awake()
    {
        StateMachine = new StateMachine<Player>();
        PlayerStateFactory = new PlayerStateFactory();

        CharacterController = GetComponent<CharacterController>();
        EnemyDetector = GetComponentInChildren<EnemyDetector>();
        ComboSystem = GetComponentInChildren<ComboSystem>();
        StaminaComponent = GetComponent<StaminaComponent>();
        HealthComponent = GetComponent<HealthComponent>();
        PostureComponent = GetComponent<PostureComponent>();
        InputHandler = GetComponent<InputHandler>();
        AnimationHandler = GetComponentInChildren<AnimationHandler>();
        Parameters = GetComponent<PlayerParameters>();

        SlashVFX = GetComponentInChildren<SlashVFX>();

        slashSoundFX = GetComponentInChildren<SlashSoundFX>();

        SkillsSFX = GetComponentInChildren<SkillsSFX>();

        rootMotion = GetComponentInChildren<RootMotionMove>();

        SwitchCameras = GetComponent<SwitchCameras>();

        SkillManager = GetComponent<SkillUIManager>();

        PlayerBlackboard = GetComponent<PlayerBlackboard>();

        UltimateSkill = GetComponentInChildren<UltimateSkill>();

        PlayerStateFactory.InitializeState(this, StateMachine);

        SlashVFX.Initialize(this);

        slashSoundFX.Initialize(this);

        InputBuffer = new InputBuffer();
    }

    private void Start()
    {
        StateMachine.Initialize(PlayerStateFactory.IdleState);
        StaminaComponent.SetPlayerParameters(Parameters);

    }

    private void OnEnable()
    {
        EventBus.OnPlayerBlockEvent += PlayBlockAnimation;
    }

    private void OnDisable()
    {
        EventBus.OnPlayerBlockEvent -= PlayBlockAnimation;
    }

    void Update()
    {
        StateMachine.CurrentState.Update();
    }

    public void TakeDamage(float damage, bool accessGotHit)
    {

        if (PlayerBlackboard.isInvulnerable) return;

        Parameters.currentHp -= damage;
        HealthComponent.UpdateHealthValue();

        if (accessGotHit)
        {
            StateMachine.ChangeState(PlayerStateFactory.GotHitState);
        }
    }

    public void AttackButtonPressed()
    {
        OnPlayerAttackButtonPressed?.Invoke();
    }

    private void PlayBlockAnimation()
    {
        AnimationHandler.Play("Parry_Accept");
    }

    public void OnHit(Enemy attacker)
    {
        if (PlayerBlackboard.isInvulnerable) return;

        TakeDamage(attacker.EnemyCombatSystem.CurrentAttackData.damage, false);

        //Debug.Log($"got hit by {attacker.EnemyCombatSystem.CurrentAttackData.attackAnimationName}");

        //if (StateMachine.CurrentState == PlayerStateFactory.GroundAttackState)
        //{
        //    return;
        //}

        if (PlayerBlackboard.onlyTakeDamage) return;

        StateMachine.ChangeState(PlayerStateFactory.GotHitState);
    }

    public void UltimateSkillCalled()
    {
        OnUltimateSkillCalled?.Invoke();
    }
}
