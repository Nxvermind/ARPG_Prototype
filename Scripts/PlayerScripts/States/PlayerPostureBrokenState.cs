using UnityEngine;

public class PlayerPostureBrokenState : PlayerGroundState
{
    public PlayerPostureBrokenState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }


    public override void Enter()
    {
        base.Enter();
        playerBlackboard.canAttack = false;
        playerBlackboard.canChargeAttack = false;
        playerParameters.postureBroken = true;
        animationHandler.Play("Parry_Broken");
    }

    public override void Exit()
    {
        base.Exit();

        entity.Parameters.currentPostureValue = entity.Parameters.maxPostureValue;
        entity.PlayerModel.ResetPostureValue();
        //entity.PostureComponent.UpdatePostureValue();
        playerParameters.postureBroken = false;

        playerBlackboard.canAttack = true;
        playerBlackboard.canChargeAttack = true;
    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.IsPlaying("Parry_Broken") && animationHandler.NormalizedTime() >= 1)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }
    protected override bool ShouldUpdateInput => false;
}
