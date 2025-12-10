using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        entity.hurtBox.enabled = false;
        characterController.detectCollisions = false;

        characterController.enabled = false;
        animationHandler.CrossFade("Death", 0.1f);


        PlayerEvents.PlayerDeathEvent();
    }

    public override void Exit()
    {
        base.Exit();

        playerBlackboard.isPlayerDead = false;

        entity.hurtBox.enabled = true;
        characterController.detectCollisions = true;

        characterController.enabled = true;

    }

    public override void Update()
    {
        base.Update();

        //if(animationHandler.IsPlaying("Death") && animationHandler.NormalizedTime() >= 1)
        //{
        //    entity.gameObject.SetActive(false);
        //}
    }
}
