using UnityEngine;

public class PlayerParryState : PlayerGroundState
{
    private bool inParryLoop;

    Transform target;

    private float enterTime;

    private bool exitingParry;
    public PlayerParryState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        target = targetingSystem.FindBestTargetInVisionAngle(targetingSystem.visionAngle);
        exitingParry = false;
        if(target == null)
        {
            target = targetingSystem.FindClosestEnemy(entity.transform);
        }

        facingSystem.FaceTarget(target);

        playerBlackboard.canMove = false;
        inParryState = true;
        playerBlackboard.canAttack = false;

        playerBlackboard.canRestorePosture = false;

        parrySystem.isBlocking = true;

        entity.StartCoroutine(parrySystem.ParryWindowRoutine());

        playerBlackboard.canChargeAttack = false;

        animationHandler.CrossFade("Parry_Enter", 0.05f);
        enterTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
        inParryLoop = false;
        inParryState = false;
        playerBlackboard.canAttack = true;

        parrySystem.isBlocking = false;


        playerBlackboard.canChargeAttack = true;

        playerBlackboard.canMove = true;

        playerBlackboard.groundChargeStartTime = Mathf.Infinity;

        playerBlackboard.canRestorePosture = true;
        playerBlackboard.lastTimeToStartRegenPosture = Time.time;
        animationHandler.Anim.ResetTrigger("parryUp");
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying("Parry_Enter") && animationHandler.NormalizedTime() >= 1f && !inParryLoop)
        {
            animationHandler.CrossFade("Parry_Loop", 0.05f);
            inParryLoop = true;
        }

        if (!inputHandler.ParryInputHeld && !exitingParry)
        {
            exitingParry = true;

            animationHandler.CrossFade("Parry_End", 0.05f);
        }

        if (exitingParry &&
            animationHandler.IsPlaying("Parry_End") &&
            animationHandler.NormalizedTime() >= .65f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
            return;
        }

        if (animationHandler.IsPlaying("Parry_Accept") && animationHandler.NormalizedTime() >= 1)
        {
            animationHandler.Play("Parry_Loop");
        }

        if (entity.PlayerModel.CurrentPostureValue <= 0)
        {
            stateMachine.ChangeState(playerStateFactory.PostureBrokenState);
            return;
        }
    }
}
