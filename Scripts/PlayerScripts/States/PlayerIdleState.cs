using System.Collections;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerBlackboard.canMove = true;

        animationHandler.Anim.applyRootMotion = false;

        playerBlackboard.canAttack = true;
        playerBlackboard.canChargeAttack = true;

        if(stateMachine.PreviousState == playerStateFactory.IntroState)
        {
            IntroScript.IntroFinishedEvent();
        }

        if(stateMachine.PreviousState == playerStateFactory.FallingState)
        {
            animationHandler.CrossFade("Idle", 0.35f);
            return;
        }

        if(stateMachine.PreviousState == playerStateFactory.UltimateSkillState)
        {
            entity.StartCoroutine(Delay());
        }

        animationHandler.CrossFade("Idle", 0.1f);
    }
    public override void Update()
    {
        base.Update();

        if ((xInput != 0 || zInput != 0) && !playerBlackboard.isRunning)
        {
            stateMachine.ChangeState(playerStateFactory.WalkState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    IEnumerator Delay()
    {
        playerBlackboard.canAttack = false;
        yield return new WaitForSecondsRealtime(.15f);
        playerBlackboard.canAttack = true;
    }
    protected override bool ShouldUpdateInput => true;
}
