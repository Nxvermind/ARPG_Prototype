using System.Collections;
using UnityEngine;

public class PlayerDodgeState : PlayerGroundState
{
    public PlayerDodgeState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerBlackboard.canMove = false;

        entity.StartCoroutine(StartInvulneravility());

        playerBlackboard.canAttack = false;
        playerBlackboard.canChargeAttack = false;

        Vector3 cameraForward = entity.cameraTransform.forward;
        Vector3 cameraRight = entity.cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 currentInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 desiredDir;

        if (currentInput != Vector3.zero)
        {
            desiredDir = (cameraForward * currentInput.z + cameraRight * currentInput.x).normalized;
        }
        else
        {
            desiredDir = entity.transform.forward;
        }

        entity.transform.rotation = Quaternion.LookRotation(desiredDir);

        entity.ImpulseSystem.AddImpulse(desiredDir, playerParameters.dodgeForce);


        if (SwitchCameras.IsLockOnTargetCameraActive)
        {
            animationHandler.Play("Dodge_BlendTree");
        }
        else
        {
            //animationHandler.CrossFade("Dodge", .1f);
            animationHandler.Play("Dodge");
        }

        entity.SlashVFX.StopSlashAndCoroutine();

        //if (entity.canvasVanish.isCanvasVanished)
        //{
        //    playerBlackboard.lastTimeAttacked = Time.time;
        //    entity.canvasVanish.isCanvasVanished = false;
        //    entity.canvasVanish.ReverseVanishing();
        //}

        playerBlackboard.LastTimeToStartRegenStamina = Time.time;
        entity.PlayerModel.ConsumeStamina(10);
        playerParameters.currentStamina = entity.PlayerModel.CurrentStamina;
    }

    public override void Exit()
    {
        base.Exit();
        playerBlackboard.canMove = true;
        playerBlackboard.canAttack = true;
        playerBlackboard.canChargeAttack = true;

        entity.ImpulseSystem.Reset();
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying("Dodge_BlendTree") && animationHandler.NormalizedTime() >= 0.55f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }

        if (animationHandler.IsPlaying("Dodge") && animationHandler.NormalizedTime() >= 0.55f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }

    IEnumerator StartInvulneravility()
    {
        playerBlackboard.isInvulnerable = true;
        yield return new WaitForSecondsRealtime(playerBlackboard.invulneravilityTime);
        playerBlackboard.isInvulnerable = false;
    }


    protected override bool ShouldUpdateInput => false;
}
