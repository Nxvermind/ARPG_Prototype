using UnityEngine;

public class PlayerAirAttackState : PlayerAirState
{
    public PlayerAirAttackState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        comboSystem.PlayAttack();
        playerBlackboard.isAttacking = true;

        playerVerticalMovement.EnableGravity(false);
        playerVerticalMovement.ZeroVerticalVelocity();
    }

    public override void Exit()
    {
        base.Exit();
        playerBlackboard.isAttacking = false;

    }

    public override void Update()
    {
        base.Update();


        if (inputHandler.LightAttackButtonPressed)
        {
            entity.InputBuffer.RegisterInput(comboSystem.CurrentAttackNode.inputBuffer, AttackType.Light);
            playerBlackboard.chargeStartTime = Time.time;

        }
        entity.InputBuffer.CountDown(Time.deltaTime);

        if (CanQueueNextAttack())
        {
            if (entity.InputBuffer.attackType == AttackType.Light)
            {
                ProcessBufferedAttack(true);
            }
        }

        if (animationHandler.IsPlaying(comboSystem.CurrentAttackNode.attackName) && animationHandler.NormalizedTime() >= .9f)
        {
            stateMachine.ChangeState(playerStateFactory.FallingState);
        }
    }

    private bool CanQueueNextAttack()
    {
        if (comboSystem.CurrentAttackNode == null)
        {
            return false;
        }

        bool inComboWindow = comboSystem.IsInComboWindow() && animationHandler.IsPlaying(comboSystem.CurrentAttackNode.attackName);

        return inComboWindow;
    }

    private void ProcessBufferedAttack(bool isLight)
    {
        playerBlackboard.lastTimeAttacked = Time.time;
        comboSystem.NextAttackNode(isLight);
        enemyDetector.RotateToEnemy();
        comboSystem.PlayNextAttack();
        entity.InputBuffer.ClearInput();
    }
    protected override bool ShouldUpdateInput => false;

}


