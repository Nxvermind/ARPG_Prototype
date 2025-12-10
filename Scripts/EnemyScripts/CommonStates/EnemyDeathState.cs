using UnityEngine;
public class EnemyDeathState : EnemyBaseState
{
    float internTimer;
    public EnemyDeathState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        EventBus.EnemyDeathEvent(entity);
        entity.EnemyBlackboard.isDead = true;
        entity.Agent.velocity = Vector3.zero;
        entity.Agent.isStopped = true;

        entity.CanvasVanish.StartVanishing();

        entity.CapsuleCollider.enabled = false;

        if (basicEnemy)
        {
            foreach (var obj in basicEnemy.patrolPoints)
            {
                Object.Destroy(obj.gameObject);
            }
        }

        if (entity.EnemyGroundDetection.isGrounded)
        {
            animationHandler.Play("Death");
        }

    }

    public override void Exit()
    {
        base.Exit();   
    }

    public override void Update()
    {
        internTimer += Time.deltaTime;

        if(animationHandler.IsPlaying("Death"))
        {
            if (animationHandler.NormalizedTime() >= .95f)
            {
                Object.Destroy(entity.gameObject);
            }

            return;
        }

        if(internTimer > 1.7f)
        {
            Object.Destroy(entity.gameObject);
        }

    }
}
