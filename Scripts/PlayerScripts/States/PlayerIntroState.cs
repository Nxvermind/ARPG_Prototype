using System.Collections;
using UnityEngine;

public class PlayerIntroState : PlayerBaseState
{
    bool canExitIntroState;
    public PlayerIntroState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerBlackboard.canAttack = false;
        playerBlackboard.canChargeAttack = false;
        playerBlackboard.canDodge = false;
        playerBlackboard.canParry = false;

        canExitIntroState = false;

        animationHandler.Play("Intro_Loop");
    }

    public override void Exit()
    {
        base.Exit();

        playerBlackboard.canAttack = true;
        playerBlackboard.canChargeAttack = true;
        playerBlackboard.canDodge = true;    
        playerBlackboard.canParry = true;


        entity.introScript.panelToMove.SetActive(true);
    }

    public override void Update()
    {
        base.Update();

        if (!entity.introScript.introStarted) return;

        if (!canExitIntroState)
        {
            ResetAnim();
        }

        if (animationHandler.IsPlaying("Intro_Loop") && animationHandler.NormalizedTime() >= .9f)
        {
            animationHandler.Play("Intro_End");
        }

        if (animationHandler.IsPlaying("Intro_End") && animationHandler.NormalizedTime() >= .7f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }

    private void ResetAnim()
    {
        animationHandler.Play("EmptyState", 0, 0);
        animationHandler.Anim.Update(0); 
        animationHandler.Play("Intro_Loop", 0, 0);
        canExitIntroState = true;
    }
}
