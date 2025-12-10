using UnityEngine;

public class PlayerVerticalMovement
{
    private readonly float gravity = -9.81f;
    private float gravityMultiplier = 2;

    public bool UseGravity { get; set; } = true;

    public float VerticalVelocity {  get; set; }

    public void EnableGravity(bool enable) => UseGravity = enable;

    public void ApplyGravity(bool isGrounded)
    {
        if (!UseGravity)
        {
            return;
        }

        if (isGrounded)
        {
            VerticalVelocity = -3f;

        }
        else
        {
            VerticalVelocity += gravity * gravityMultiplier * Time.deltaTime;

        }

        //Debug.Log($"vertical velocity is {VerticalVelocity}");
    }

    public Vector3 GetVerticalMovement()
    {
        return Vector3.up * VerticalVelocity;
    }

    public void ZeroVerticalVelocity()
    {
        VerticalVelocity = 0;
    }
}
