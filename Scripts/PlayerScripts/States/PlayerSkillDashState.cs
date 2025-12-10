using UnityEngine;

public class PlayerSkillDashState : PlayerGroundState
{
    Vector3 skilldashDirection;
    Vector3 targetDir;
    public PlayerSkillDashState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerBlackboard.canAttack = false;
        playerBlackboard.canDodge = false;
        playerBlackboard.canParry = false;
        playerBlackboard.canChargeAttack = false;

        playerBlackboard.isInvulnerable = true;

        if(enemyDetector.CurrentTarget != null)
        {
            targetDir = (enemyDetector.CurrentTarget.position - entity.transform.position).normalized;
            entity.transform.LookAt(enemyDetector.CurrentTarget);
            skilldashDirection = targetDir * 7;
        }
        else
        {
            skilldashDirection = entity.transform.forward * 7;
        }

        animationHandler.Play("Skill_Dash");

        entity.SkillsSFX.PlaySoundFX();
    }
    public override void Exit()
    {
        base.Exit();
        playerBlackboard.canAttack = true;
        playerBlackboard.canChargeAttack = true;
        playerBlackboard.canDodge = true;

        playerBlackboard.isInvulnerable = false;
    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.IsPlaying("Skill_Dash") && animationHandler.NormalizedTime() <= 0.45f)
        {
            entity.CharacterController.Move(skilldashDirection * Time.deltaTime);
        }
        else
        {
            entity.CharacterController.Move(Vector3.zero);
        }


        if (animationHandler.IsPlaying("Skill_Dash") && animationHandler.NormalizedTime() >= 1f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }



    protected override bool ShouldUpdateInput => false;
}
