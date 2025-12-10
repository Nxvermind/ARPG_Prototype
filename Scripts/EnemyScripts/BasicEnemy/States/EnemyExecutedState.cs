using System.Collections;
using UnityEngine;

public class EnemyExecutedState : EnemyBaseState
{
    float internTimer;
    bool startedDissolve = false;

    public EnemyExecutedState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        entity.rb.position = basicEnemy.executionPoint.position;
        entity.rb.rotation = basicEnemy.executionPoint.rotation;

        //entity.EnemyBlackboard.isBeingExecuted = false;

        stopLookingAtPlayer = true;

        entity.CanvasHUD.enabled = false;


        entity.CapsuleCollider.enabled = false;
        entity.Agent.enabled = false;
        entity.rb.isKinematic = true;

        EventBus.EnemyDeathEvent(entity);
        entity.EnemyBlackboard.isDead = true;

        entity.CanvasVanish.StartVanishing();

        foreach (var obj in basicEnemy.patrolPoints)
        {
            Object.Destroy(obj.gameObject);
        }

        animationHandler.CrossFade("Executed", 0.1f);
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.NormalizedTime() >= .7f) 
        {
            if (!startedDissolve)
            {
                //ApplyDissolveShader.instance.StartCoroutine(ApplyDissolveShader.instance.Delay());
                startedDissolve = true;
            }

            internTimer += Time.deltaTime;

            if (internTimer >= 2)
            {
                Object.Destroy(entity.gameObject);
                //stateMachine.ChangeState(enemyStateFactory.DeathState);
            }
        }
    }
}
