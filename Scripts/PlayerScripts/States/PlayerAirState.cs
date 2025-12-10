using UnityEngine;

public class PlayerAirState : PlayerBaseState
{
    protected bool fallingCheckGround;

    protected bool collisioningWithEnemy;

    public PlayerAirState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        fallingCheckGround = Physics.Raycast(entity.transform.position + playerBlackboard.checkGroundOffset, Vector3.down, playerBlackboard.checkGroundDistance,
              playerBlackboard.groundLayer);

        collisioningWithEnemy = Physics.CheckSphere(entity.transform.position + playerBlackboard.checkGroundOffset, .4f, playerBlackboard.enemyLayer);

        if (Input.GetKeyDown(KeyCode.V))
        {
            stateMachine.ChangeState(playerStateFactory.AirChargeAttackState);
        }

        //Debug.Log($"im in {stateMachine.CurrentState}");


    }
}
