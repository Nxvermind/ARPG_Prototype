using System.Collections;
using UnityEngine;

public class EnemyJumpBackwardsState : EnemyBaseState
{
    public EnemyJumpBackwardsState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        animationHandler.Play("JumpBackwards");

        entity.StartCoroutine(JumpBackCor());
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

    }

    private void JumpBackwards()
    {
        Vector3 dir = (entity.transform.position - entity.target.position).normalized;
        Vector3 force = dir * 30f + Vector3.up * 25f; // Ajusta estos valores según peso del enemigo
        entity.rb.AddForce(force, ForceMode.Impulse);
    }

    IEnumerator JumpBackCor()
    {
        entity.rb.isKinematic = false;
        entity.Agent.enabled = false;

        yield return new WaitUntil(() => animationHandler.IsPlaying("JumpBackwards") && animationHandler.NormalizedTime() >= .3f);

        JumpBackwards();

        yield return new WaitForSecondsRealtime(0.1f);

        yield return new WaitUntil(() => entity.EnemyGroundDetection.isGrounded);

        entity.rb.isKinematic = true;
        entity.Agent.enabled = true;

        stateMachine.ChangeState(enemyStateFactory.ChaseState);
    }
}
