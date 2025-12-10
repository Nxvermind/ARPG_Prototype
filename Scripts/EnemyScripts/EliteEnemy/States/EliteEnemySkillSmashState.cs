using System.Collections;
using UnityEngine;

public class EliteEnemySkillSmashState : EnemyBaseState
{
    Transform parentTransform;

    public EliteEnemySkillSmashState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {

        
    }

    public override void Enter()
    {
        base.Enter();

        parentTransform = entity.GetComponentInParent<Transform>();

        enemyBlackboard.onlyTakeDamage = true;

        entity.rb.isKinematic = false;
        entity.Agent.enabled = false;
        //Debug.Log("i enter the skill smash state");
        eliteEnemy.skillSmash.skillSmashReady = false;
        entity.StartCoroutine(SkillSmashCor());

        SwitchCameras.ForceExitLockOnTargetCamera = true;

        animationHandler.Play("BigOrc_JumpToSkill");
    }

    public override void Exit()
    {
        base.Exit();
        entity.rb.isKinematic = true;
        entity.Agent.enabled = true;
        stopLookingAtPlayer = false;
        enemyBlackboard.onlyTakeDamage = false;
        entity.StartCoroutine(eliteEnemy.skillSmash.SkillCD());
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying("BigOrc_Skill_In") && animationHandler.NormalizedTime() >= 1)
        {
            animationHandler.Play("BigOrc_Skill_Loop");
        }


        if (animationHandler.IsPlaying("BigOrc_Skill_Loop") && entity.EnemyGroundDetection.isGrounded)
        {
            eliteEnemy.skillSmash.SkillDamage();
            eliteEnemy.skillSmash.PlaySkillSFX();
            eliteEnemy.skillSmash.SkillSmashCameraShake();
            animationHandler.Play("BigOrc_Skill_Out");
        }

        if (animationHandler.IsPlaying("BigOrc_Skill_Out") && animationHandler.NormalizedTime() >= .9f)
        {

            stateMachine.ChangeState(eliteEnemy.eliteEnemyStateFactory.IdleState);
        }
        else
        {
            stopLookingAtPlayer = true;
        }
    }
    IEnumerator SkillSmashCor()
    {
        yield return new WaitUntil(() => eliteEnemy.skillSmash.AllowedToJump);

        eliteEnemy.skillSmash.Elevate(entity.rb);

        float startY = parentTransform.position.y;
        float maxHeight = startY + eliteEnemy.skillSmash.maxHeight;

        yield return new WaitUntil(() => parentTransform.position.y >= maxHeight);

        animationHandler.CrossFade("BigOrc_Skill_In", 0.1f);

        entity.rb.linearVelocity = Vector3.zero;
        entity.rb.useGravity = false;

        yield return new WaitForSecondsRealtime(1.2f);

        entity.rb.useGravity = true;

        entity.StartCoroutine(eliteEnemy.skillSmash.LerpPosition(target.position, entity.rb));


    }
}
