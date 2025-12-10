using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    private Coroutine patrolCoroutine;

    private int currentPoint = 1;

    public EnemyPatrolState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        entity.Agent.speed = enemyParameters.patrolSpeed;
        patrolCoroutine = entity.StartCoroutine(Patroling());
        animationHandler.CrossFade("Patrol", 0.1f);
    }

    public override void Exit()
    {
        base.Exit();
        if (patrolCoroutine != null)
        {
            entity.StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
        currentPoint = 1;
    }

    public override void Update()
    {
        base.Update();
        if (enemyVision.playerInSight && enemyGroundDetection.isGrounded)
        {
            stateMachine.ChangeState(enemyStateFactory.ChaseState);
        }
    }
    IEnumerator Patroling()
    {
        while (true)
        {
            if (currentPoint >= basicEnemy.patrolPoints.Length)
            {
                currentPoint = 0;
            }

            entity.Agent.SetDestination(basicEnemy.patrolPoints[currentPoint].position);

            // 3. Esperar a que llegue al destino
            while (!HasReachedDestination())
            {
                yield return null; // Espera a que se complete la ruta
            }

            // 4. Reproducir animación "Idle" al llegar
            animationHandler.CrossFade("Idle", 0.1f);

            // 5. Esperar 6 segundos en el lugar
            yield return new WaitForSecondsRealtime(2);

            currentPoint++;

            animationHandler.CrossFade("Patrol", 0.1f);
        }
    }
}
