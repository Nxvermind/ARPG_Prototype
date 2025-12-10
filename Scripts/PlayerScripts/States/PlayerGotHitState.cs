using UnityEngine;

public class PlayerGotHitState : PlayerBaseState
{
    public PlayerGotHitState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }


    public override void Enter()
    {
        base.Enter();

        playerBlackboard.canAttack = false;
        animationHandler.Play("Got_Hit");
    }

    public override void Exit()
    {
        base.Exit();
        playerBlackboard.canAttack = true;
    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.IsPlaying("Got_Hit") && animationHandler.NormalizedTime() >= .85f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }
    protected override bool ShouldUpdateInput => false;
}
