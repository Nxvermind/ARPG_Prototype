using UnityEngine;

public class PlayerAirChargeAttackState : PlayerAirState
{
    bool activated;
    public PlayerAirChargeAttackState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerVerticalMovement.ZeroVerticalVelocity();
        playerVerticalMovement.EnableGravity(false);
        animationHandler.CrossFade("AirChargeAttack_Start", 0.1f);
    }

    public override void Exit()
    {
        base.Exit();
        entity.ImpulseSystem.Reset();
        activated = false;
    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.IsPlaying("AirChargeAttack_Start") && animationHandler.NormalizedTime() >= 1)
        {
            playerVerticalMovement.EnableGravity(true);
            animationHandler.Play("AirChargeAttack_End");
        }

        if(animationHandler.IsPlaying("AirChargeAttack_End") && animationHandler.NormalizedTime() >= .12f)
        {
            entity.ImpulseSystem.AddVerticalImpulse(-25);
            animationHandler.Anim.SetFloat("AirChargeSpeed", 0);
        }

        if(animationHandler.IsPlaying("AirChargeAttack_End") && fallingCheckGround && !activated)
        {
            activated = true;
            animationHandler.Anim.SetFloat("AirChargeSpeed", 1);

            entity.StartCoroutine(entity.ShakeGenerator.ApplyNoise(2.5f, 1.5f, .2f));
        }

        if(animationHandler.IsPlaying("AirChargeAttack_End") && animationHandler.NormalizedTime() >= .6f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }
}
