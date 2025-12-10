using UnityEngine;

public class PlayerParryState : PlayerGroundState
{
    private bool inParryLoop;
    public PlayerParryState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        animationHandler.CrossFade("Parry_Enter", 0.05f);
        inParryState = true;
        playerBlackboard.canAttack = false;

        parrySystem.isPlayerBlocking = true;

        parrySystem.coll.enabled = true;
        entity.StartCoroutine(parrySystem.ParryWindowRoutine());

        enemyDetector.RotateToEnemy();

        playerBlackboard.canChargeAttack = false;
    }

    public override void Exit()
    {
        base.Exit();
        inParryLoop = false;
        inParryState = false;
        playerBlackboard.canAttack = true;

        parrySystem.isPlayerBlocking = false;
        parrySystem.coll.enabled = false;

        playerBlackboard.canChargeAttack = true;

        playerBlackboard.chargeStartTime = Mathf.Infinity;
    }

    public override void Update()
    {
        base.Update();

        if(inputHandler.LightAttackButtonPressed && entity.parrySystem.canParryAttack)
        {
            stateMachine.ChangeState(playerStateFactory.GroundAttackState);
        }

        if (animationHandler.IsPlaying("Parry_Enter") && animationHandler.NormalizedTime() >= 1f && !inParryLoop)
        {
            animationHandler.CrossFade("Parry_Loop", 0.05f);
            inParryLoop = true;
        }
        
        if (inputHandler.ParryInputUp)
        {
            animationHandler.CrossFade("Parry_End", 0.05f);
        }

        if(animationHandler.IsPlaying("Parry_End") && animationHandler.NormalizedTime() >= .65f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }

        if(animationHandler.IsPlaying("Parry_Accept") && animationHandler.NormalizedTime() >= 1)
        {
            animationHandler.Play("Parry_Loop");
        }
    }

    protected override bool ShouldUpdateInput => false;
}
