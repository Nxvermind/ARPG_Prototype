using UnityEngine;


public class PlayerMovementHandler 
{
    private readonly PlayerHorizontalMovement playerHorizontalMovement;
    private readonly PlayerVerticalMovement playerVerticalMovement;
    private readonly ImpulseSystem impulseSystem;
    private readonly CharacterController characterController;
    private readonly PlayerBlackboard playerBlackboard;
    private readonly MotionSystem motionSystem;

    private Vector3 correction;

    public PlayerMovementHandler(PlayerHorizontalMovement playerHorizontalMovement, PlayerVerticalMovement playerVerticalMovement, 
        ImpulseSystem impulseSystem, CharacterController characterController, PlayerBlackboard playerBlackboard, MotionSystem motionSystem)
    {
        this.playerHorizontalMovement = playerHorizontalMovement;
        this.playerVerticalMovement = playerVerticalMovement;
        this.impulseSystem = impulseSystem;
        this.characterController = characterController;
        this.playerBlackboard = playerBlackboard;
        this.motionSystem = motionSystem;
    }

    public void HandleMovement()
    {
        Vector3 horizontal = playerHorizontalMovement.GetHorizontalMovement();

        Vector3 verticalMovement = playerVerticalMovement.GetVerticalMovement();

        Vector3 impulse = impulseSystem.ConsumeImpulse();

        Vector3 motion = motionSystem.ConsumeMotion();

        if (playerBlackboard.applyMovementCorrection)
        {
            if(characterController.GetPenetrationInLayer(playerBlackboard.correctionLayers, out Vector3 correction))
            {
                this.correction = correction;
            }
        }
        else
        {
            correction = Vector3.zero;
        }

        Vector3 final = horizontal + verticalMovement + impulse + motion + correction;

        characterController.Move(final * Time.deltaTime);
    }
}
