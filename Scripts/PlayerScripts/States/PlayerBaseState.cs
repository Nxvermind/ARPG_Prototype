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
    protected SkillUIManager skillManager;
    protected PlayerBlackboard playerBlackboard;
    protected EnemyDetector enemyDetector;
    protected ParrySystem parrySystem;
    protected PlayerHorizontalMovement playerHorizontalMovement;
    protected PlayerVerticalMovement playerVerticalMovement;

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
        skillManager = _player.SkillManager;
        playerBlackboard = _player.PlayerBlackboard;
        enemyDetector = _player.EnemyDetector;
        parrySystem = _player.parrySystem;
        playerHorizontalMovement = _player.PlayerHorizontalMovement;
        playerVerticalMovement = _player.PlayerVerticalMovement;
    }

    public override void Enter()
    {
        playerBlackboard.canDodge = true;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        //Debug.Log($"im in {stateMachine.CurrentState}");

        if (stateMachine.CurrentState == playerStateFactory.IntroState) return;

        if (entity.introScript.optionsShowed)
        {
            playerBlackboard.canChargeAttack = false;
            playerBlackboard.canAttack = false;
            return;
        }

        if (entity.playerRespawned)
        {
            entity.playerRespawned = false;
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }

        if (playerBlackboard.isPlayerDead)
        {
            return;
        }


        if (ShouldUpdateInput || playerBlackboard.canMove)
        {
            xInput = Input.GetAxisRaw("Horizontal");
            zInput = Input.GetAxisRaw("Vertical");
        }

        if (entity.Parameters.currentHp <= 0)
        {
            playerBlackboard.isPlayerDead = true;
            stateMachine.ChangeState(playerStateFactory.DeathState);
        }

        PerformFirstAttack();

        playerHorizontalMovement.CalculateCameraRelativeMovement(xInput, zInput);
        playerVerticalMovement.ApplyGravity(characterController.isGrounded);
        HandleMovement();


        if (inputHandler.UltimateSkillButton && playerBlackboard.ultimateSkillAvailable && characterController.isGrounded)
        {
            PlayerEvents.UltimateSkillCalled();
            playerBlackboard.ultimateSkillAvailable = false;
            stateMachine.ChangeState(playerStateFactory.UltimateSkillState);
        }

        if (enemyDetector.NumOfEnemiesDetected == 0)
        {
            if (!playerBlackboard.firstAttack) return;

            if (Time.time - playerBlackboard.lastTimeAttacked >= entity.canvasVanish.canvasVanishDelay && !entity.canvasVanish.isCanvasVanished)
            {
                entity.canvasVanish.isCanvasVanished = true;
                entity.canvasVanish.StartVanishing();
            }
            else if (Time.time - playerBlackboard.lastTimeAttacked < entity.canvasVanish.canvasVanishDelay && entity.canvasVanish.isCanvasVanished)
            {
                entity.canvasVanish.isCanvasVanished = false;
                entity.canvasVanish.ReverseVanishing();
            }
        }
        else
        {
            playerBlackboard.lastTimeAttacked = Time.time;

            if (entity.canvasVanish.isCanvasVanished)
            {
                entity.canvasVanish.isCanvasVanished = false;
                entity.canvasVanish.ReverseVanishing();
            }
        }
    }
    private void PerformFirstAttack()
    {
        if (playerBlackboard.isAttacking) return;

        if (characterController.isGrounded)
        {
            if (inputHandler.LightAttackButtonPressed && playerBlackboard.canAttack)
            {
                playerBlackboard.firstAttack = true;

                playerBlackboard.lastTimeAttacked = Time.time;

                playerBlackboard.chargeStartTime = Time.time;

                comboSystem.IsLightAttackNode(true);

                stateMachine.ChangeState(playerStateFactory.GroundAttackState);

            }
            else if (inputHandler.HeavyAttackButtonPressed && playerBlackboard.canAttack)
            {
                playerBlackboard.firstAttack = true;
                playerBlackboard.lastTimeAttacked = Time.time;

                playerBlackboard.chargeStartTime = Time.time;

                comboSystem.IsLightAttackNode(false);

                stateMachine.ChangeState(playerStateFactory.GroundAttackState);

            }
        }
        else
        {
            if(inputHandler.LightAttackButtonPressed && playerBlackboard.canAttack)
            {
                playerBlackboard.firstAttack = true;
                playerBlackboard.lastTimeAttacked = Time.time;
                comboSystem.AirAttack();
                stateMachine.ChangeState(playerStateFactory.AirAttackState);

            }
        }

    }

    private void HandleMovement()
    {
        Vector3 horizontal = playerHorizontalMovement.GetHorizontalMovement();

        Vector3 verticalMovement = playerVerticalMovement.GetVerticalMovement();

        Vector3 impulse = entity.ImpulseSystem.ConsumeImpulse();

        Vector3 final = horizontal + verticalMovement + impulse;

        characterController.Move(final * Time.deltaTime);
    }

    public void RotatePlayerInDesiredDirection(Transform player, Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * 7f);
    }


    protected virtual bool ShouldUpdateInput => true;
}
