using System.Collections;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        animationHandler.CrossFade("Idle",0.1f);

        if (stateMachine.PreviousState == enemyStateFactory.LookingForPlayerState && !enemyVision.playerInSight)
        {
            entity.StartCoroutine(BackToLookingForPlayer());
        }

        if (basicEnemy)
        {
            if (enemyBlackboard.rememberedGotHit && !enemyVision.playerInSight)
            {
                stateMachine.ChangeState(enemyStateFactory.TurnAroundState);

            }
        }


        EliteEnemySpecificIdle();
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        //if (enemyVision.playerInSight)
        //{
        //    if (basicEnemy)
        //    {
        //        entity.coordinator.PermissionToBeActiveAttacker(entity);

        //        if (!enemyVision.alreadySeenPlayer)
        //        {
        //            stateMachine.ChangeState(enemyStateFactory.PreparingtoAttack);
        //        }
        //        else
        //        {
        //            stateMachine.ChangeState(enemyStateFactory.ChaseState);
        //        }
        //    }

        //    if (eliteEnemy)
        //    {
        //        if (!enemyVision.alreadySeenPlayer)
        //        {
        //            enemyVision.alreadySeenPlayer = true;
        //            entity.StartCoroutine(eliteEnemy.skillSmash.SkillCD());
        //        }

        //        stateMachine.ChangeState(enemyStateFactory.ChaseState);
        //    }
        //}
    }

    IEnumerator BackToLookingForPlayer()
    {
        yield return new WaitForSecondsRealtime(2);

        stateMachine.ChangeState(enemyStateFactory.LookingForPlayerState);
    }

    private void EliteEnemySpecificIdle()
    {
        if (eliteEnemy)
        {
            if (stateMachine.PreviousState == eliteEnemyStateFactory.SkillSmashState && !enemyVision.playerInSight)
            {
                stateMachine.ChangeState(enemyStateFactory.TurnAroundState);
                return;
            }

            if (stateMachine.PreviousState == eliteEnemyStateFactory.StunState)
            {
                Vector3 enemyVector = entity.transform.forward;
                Vector3 d = (target.position - entity.transform.position).normalized;

                float dot = Vector3.Dot(enemyVector, d);

                if (dot < 0)
                {
                    stateMachine.ChangeState(enemyStateFactory.TurnAroundState);
                }

                return;
            }

            if(stateMachine.PreviousState == enemyStateFactory.TurnAroundState && !enemyVision.playerInSight)
            {
                entity.StartCoroutine(BackToTurnAroundState());
            }

        }
    }

    IEnumerator BackToTurnAroundState()
    {
        yield return new WaitForSecondsRealtime(1f);

        stateMachine.ChangeState(enemyStateFactory.TurnAroundState);  
    }
}
