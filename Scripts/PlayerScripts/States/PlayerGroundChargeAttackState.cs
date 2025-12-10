using System.Collections;
using UnityEngine;

public class PlayerGroundChargeAttackState : PlayerGroundState
{
    private bool elevated;

    public PlayerGroundChargeAttackState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerVerticalMovement.EnableGravity(false);

        playerBlackboard.canAttack = false;
        enemyDetector.RotateToEnemy();   
        animationHandler.Play("ChargeAttack");
        comboSystem.ChargeAttack();

        playerBlackboard.canParry = false;
    }

    public override void Exit()
    {
        base.Exit();

        playerBlackboard.canAttack = true;

        elevated = false;

        entity.ImpulseSystem.Reset();
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying("ChargeAttack") && animationHandler.NormalizedTime() >= .45f && !elevated)
        {
            elevated = true;

            entity.ImpulseSystem.AddVerticalImpulse(50);
        }

        if (animationHandler.IsPlaying("ChargeAttack") && animationHandler.NormalizedTime() >= .9f)
        {
            stateMachine.ChangeState(playerStateFactory.FallingState);
        }
    }

    protected override bool ShouldUpdateInput => false;
}
