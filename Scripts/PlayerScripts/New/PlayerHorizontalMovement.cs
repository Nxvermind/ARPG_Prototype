using System.Collections;
using UnityEngine;

public class PlayerHorizontalMovement
{
    private readonly Transform cameraTransform;
    private Vector3 moveDirection;
    
    public float MoveSpeed { get; set; }

    public PlayerHorizontalMovement(Transform cameraTransform)
    {
        this.cameraTransform = cameraTransform;
    }

    public Vector3 GetHorizontalMovement()
    {
        return new Vector3(moveDirection.x * MoveSpeed, 0, moveDirection.z * MoveSpeed);
    }

    public Vector3 GetRawMoveDirection()
    {
        return moveDirection;
    }

    public void CalculateCameraRelativeMovement(float xInput, float zInput)
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = (cameraForward * zInput + cameraRight * xInput).normalized;
    }
}
