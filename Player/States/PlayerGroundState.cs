using Unity.Cinemachine;
using UnityEngine;

public class PlayerGroundState : PlayerBaseState
{
    protected bool inParryState;

    protected bool alreadyExecutingEnemy;

    public PlayerGroundState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.CurrentState == playerStateFactory.ExecutionState) return;

        if (!groundDetection.isGrounded) return;

        if (playerCombatContext.IsUltimateSkillExecuting) return;

        if (entity.PlayerModel.CurrentStamina > 0)
        {
            playerBlackboard.canRun = true;
        }
        else
        {
            playerBlackboard.canRun = false;
            playerBlackboard.canDodge = false;
        }

        if(Time.time >= playerParameters.LastDodgeTime + playerParameters.dodgeCooldown)
        {
            playerBlackboard.dodgeAvailable = true;
        }

        if (entity.executionBlackboard.IsExecutingEnemy && !alreadyExecutingEnemy)
        {
            stateMachine.ChangeState(playerStateFactory.ExecutionState);
        }

        HandleParry();

        Dodge();

        //Used by the lock-on target blend tree
        SmoothInputMovement();

        if (entity.lockOnTargetBlackboard.IsLockOnTargetActive)
        {
            if (!playerBlackboard.isRunning)
            {
                animationHandler.Anim.SetFloat("Horizontal", smoothInput.x);
                animationHandler.Anim.SetFloat("Vertical", smoothInput.z);

                if (!playerBlackboard.stopLookingAtLockOnTarget)
                {                  
                    facingSystem.RotateTowardsDirection(
                        DirectionUtility.DirectionToTarget(entity.transform, LockOnTargetLogic.CurrentLockOnTarget), 
                        12);
                }
            }
            else
            {
                facingSystem.RotateTowardsDirection(playerHorizontalMovement.GetRawMoveDirection(), 7);

                animationHandler.Anim.SetFloat("Horizontal", xInput);
                animationHandler.Anim.SetFloat("Vertical", zInput);
            }

        }
        else
        {
            facingSystem.RotateTowardsDirection(playerHorizontalMovement.GetRawMoveDirection(), 12);
        }

        #region Skills

        if (playerSkills.CanUseSkills)
        {
            if (inputHandler.SkillDashButton && playerSkills.DashSkill.IsSkillReady)
            {
                stateMachine.ChangeState(playerStateFactory.DashSkillState);
            }

            if (inputHandler.TeleportSkillButton)
            {
                playerSkills.TeleportSkill.TryExecuteTeleportSkill();
            }

            if (inputHandler.UltimateSkillButton && playerSkills.UltimateSkill.IsSkillReady)
            {
                stateMachine.ChangeState(playerStateFactory.UltimateSkillState);
            }
        }

        #endregion

        if (inputHandler.LockOnTargetButton)
        {
            entity.lockOnTargetSystem.ToggleLockOnTarget();
        }

        if (inputHandler.ExecutionButton)
        {
            entity.executionSystem.TryExecution();
        }
    }

    private void SmoothInputMovement()
    {
        smoothInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void HandleParry()
    {
        if (inputHandler.ParryInputHeld && !inParryState && !animationHandler.IsPlaying("Parry_End") && !playerParameters.postureBroken)
        {
            if (playerBlackboard.canParry)
            {
                stateMachine.UnlockState();
                stateMachine.ChangeState(playerStateFactory.ParryState);
            }
        }
    }
    private void Dodge()
    {
        if (inputHandler.SpaceKey && (playerBlackboard.firstDodge || playerBlackboard.dodgeAvailable))
        {
            if (playerBlackboard.canDodge && entity.PlayerModel.CurrentStamina > 0)
            {
                stateMachine.ChangeState(playerStateFactory.DodgeState);
                playerBlackboard.firstDodge = false;
            }
        }
    }
}
