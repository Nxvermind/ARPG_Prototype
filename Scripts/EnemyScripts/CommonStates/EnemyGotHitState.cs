using UnityEngine;
using System.Collections;

public class EnemyGotHitState : EnemyBaseState
{
    public EnemyGotHitState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
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

        if(enemyBlackboard.gotHit && !entity.EnemyGroundDetection.isGrounded)
        {
            stateMachine.ChangeState(enemyStateFactory.AirGotHitState);
        }
    }

    protected void Levitate()
    {
        entity.rb.isKinematic = false;
        entity.Agent.enabled = false;
        //entity.rb.linearVelocity = new(0, entity.rb.linearVelocity.y, 0);
        entity.rb.AddForce(Vector3.up * entity.attackProvider.CurrentAttackNode.strongAttackForce, ForceMode.VelocityChange);
        //entity.StartCoroutine(LevitateCurve(basicEnemy.EnemyCurves.levitateCurve, 0.4f));
    }

    IEnumerator LevitateCurve(AnimationCurve curve, float duration)
    {
        float time = 0;
        Vector3 start = basicEnemy.rb.position;

        while (time < duration)
        {
            float t = time / duration;
            float height = curve.Evaluate(t);

            Vector3 position = new Vector3(start.x, start.y + height, start.z);
            basicEnemy.rb.MovePosition(position);

            time += Time.deltaTime;
            yield return null;
        }
    }
}
