using UnityEngine;

public class PlayerFallingState : PlayerAirState
{

    public PlayerFallingState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerBlackboard.canChargeAttack = false;

        playerVerticalMovement.EnableGravity(true);

        animationHandler.CrossFade("Falling", 0.07f);
    }

    public override void Exit()
    {
        base.Exit();
        playerBlackboard.canParry = true;
        playerBlackboard.canChargeAttack = true;
    }

    public override void Update()
    {
        base.Update();

        if (collisioningWithEnemy)
        {
            Debug.Log("collisioning");
            entity.transform.position += new Vector3(0, 0, -0.1f);
        }

        if (fallingCheckGround)
        {
            playerBlackboard.canAttack = false;
            playerBlackboard.canChargeAttack = false;
            if (!animationHandler.IsPlaying("Landing"))
            {
                animationHandler.Play("Landing");
            }
        }


        if (animationHandler.IsPlaying("Landing") && animationHandler.NormalizedTime() > .9f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }
}
