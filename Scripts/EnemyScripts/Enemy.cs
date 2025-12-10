using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    None,
    Melee,
    Range,
    Elite
}
public class Enemy : MonoBehaviour
{
    #region Showed in Inspector

    public Transform target;

    [SerializeField] private MonoBehaviour attackNodeProvider;

    public ICurrentAttackNodeProvider attackProvider;

    public EnemyAttackCoordinator coordinator;

    public EnemyType enemyType;

    #endregion

    #region Components

    public AttackNode LastAttackNode { get; set; }

    public AnimationHandler AnimationHandler { get; private set; }

    public NavMeshAgent Agent { get; set; }

    public Rigidbody rb { get; private set; }

    //Collider for rigidbody
    public CapsuleCollider CapsuleCollider { get; private set; }

    public EnemyParameters EnemyParameters { get; private set; }

    public EnemyBlackboard EnemyBlackboard { get; private set; }

    public EnemyCombatSystem EnemyCombatSystem { get; private set; }

    public EnemyParryStatus EnemyParryStatus { get; private set; }

    public EnemyVision EnemyVision { get; private set; }

    public StaggerSystem StaggerSystem { get; private set; }

    public EnemyGroundDetection EnemyGroundDetection { get; private set; }

    public EnemyHitBox EnemyHitBox { get; private set; }

    public EnemySearchSystem EnemySearchSystem { get; private set; }

    public EnemyCombatStatus EnemyCombatStatus { get; private set; }

    #endregion

    #region CanvasRelated
    public Canvas CanvasHUD { get; private set; }

    public CanvasVanish CanvasVanish { get; private set; }
    #endregion

    #region StateMachine

    protected StateMachine<Enemy> stateMachine;

    public EnemyStateFactory EnemyStateFactory { get; set; }
    #endregion

    public virtual void Awake()
    {
        stateMachine = new StateMachine<Enemy>();

        EnemyBlackboard = GetComponent<EnemyBlackboard>();
        EnemyVision = GetComponent<EnemyVision>();
        Agent = GetComponent<NavMeshAgent>();
        EnemyParameters = GetComponent<EnemyParameters>();
        AnimationHandler = GetComponent<AnimationHandler>();
        EnemyGroundDetection = GetComponentInChildren<EnemyGroundDetection>();
        StaggerSystem = GetComponent<StaggerSystem>();

        EnemySearchSystem = GetComponent<EnemySearchSystem>();
        EnemyCombatStatus = GetComponent<EnemyCombatStatus>();
        EnemyParryStatus = GetComponent<EnemyParryStatus>();

        rb = GetComponent<Rigidbody>();
        CapsuleCollider = GetComponent<CapsuleCollider>();

        attackProvider = attackNodeProvider as ICurrentAttackNodeProvider;

        EnemyCombatSystem = GetComponent<EnemyCombatSystem>();

        EnemyHitBox = GetComponent<EnemyHitBox>();

        CanvasHUD = GetComponentInChildren<Canvas>();
        CanvasVanish = GetComponentInChildren<CanvasVanish>();

    }

    public virtual void Start()
    {
        EnemyBlackboard.isDead = false;
        
    }

    public virtual void Update()
    {
        stateMachine.CurrentState?.Update();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<UltimateSkill>(out var skill))
        {
            EnemyParameters.currentHp -= skill.Damage();    
            EnemyBlackboard.gotHitByUltimateSkill = true;
        }

    }

    public virtual void UpdateSpecificLogic()
    {

    }

    public bool IsPlayerInRange(float range)
    {
        Vector3 distance = target.position - transform.position;

        float sqrDistance = distance.sqrMagnitude;

        float sqrDistanceToPlayer = range * range;

        return sqrDistance <= sqrDistanceToPlayer;
    }

}
