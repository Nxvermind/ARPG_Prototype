using System;
using UnityEngine;

public class Player : MonoBehaviour , ICurrentAttackNodeProvider , IDamageable, IHitable
{
    #region Showed In Inspector

    public Transform cameraTransform;

    public Collider hurtBox;

    public ParrySystem parrySystem;

    public CanvasVanish canvasVanish;

    public ExecutionManager executionManager;

    public IntroScript introScript;

    public bool playerRespawned;

    #endregion

    #region PlayerComponents

    public PlayerParameters Parameters { get; private set; }

    public InputHandler InputHandler { get; private set; }

    public AnimationHandler AnimationHandler { get; private set; }

    public CharacterController CharacterController { get; private set; }

    public EnemyDetector EnemyDetector { get; private set; }

    public ComboSystem ComboSystem { get; private set; }

    public AttackNode CurrentAttackNode => ComboSystem.CurrentAttackNode; //interface implemented

    [HideInInspector] public AttackNode lastAttackNode;

    public SlashVFX SlashVFX { get; private set; }

    private SlashSoundFX slashSoundFX;

    public SkillsSFX SkillsSFX { get; private set; }

    [HideInInspector] public RootMotion rootMotion;

    public SwitchCameras SwitchCameras { get; private set; }

    public SkillUIManager SkillManager { get; private set; }

    public PlayerBlackboard PlayerBlackboard { get; private set; }

    public UltimateSkill UltimateSkill { get; private set; }

    public InputBuffer InputBuffer { get; private set; }

    public PlayerHorizontalMovement PlayerHorizontalMovement { get; private set; }

    public PlayerVerticalMovement PlayerVerticalMovement { get; private set; }

    public ImpulseSystem ImpulseSystem { get; private set; }

    public ShakeGenerator ShakeGenerator { get; private set; }

    #endregion

    #region MVP

    [SerializeField] private PlayerView playerView;
    public PlayerModel PlayerModel { get; private set; }
    public PlayerPresenter PlayerPresenter { get; private set; }

    #endregion

    #region StateMachine&Factory
    public StateMachine<Player> StateMachine {  get; private set; }

    public PlayerStateFactory PlayerStateFactory {  get; private set; }
    #endregion

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        EnemyDetector = GetComponentInChildren<EnemyDetector>();
        ComboSystem = GetComponentInChildren<ComboSystem>();
        InputHandler = GetComponent<InputHandler>();
        AnimationHandler = GetComponentInChildren<AnimationHandler>();
        Parameters = GetComponent<PlayerParameters>();

        SlashVFX = GetComponentInChildren<SlashVFX>();
        slashSoundFX = GetComponentInChildren<SlashSoundFX>();
        SkillsSFX = GetComponentInChildren<SkillsSFX>();
        rootMotion = GetComponentInChildren<RootMotion>();
        SwitchCameras = GetComponent<SwitchCameras>();
        SkillManager = GetComponent<SkillUIManager>();
        PlayerBlackboard = GetComponent<PlayerBlackboard>();
        UltimateSkill = GetComponentInChildren<UltimateSkill>();
        ShakeGenerator = GetComponentInChildren<ShakeGenerator>();

        PlayerModel = new PlayerModel(Parameters);
        PlayerPresenter = new PlayerPresenter(PlayerModel, playerView);

        // IMPORTANT: crea PlayerMovement e ImpulseSystem antes de crear estados
        PlayerHorizontalMovement = new PlayerHorizontalMovement(cameraTransform);
        PlayerVerticalMovement = new PlayerVerticalMovement();
        ImpulseSystem = new ImpulseSystem();

        // 2) ahora sí crea la StateFactory y los estados (que referencian PlayerMovement)
        StateMachine = new StateMachine<Player>();
        PlayerStateFactory = new PlayerStateFactory();
        PlayerStateFactory.InitializeState(this, StateMachine);

        // 3) resto de inicializaciones que dependan de estados
        SlashVFX.Initialize(this);
        slashSoundFX.Initialize(this);
        InputBuffer = new InputBuffer();
    }

    private void Start()
    {
        StateMachine.Initialize(PlayerStateFactory.IdleState);
    }

    private void OnEnable()
    {
        PlayerEvents.OnPlayerBlockEvent += PlayBlockAnimation;
        EventBus.OnEnemyHitEvent += IncreaseUltimateSkillValue;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerBlockEvent -= PlayBlockAnimation;
        EventBus.OnEnemyHitEvent -= IncreaseUltimateSkillValue;
    }

    void Update()
    {
        StateMachine.CurrentState.Update();

        if(Time.time >= PlayerBlackboard.LastTimeToStartRegenStamina + PlayerBlackboard.timeToStartRegenStamina)
        {
            PlayerModel.RestoreStamina(Parameters.regenStaminaValue * Time.deltaTime);
            Parameters.currentStamina = PlayerModel.CurrentStamina;
        }
    }

    public void TakeDamage(float damage, bool accessGotHit)
    {

        if (PlayerBlackboard.isInvulnerable) return;

        Parameters.currentHp -= damage;
        PlayerModel.UpdateHP(Parameters.currentHp);

        if (accessGotHit)
        {
            StateMachine.ChangeState(PlayerStateFactory.GotHitState);
        }
    }

    private void IncreaseUltimateSkillValue()
    {
        Parameters.currentUltimateSkillValue += Parameters.regenUltimateSkillValue;
        PlayerModel.UpdateUltimateSkillValue(Parameters.regenUltimateSkillValue);

        if(Parameters.currentUltimateSkillValue >= Parameters.maxUltimateSkillValue)
        {
            PlayerBlackboard.ultimateSkillAvailable = true;
        }
    }


    private void PlayBlockAnimation()
    {
        AnimationHandler.Play("Parry_Accept");
    }

    public void OnHit(Enemy attacker)
    {
        if (PlayerBlackboard.isInvulnerable) return;

        TakeDamage(attacker.EnemyCombatSystem.CurrentAttackData.damage, false);

        if (PlayerBlackboard.onlyTakeDamage) return;

        StateMachine.ChangeState(PlayerStateFactory.GotHitState);
    }


}
