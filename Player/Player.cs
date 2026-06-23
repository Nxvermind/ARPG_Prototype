using INab.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IPlayerMovementProvider
{
    #region Showed In Inspector

    public Transform cameraTransform;

    public Transform cameraForwardReference;

    public Transform executionPoint;

    public Collider hurtBox;

    public Transform mesh;

    public LockOnTargetBlackboard lockOnTargetBlackboard;

    public ExecutionBlackboard executionBlackboard;

    public LockOnTargetSystem lockOnTargetSystem;

    public ExecutionSystem executionSystem;

    #endregion

    #region PlayerComponents

    public PlayerParameters Parameters { get; private set; }

    public AnimationHandler AnimationHandler { get; private set; }

    public CharacterController CharacterController { get; private set; }

    public SlashSoundFX SlashSoundFX { get; private set; }

    public SkillsSFX SkillsSFX { get; private set; }

    [HideInInspector] public RootMotion rootMotion;

    public PlayerBlackboard PlayerBlackboard { get; private set; }

    public UltimateSkill UltimateSkill { get; private set; }

    public InputBuffer InputBuffer { get; private set; }

    public PlayerHorizontalMovement PlayerHorizontalMovement { get; private set; }

    public PlayerVerticalMovement PlayerVerticalMovement { get; private set; }

    public ShakeGenerator ShakeGenerator { get; private set; }

    public PlayerSkills PlayerSkills { get; private set; }

    public PlayerCombatContext PlayerCombatContext { get; private set; }

    public PlayerMovementHandler MovementHandler { get; private set; }

    public GroundDetection GroundDetection { get; private set; }

    public PlayerRegenSystem RegenSystem { get; private set; }

    public MotionSystem MotionSystem { get; private set; }

    public PlayerCurves PlayerCurves { get; private set; }

    public PlayerHitBox HitBox { get; private set; }

    public DodgeSystem DodgeSystem { get; private set; }

    #endregion

    #region Systems

    public EnemyDetector EnemyDetector { get; private set; }
    public ComboSystem ComboSystem { get; private set; }
    public InputHandler InputHandler { get; private set; }
    public ParrySystem ParrySystem { get; private set; }
    public ImpulseSystem ImpulseSystem { get; private set; }
    public FacingSystem FacingSystem { get; private set; }  
    public TargetingSystem TargetingSystem { get; private set; }

    #endregion

    #region MVP

    [Header("MVP")]
    [SerializeField] private PlayerView playerView;
    public PlayerModel PlayerModel { get; private set; }
    public PlayerPresenter PlayerPresenter { get; private set; }

    #endregion

    #region StateMachine&Factory
    public StateMachine<Player> StateMachine {  get; private set; }

    public PlayerStateFactory PlayerStateFactory {  get; private set; }


    #endregion

    private int playerLayer;
    private int ghostLayer;

    public WeaponTrailEffect WeaponTrailEffect { get; private set; }

    public Vector3 GetMovement => PlayerHorizontalMovement.GetRawMoveDirection();

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        EnemyDetector = GetComponentInChildren<EnemyDetector>();
        TargetingSystem = GetComponentInChildren<TargetingSystem>();
        ComboSystem = GetComponentInChildren<ComboSystem>();
        InputHandler = new InputHandler();
        AnimationHandler = GetComponentInChildren<AnimationHandler>();
        Parameters = GetComponent<PlayerParameters>();
        ParrySystem = GetComponentInChildren<ParrySystem>();
        PlayerCombatContext = GetComponent<PlayerCombatContext>();
        GroundDetection = GetComponentInChildren<GroundDetection>();
        FacingSystem = new FacingSystem(transform);

        PlayerModel = new PlayerModel(Parameters);
        PlayerPresenter = new PlayerPresenter(PlayerModel, playerView);

        SlashSoundFX = GetComponentInChildren<SlashSoundFX>();
        SkillsSFX = GetComponentInChildren<SkillsSFX>();
        rootMotion = GetComponentInChildren<RootMotion>();
        PlayerBlackboard = GetComponent<PlayerBlackboard>();
        UltimateSkill = GetComponentInChildren<UltimateSkill>();
        ShakeGenerator = GetComponentInChildren<ShakeGenerator>();
        PlayerSkills = GetComponentInChildren<PlayerSkills>();
        PlayerCurves = GetComponent<PlayerCurves>();
        HitBox = GetComponentInChildren<PlayerHitBox>();
        DodgeSystem = GetComponentInChildren<DodgeSystem>();

        RegenSystem = new PlayerRegenSystem(PlayerBlackboard, PlayerModel, Parameters);
        PlayerHorizontalMovement = new PlayerHorizontalMovement(cameraTransform);
        PlayerVerticalMovement = new PlayerVerticalMovement();
        ImpulseSystem = new ImpulseSystem();
        MotionSystem = new MotionSystem();
        InputBuffer = new InputBuffer();

        MovementHandler = new PlayerMovementHandler(PlayerHorizontalMovement, PlayerVerticalMovement, 
            ImpulseSystem, CharacterController, PlayerBlackboard, MotionSystem);

        StateMachine = new StateMachine<Player>();
        PlayerStateFactory = new PlayerStateFactory();
        PlayerStateFactory.InitializeState(this, StateMachine);

        SlashSoundFX.Initialize(PlayerCombatContext);


        playerLayer = LayerMask.NameToLayer("Player");
        ghostLayer = LayerMask.NameToLayer("Ghost");
        WeaponTrailEffect = GetComponentInChildren<WeaponTrailEffect>();
    }

    private void Start()
    {
        PlayerVerticalMovement.EnableGravity(true);
        StateMachine.Initialize(PlayerStateFactory.IntroState);
    }

    private void OnEnable()
    {
        EventBus.OnEnemyHitEvent += EnemyHitEvent;
    }

    private void OnDisable()
    {
        EventBus.OnEnemyHitEvent -= EnemyHitEvent;
    }

    void Update()
    {
        InputHandler.CheckInputs();

        StateMachine.CurrentState?.Update();
    }

    private void EnemyHitEvent()
    {
        IncreaseUltimateSkillValue();
        Lifesteal();
    }

    private void IncreaseUltimateSkillValue()
    {
        if (PlayerModel.CurrentUltimateSkillValue >= Parameters.maxUltimateSkillValue) return;

        PlayerModel.IncreaseUltimateSkillValue(Parameters.regenUltimateSkillValue);

        if (PlayerModel.CurrentUltimateSkillValue >= Parameters.maxUltimateSkillValue)
        {
            PlayerSkills.UltimateSkill.UltiSkillReady();
        }
    }

    private void Lifesteal()
    {
        if (PlayerModel.CurrentHP >= PlayerModel.MaxHP) return;

        PlayerModel.IncreaseCurrentHP(Parameters.lifesteal);
    }

    public void EnableGhostMode()
    {
        gameObject.layer = ghostLayer;
    }

    public void DisableGhostMode()
    {
        gameObject.layer = playerLayer;
    }
}
