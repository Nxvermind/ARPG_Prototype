using System.Collections;
using UnityEngine;

public class PlayerGroundAttackState : PlayerGroundState
{
    public PlayerGroundAttackState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        entity.PlayerBlackboard.isAttacking = true;

        if (stateMachine.PreviousState == playerStateFactory.ParryState)
        {
            comboSystem.ParryAttack();
            animationHandler.CrossFade("Parry_Attack", .1f);
        }
        else
        {
            comboSystem.PlayAttack();
        }

        playerBlackboard.isAttacking = true;
        enemyDetector.RotateToEnemy();
    }

    public override void Exit()
    {
        base.Exit();
        comboSystem.ResetCombo();

        playerBlackboard.isAttacking = false;
        entity.SlashVFX.StopSlashAndCoroutine();
        animationHandler.Anim.applyRootMotion = false;
    }

    public override void Update()
    {
        base.Update();

        if (inputHandler.ParryInputHeld)
        {
            return;
        }

        if (animationHandler.NormalizedTime() >= .8f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
            return;
        }

        if (inputHandler.LightAttackButtonPressed)
        {
            entity.InputBuffer.RegisterInput(comboSystem.CurrentAttackNode.inputBuffer, AttackType.Light);
            playerBlackboard.chargeStartTime = Time.time;
        }
        else if (inputHandler.HeavyAttackButtonPressed)
        {
            entity.InputBuffer.RegisterInput(comboSystem.CurrentAttackNode.inputBuffer, AttackType.Heavy);

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
    }

    private bool CanQueueNextAttack()
    {
        if (comboSystem.CurrentAttackNode == null)
        {
            return false;
        }

        bool inComboWindow = comboSystem.IsInComboWindow() && animationHandler.IsPlaying(comboSystem.CurrentAttackNode.attackName);

        bool canRestart = (animationHandler.NormalizedTime() >= comboSystem.CurrentAttackNode.timeToLetRestartCombo)
                          && comboSystem.CurrentAttackNode.noNextAttack;

        return inComboWindow || canRestart;
    }

    private void ProcessBufferedAttack(bool isLightAttack)
    {
        playerBlackboard.lastTimeAttacked = Time.time;
        comboSystem.NextAttackNode(isLightAttack);
        enemyDetector.RotateToEnemy();
        comboSystem.PlayNextAttack();
        entity.InputBuffer.ClearInput();
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        Vector3 cameraForward = entity.cameraTransform.forward;
        Vector3 cameraRight = entity.cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Tomar input actual en el momento del dodge
        Vector3 currentInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 desiredDir;

        if (currentInput != Vector3.zero)
        {
            desiredDir = (cameraForward * currentInput.z + cameraRight * currentInput.x).normalized;
        }
        else
        {
            // Si no hay input actual, usar forward actual del personaje
            desiredDir = entity.transform.forward;
        }

        // Rotar hacia la dirección
        entity.transform.rotation = Quaternion.LookRotation(desiredDir);
    }

    protected override bool ShouldUpdateInput => false;
}
