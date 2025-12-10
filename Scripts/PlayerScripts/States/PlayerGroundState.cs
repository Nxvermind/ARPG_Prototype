using UnityEngine;

public class PlayerGroundState : PlayerBaseState
{
    protected bool inParryState;

    public PlayerGroundState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.CurrentState == playerStateFactory.ExecutionState) return;

        if (!characterController.isGrounded) return;

        if (playerParameters.currentStamina > 0)
        {
            playerBlackboard.canRun = true;
            playerBlackboard.canDodge = true;
        }
        else
        {
            playerBlackboard.canRun = false;
            playerBlackboard.canDodge = false;
        }

        if (inputHandler.LightAttackButtonHeld && playerBlackboard.canChargeAttack)
        {
            float heldTime = Time.time - playerBlackboard.chargeStartTime;

            if (heldTime >= playerBlackboard.chargeThreshold)
            {
                stateMachine.ChangeState(playerStateFactory.GroundChargeAttackState);
                return;
            }
        }

        if (entity.Parameters.currentPostureValue <= 0)
        {
            stateMachine.ChangeState(playerStateFactory.PostureBrokenState);
        }

        Execution();

        if (inputHandler.SkillDashButton && skillManager.data["DashSkill"].isSkillReady)
        {
            stateMachine.ChangeState(playerStateFactory.SkillDash);
            skillManager.StartSkillCooldown("DashSkill");
        }
        else if (inputHandler.SkillDashButton && !skillManager.data["DashSkill"].isSkillReady)
        {
            SkillOnCooldownSFX.instance.PlaySFX();
        }

        if (inputHandler.ParryInputHeld && !inParryState && !animationHandler.IsPlaying("Parry_End") && !playerParameters.postureBroken)
        {
            if (playerBlackboard.canParry)
            {
                //Debug.Log("going to parryState");
                stateMachine.ChangeState(playerStateFactory.ParryState);
            }
        }

        Dodge();

        //esto lo uso en el blend tree del lock on target
        SmoothInputMovement();

        if (SwitchCameras.IsLockOnTargetCameraActive)
        {
            if (!playerBlackboard.isRunning)
            {
                animationHandler.Anim.SetFloat("Horizontal", smoothInput.x);
                animationHandler.Anim.SetFloat("Vertical", smoothInput.z);
                Vector3 targetPosition = LockOnTargetCamera.CurrentLockOnTarget.position;

                // Mantener la altura del entity para bloquear rotación en X
                targetPosition.y = entity.transform.position.y;

                entity.transform.LookAt(targetPosition);
            }
            else
            {
                RotatePlayerInDesiredDirection(entity.transform, playerHorizontalMovement.GetRawMoveDirection());

                animationHandler.Anim.SetFloat("Horizontal", xInput);
                animationHandler.Anim.SetFloat("Vertical", zInput);
            }

        }
        else
        {
            RotatePlayerInDesiredDirection(entity.transform, playerHorizontalMovement.GetRawMoveDirection());
        }
    }

    private void SmoothInputMovement()
    {
        smoothInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }


    private void Dodge()
    {
        if (stateMachine.CurrentState == playerStateFactory.DodgeState || stateMachine.CurrentState == playerStateFactory.SkillDash) return;

        if (inputHandler.SpaceKey && (playerBlackboard.firstDodge || Time.time >= playerParameters.LastDodgeTime + playerParameters.dodgeCooldown))
        {
            if (playerBlackboard.canDodge)
            {
                playerParameters.LastDodgeTime = Time.time;
                stateMachine.ChangeState(playerStateFactory.DodgeState);
                playerBlackboard.firstDodge = false;
            }
        }
    }

    private void Execution()
    {
        var execEnemy = entity.executionManager.currentExecutableEnemy;

        if (execEnemy == null) return;

        if (inputHandler.ExecutionButton && !execEnemy.EnemyBlackboard.wasExecuted && execEnemy.EnemyBlackboard.canExecuteThisEnemy)
        {
            execEnemy.EnemyBlackboard.isBeingExecuted = true;
            execEnemy.EnemyBlackboard.canExecuteThisEnemy = false;

            stateMachine.ChangeState(playerStateFactory.ExecutionState);
            EventBus.StartExecution();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
