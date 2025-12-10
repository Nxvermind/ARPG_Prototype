using UnityEngine;

public class PlayerExecutionState : PlayerGroundState
{
    public PlayerExecutionState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerBlackboard.canAttack = false;
        playerBlackboard.onlyTakeDamage = true;


        entity.canvasVanish.InvisibleCanvas();

        animationHandler.Play("Execution");
    }

    public override void Exit()
    {
        base.Exit();
        playerBlackboard.onlyTakeDamage = false;

        entity.canvasVanish.ReverseVanishing();
    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.IsPlaying("Execution") && animationHandler.NormalizedTime() >= 1f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }

    }

    protected override bool ShouldUpdateInput => false;
}
