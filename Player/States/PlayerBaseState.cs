using System.Collections;
using UnityEngine;

public class PlayerBaseState : State<Player>
{
    protected PlayerParameters playerParameters;
    protected CharacterController characterController;
    protected PlayerStateFactory playerStateFactory;
    protected InputHandler inputHandler;
    protected AnimationHandler animationHandler;
    protected ComboSystem comboSystem;
    protected PlayerBlackboard playerBlackboard;
    protected EnemyDetector enemyDetector;
    protected ParrySystem parrySystem;
    protected PlayerHorizontalMovement playerHorizontalMovement;
    protected PlayerVerticalMovement playerVerticalMovement;
    protected PlayerSkills playerSkills;
    protected PlayerCombatContext playerCombatContext;
    protected FacingSystem facingSystem;
    protected ImpulseSystem impulseSystem;
    protected GroundDetection groundDetection;
    protected TargetingSystem targetingSystem;
    protected RootMotion rootMotion;

    protected Vector3 smoothInput;

    protected float xInput, zInput;

    public PlayerBaseState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
        playerParameters = _player.Parameters;
        playerStateFactory = _player.PlayerStateFactory;
        inputHandler = _player.InputHandler;
        animationHandler = _player.AnimationHandler;
        comboSystem = _player.ComboSystem;
        characterController = _player.CharacterController;
        playerBlackboard = _player.PlayerBlackboard;
        enemyDetector = _player.EnemyDetector;
        parrySystem = _player.ParrySystem;
        playerHorizontalMovement = _player.PlayerHorizontalMovement;
        playerVerticalMovement = _player.PlayerVerticalMovement;
        playerSkills = _player.PlayerSkills;
        playerCombatContext = _player.PlayerCombatContext;
        facingSystem = _player.FacingSystem;
        impulseSystem = _player.ImpulseSystem;
        groundDetection = _player.GroundDetection;
        targetingSystem = _player.TargetingSystem;
        rootMotion = _player.rootMotion;
    }

    public override void Enter()
    {
        playerBlackboard.canDodge = true;
    }

    public override void Exit()
    {
        entity.WeaponTrailEffect.StopTrail(0);
    }

    public override void Update()   
    {
        if (stateMachine.CurrentState == playerStateFactory.IntroState) return;
 
        entity.TargetingSystem.UpdateTarget();
        entity.GroundDetection.CheckGround();
        entity.rootMotion.UpdateRootMotion();
        entity.RegenSystem.Update();

        if (IsDead()) return;

        if (playerBlackboard.canMove)
        {
            xInput = Input.GetAxisRaw("Horizontal");
            zInput = Input.GetAxisRaw("Vertical");

            playerHorizontalMovement.CalculateCameraRelativeMovement(xInput, zInput);
        }
        else
        {
            xInput = 0;
            zInput = 0;
            playerHorizontalMovement.Reset();
        }

        playerVerticalMovement.ApplyGravity(groundDetection.isGrounded);

        entity.MovementHandler.HandleMovement();
    }

    protected void PerformFirstAttack()
    {
        if (playerBlackboard.isAttacking) return;

        if (entity.DodgeSystem.sucessfulPerfectDodge)
        {
            if (inputHandler.LightAttackButtonPressed)
            {
                entity.InputBuffer.RegisterInput(.25f, AttackType.Light);
            }

            if(playerBlackboard.canAttack && entity.InputBuffer.HasInput)
            {
                stateMachine.ChangeState(playerStateFactory.PerfectDodgeAttackState);
            }

            return;
        }

        if (groundDetection.isGrounded)
        {
            if (playerBlackboard.canAttack && inputHandler.LightAttackButtonPressed)
            {
                playerBlackboard.groundChargeStartTime = Time.time;

                comboSystem.IsLightAttackNode(true);
                stateMachine.ChangeState(playerStateFactory.GroundAttackState);
                return;

            }
            else if (playerBlackboard.canAttack && inputHandler.HeavyAttackButtonPressed)
            {
                playerBlackboard.groundChargeStartTime = Time.time;

                comboSystem.IsLightAttackNode(false);
                stateMachine.ChangeState(playerStateFactory.GroundAttackState);
                return;
            }
        }
    }

    private bool IsDead()
    {
        if (playerBlackboard.isPlayerDead) return true;

        if (entity.PlayerModel.CurrentHP > 0) return false;

        playerBlackboard.isPlayerDead = true;
        stateMachine.ChangeState(playerStateFactory.DeathState);
        return true;
    }
}
