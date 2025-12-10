using System.Collections;
using UnityEngine;

public class EnemyLookingForPlayerState : EnemyBaseState
{
    Vector3 velocity = Vector3.zero;

    public EnemyLookingForPlayerState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.updatePosition = false;
        animationHandler.CrossFade("AggresiveWalk", 0.14f);
        agent.speed = enemyParameters.aggresiveWalkSpeed;

        if (enemyVision.wasPlayerDetected && enemyBlackboard.lastKnownPlayerPosition != Vector3.zero)
        {
            entity.StartCoroutine(LookForPlayer(enemyBlackboard.lastKnownPlayerPosition));
        }
        else
        {
            entity.StartCoroutine(LookForPlayer(enemyBlackboard.positionToStartSearching));
        }

    }

    public override void Exit()
    {
        base.Exit();
        agent.updatePosition = true;
        agent.ResetPath();
    }

    public override void Update()
    {
        base.Update();
        entity.transform.position = Vector3.SmoothDamp(entity.transform.position, agent.nextPosition, ref velocity, 0.01f);
    }

    IEnumerator LookForPlayer(Vector3 startPosition)
    {
        if(enemySearchSystem.currentTry >= enemySearchSystem.maxTimesToTry)
        {
            Debug.Log("Last time tried, going back to patrol point");
            stateMachine.ChangeState(basicEnemy.basicEnemyStateFactory.BackToPatrolPointState);
            enemySearchSystem.currentTry = 0;
            yield break;
        }

        if(enemySearchSystem.GetReachableRandomPosition(startPosition, agent, 10 , out Vector3 result))
        {
            agent.SetDestination(result);
        }

        yield return new WaitUntil(() => HasReachedDestination() || enemyVision.playerInSight);

        enemySearchSystem.currentTry++;

        stateMachine.ChangeState(enemyStateFactory.IdleState);

    }
}
