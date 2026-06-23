using System.Collections;
using UnityEngine;

public class EnemyTurnAroundState : EnemyBaseState
{
    private float delayTimer;

    private int maxTimes = 3;
    int counter;

    public EnemyTurnAroundState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rootMotion.ActivateRootMotion();

        entity.EnemyNavigation.StopMovement();
        entity.EnemyNavigation.ClearMovement();

        enemyBlackboard.canLookAtPlayer = false;
        enemyBlackboard.IsTurningAround = true;
        //enemyBlackboard.onlyTakeDamage = true;
        enemyBlackboard.canUpdateStaggerValue = true;

        enemyBlackboard.rememberedGotHit = false;

        agent.updateRotation = false;

        delayTimer = 0f;

        SpecificTurnAround();
    }

    public override void Exit()
    {
        base.Exit();
        enemyBlackboard.IsTurningAround = false;
        enemyBlackboard.canLookAtPlayer = true;
        enemyBlackboard.onlyTakeDamage = false;

        rootMotion.DeactivateRootMotion();

        entity.EnemyNavigation.ResumeMovement();

        agent.updateRotation = true;

        delayTimer = 0f;
        counter = 0;
    }

    public override void Update()
    {
        base.Update();

        if (basicEnemy)
        {
            if (enemyParameters.currentStaggerValue >= enemyParameters.maxStaggerValue &&
                            enemyGroundDetection.isGrounded &&
                            !enemyBlackboard.isIncapacitated)
            {
                stateMachine.ChangeState(enemyStateFactory.StaggerState);
                return;
            }
        }

        if (enemyVision.isPlayerInSight)
        {
            rootMotion.DeactivateRootMotion();
            entity.FacingSystem.RotateTowardsDirection(DirectionUtility.DirectionToTarget(entity.transform, target), 10);
            counter = 0;
            delayTimer += Time.deltaTime;
            
            if (DirectionUtility.Dot(entity.transform, target) >= .8f && delayTimer >= .5f)
            {
                stateMachine.ChangeState(enemyStateFactory.IdleState);
                return;
            }
        }
        else
        {
            rootMotion.ActivateRootMotion();

            if((animationHandler.IsPlaying("StrongGotHit_F") || animationHandler.IsPlaying("BigOrc_Skill_Out")) && animationHandler.NormalizedTime() >= 1)
            {
                SpecificTurnAround();
            }

            if (IsTurnAnimationFinished(.8f))
            {
                if(counter >= maxTimes)
                {
                    stateMachine.ChangeState(enemyStateFactory.LookingForPlayerState);
                    return;
                }

                delayTimer = 0;
                SpecificTurnAround();
                return;
            }
        }
    }
    private bool IsTurnAnimationFinished(float normalizedTime)
    {
        return
            (animationHandler.IsPlaying("TurnRight") ||
             animationHandler.IsPlaying("TurnLeft") ||
             animationHandler.IsPlaying("TurnAround"))
            &&
            animationHandler.NormalizedTime() >= normalizedTime;
    }


    private void SpecificTurnAround()
    {
        Vector3 toPlayer = DirectionUtility.DirectionToTarget(entity.transform, target);

        float signedAngle = Vector3.SignedAngle(entity.transform.forward, toPlayer, Vector3.up);

        float absAngle = Mathf.Abs(signedAngle);

        //Debug.Log($"signed angle is {signedAngle}");
        counter++;

        if (absAngle > 135)
        {
            animationHandler.Play("TurnAround");
        }
        else if (signedAngle > 45)
        {
            animationHandler.Play("TurnRight");
        }
        else if (signedAngle < -45)
        {
            animationHandler.Play("TurnLeft");
        }
    }
}
