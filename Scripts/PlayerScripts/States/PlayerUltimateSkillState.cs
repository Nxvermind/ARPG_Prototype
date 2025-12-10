using UnityEngine;

public class PlayerUltimateSkillState : PlayerBaseState
{
    public PlayerUltimateSkillState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerBlackboard.canAttack = false;
        animationHandler.CrossFade("UltimateSkill", 0.1f);

        entity.canvasVanish.InvisibleCanvas();

        entity.PlayerModel.ResetUltimateSkillValue();
    }

    public override void Exit()
    {
        base.Exit();

        entity.canvasVanish.ReverseVanishing();
    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.IsPlaying("UltimateSkill") && animationHandler.NormalizedTime() >= 0.78f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }

    }

    protected override bool ShouldUpdateInput => false;
}
