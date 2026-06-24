using System.Collections;
using UnityEngine;

//Known issue:
//AttackState may exit prematurely to IdleState during the first few frames.

//Temporary fix:
//The state remains locked until normalizedTime >= 0.2f.

//Likely cause:
//AnimatorStateInfo/normalizedTime can return unreliable values
//during the transition into the attack animation.

public class PlayerGroundAttackState : PlayerGroundState
{
    public PlayerGroundAttackState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.LockState();
        playerBlackboard.isAttacking = true;
        playerBlackboard.applyMovementCorrection = true;

        playerBlackboard.canMove = false;
        playerBlackboard.canDodge = false;

        facingSystem.FaceTarget(targetingSystem.CurrentTarget);

        targetingSystem.LockTarget();

        comboSystem.PlayAttack();

        ApplyTargetAttraction(targetingSystem.CurrentTarget);

        PlayerEvents.PlayerCombatEnterEvent();
    }

    public override void Exit()
    {
        base.Exit();

        entity.HitBox.DeactivateWeaponHitBox();
        playerBlackboard.applyMovementCorrection = false;
        comboSystem.ResetCombo();
        targetingSystem.UnlockTarget();

        playerSkills.CanUseOtherSkills(true);

        playerBlackboard.canMove = true;
        playerBlackboard.isAttacking = false;
        entity.rootMotion.DeactivateRootMotion();

        if(!playerCombatContext.ThereAreEnemies)
        {
            PlayerEvents.PlayerCombatExitEvent();
        }

        entity.SlashSoundFX.StopSFX();
    }

    public override void Update()
    {
        base.Update();

        if (inputHandler.ParryInputHeld) return;

        if (inputHandler.LightAttackButtonPressed)
        {
            if (comboSystem.CurrentAttackNode == null) return;
            entity.InputBuffer.RegisterInput(comboSystem.CurrentAttackNode.inputBuffer, AttackType.Light);
        }
        else if (inputHandler.HeavyAttackButtonPressed)
        {
            if (comboSystem.CurrentAttackNode == null) return;
            entity.InputBuffer.RegisterInput(comboSystem.CurrentAttackNode.inputBuffer, AttackType.Heavy);
        }

        if (animationHandler.NormalizedTime() >= .2f)
        {
            stateMachine.UnlockState();
            playerBlackboard.canDodge = true;
        }

        entity.InputBuffer.CountDown(Time.deltaTime);

        if (CanQueueNextAttack())
        {
            if (entity.InputBuffer.attackType == AttackType.Light) 
            {
                ProcessBufferedAttack(true);
                PlayerEvents.AttackButtonPressed();
            }
            else if (entity.InputBuffer.attackType == AttackType.Heavy)
            {
                ProcessBufferedAttack(false);
                PlayerEvents.AttackButtonPressed();
            }
        }

        //This condition is intentionally gated by LockState to prevent premature exits
        //during the first frames of the attack animation.
        if (!entity.InputBuffer.HasInput && comboSystem.CurrentAttackNode != null &&
             animationHandler.IsPlaying(comboSystem.CurrentAttackNode.attackName) &&
             animationHandler.NormalizedTime() >= comboSystem.CurrentAttackNode.exitToIdleTime)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
            return;
        }
    }

    private bool CanQueueNextAttack()
    {
        if (comboSystem.CurrentAttackNode == null)
        {
            return false;
        }

        bool inComboWindow = animationHandler.IsPlaying(comboSystem.CurrentAttackNode.attackName) && comboSystem.IsInComboWindow();

        return inComboWindow || comboSystem.CanRestartCombo();
    }

    private void ProcessBufferedAttack(bool isLightAttack)
    {

        if (targetingSystem.IsCurrentTargetLocked)
        {
            facingSystem.FaceTarget(targetingSystem.LockedTarget);
        }
        else
        {
            facingSystem.FaceTarget(playerCombatContext.CurrentTarget);
            targetingSystem.LockTarget();
        }

        comboSystem.NextAttackNode(isLightAttack);

        comboSystem.PlayNextAttack();

        entity.InputBuffer.ClearInput();

        ApplyTargetAttraction(targetingSystem.CurrentTarget);

        if (entity.lockOnTargetBlackboard.IsLockOnTargetActive) return;

        RotatePlayer();
    }

    private void ApplyTargetAttraction(Transform target)
    {
        if (target == null) return;

        float distance = Vector3.Distance(target.position, entity.transform.position);

        if (distance > playerBlackboard.minTargetAttractionDistance && distance < playerBlackboard.maxTargetAttractionDistance)
        {
            Vector3 dir = (target.position - entity.transform.position).normalized;

            Vector3 end = target.position - dir * 2;

            facingSystem.FaceTarget(target);

            entity.MotionSystem.StartMotion(
                entity.transform.position,
                end,
                .25f,
                entity.PlayerCurves.targetAttractionCurve);
        }
    }

    private void RotatePlayer()
    {
        Vector3 cameraForward = entity.cameraTransform.forward;
        Vector3 cameraRight = entity.cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 currentInput = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 desiredDir;

        if (currentInput != Vector3.zero)
        {
            desiredDir = (cameraForward * currentInput.z + cameraRight * currentInput.x).normalized;
        }
        else
        {
            desiredDir = entity.transform.forward;
        }

        entity.transform.rotation = Quaternion.LookRotation(desiredDir);
    }
}
