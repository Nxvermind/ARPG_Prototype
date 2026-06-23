using UnityEngine;

public class FacingSystem 
{
    private readonly Transform entity;

    public FacingSystem(Transform entity)
    {
        this.entity = entity;
    }

    public void FaceTarget(Transform target)
    {
        if (target == null) return;

        Vector3 dir = (target.position - entity.transform.position).normalized;
        dir.y = 0;

        entity.rotation = Quaternion.LookRotation(dir);
    }

    public void FaceDirection(Vector3 direction)
    {
        Vector3 dir = (direction - entity.transform.position).normalized;
        dir.y = 0;

        entity.rotation = Quaternion.LookRotation(dir);
    }

    public void RotateInstantly(Vector3 direction)
    {
        entity.rotation = Quaternion.LookRotation(direction);
    }

    public void RotateTowardsDirection(Vector3 direction, float rotationSpeed)
    {
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        entity.rotation = Quaternion.Slerp(entity.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
