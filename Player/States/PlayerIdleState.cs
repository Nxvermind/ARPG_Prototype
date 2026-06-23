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

        playerBlackboard.canAttack = true;
        playerBlackboard.canChargeAttack = true;

        if (stateMachine.PreviousState == playerStateFactory.IntroState)
        {
            IntroScript.IntroFinishedEvent();
        }

        if (stateMachine.PreviousState == playerStateFactory.FallingState)
        {
            animationHandler.CrossFade("Idle", 0.35f);
            return;
        }

        if (stateMachine.PreviousState == playerStateFactory.UltimateSkillState)
        {
            entity.StartCoroutine(Delay());
        }

        animationHandler.CrossFade("Idle", 0.1f);
    }
    public override void Update()
    {
        base.Update();

        PerformFirstAttack();

        if ((xInput != 0 || zInput != 0) && !playerBlackboard.isRunning)
        {
            stateMachine.ChangeState(playerStateFactory.WalkState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    IEnumerator Delay()
    {
        playerBlackboard.canAttack = false;
        playerBlackboard.canMove = false;
        yield return new WaitForSecondsRealtime(.8f);
        playerBlackboard.canAttack = true;
        playerBlackboard.canMove = true;
        playerCombatContext.ExecutingUltimateSkill(false);
    }
}
