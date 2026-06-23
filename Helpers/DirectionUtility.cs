using UnityEngine;

public static class DirectionUtility 
{
    public static float Dot(Transform origin, Transform target)
    {
        Vector3 dir = target.position - origin.position;
        dir.y = 0;

        return Dot(origin.forward, dir);
    }

    public static float Dot(Vector3 forward, Vector3 direction)
    {
        forward.y = 0f;
        forward.Normalize();

        direction.y = 0f;
        direction.Normalize();

        return Vector3.Dot(forward, direction);
    }

    public static Vector3 DirectionToTarget(Transform origin, Transform target)
    {
        Vector3 dir = (target.position - origin.position).normalized;
        dir.y = 0f;
        dir.Normalize();

        return dir; 
    }
}
