using System.Collections;
using UnityEngine;

public class PlayerPerfectDodgeAttackState : PlayerGroundState
{
    public PlayerPerfectDodgeAttackState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerBlackboard.canMove = false;
        playerBlackboard.isAttacking = true;
        playerBlackboard.isInvulnerable = true;
        playerBlackboard.applyMovementCorrection = true;
        comboSystem.SetCurrentAttackNode(entity.DodgeSystem.perfectDodgeAttackNode);

        animationHandler.CrossFade("PerfectDodge_Attack", .2f);

        facingSystem.FaceTarget(entity.DodgeSystem.Attacker.transform);
        entity.StartCoroutine(TargetAttractionWithDelay(playerCombatContext.CurrentAttackNode.targetAttractionDelay));
    }

    public override void Exit()
    {
        base.Exit();
        playerBlackboard.canMove = true;
        playerBlackboard.isAttacking = false;
        playerBlackboard.isInvulnerable = false;
        playerBlackboard.applyMovementCorrection = false;
        entity.rootMotion.DeactivateRootMotion();
    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.IsPlaying("PerfectDodge_Attack"))
        {
            if (animationHandler.NormalizedTime() >= .6f)
            {
                playerBlackboard.isAttacking = false;
                playerBlackboard.isInvulnerable = false;
                PerformFirstAttack();
            }

            if (animationHandler.NormalizedTime() >= .9f)
            {
                stateMachine.ChangeState(playerStateFactory.IdleState);
                return;
            }
        }
    }

    IEnumerator TargetAttractionWithDelay(float delay)
    {
        yield return new WaitUntil(() => animationHandler.IsPlaying("PerfectDodge_Attack") && animationHandler.NormalizedTime() >= delay);
        
        Vector3 start = entity.transform.position;

        Vector3 dir = (entity.DodgeSystem.Attacker.transform.position - start).normalized;

        Vector3 end = entity.DodgeSystem.Attacker.transform.position - dir * entity.DodgeSystem.attackStoppingDistance;

        entity.MotionSystem.StartMotion(start, end, entity.DodgeSystem.attackMotionDuration, entity.DodgeSystem.perfectDodgeAttackCurve);
    }
}
