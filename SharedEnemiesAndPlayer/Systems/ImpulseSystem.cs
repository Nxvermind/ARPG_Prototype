using UnityEngine;

public class ImpulseSystem
{
    public Vector3 horizontalImpulse;
    public Vector3 verticalImpulse;

    private const float snapThreshold = 0.01f;
    private bool noDampActive;
    private float noDampDuration;

    public void AddHorizontalImpulse(Vector3 dir, float force)
    {
        horizontalImpulse = dir.normalized * force;
    }

    public void AddHorizontalImpulseNoDamp(Vector3 dir, float force, float duration)
    {
        noDampActive = true;
        noDampDuration = duration;
        horizontalImpulse = dir.normalized * force;
    }

    public void AddVerticalImpulse(float force)
    {
        verticalImpulse += Vector3.up * force;
    }

    public bool HasImpulse()
    {
        if (horizontalImpulse != Vector3.zero || verticalImpulse != Vector3.zero) return true;

        return false;
    }

    public bool HasVerticalImpulse() => Mathf.Abs(verticalImpulse.y) > 0.1f;

    public Vector3 ConsumeImpulse()
    {
        horizontalImpulse = ResolveHorizontalImpulse();

        verticalImpulse = Vector3.MoveTowards(verticalImpulse, Vector3.zero, 20 * Time.deltaTime);

        return horizontalImpulse + verticalImpulse;
    }

    private Vector3 DampVector(Vector3 current, float dampSpeed)
    {
        current = Vector3.Lerp(current, Vector3.zero, dampSpeed * Time.deltaTime);

        if (current.sqrMagnitude < snapThreshold * snapThreshold)
        {
            current = Vector3.zero;
        }

        return current;
    }

    private Vector3 ResolveHorizontalImpulse()
    {
        if (noDampActive)
        {
            noDampDuration -= Time.deltaTime;

            if(noDampDuration <= 0)
            {
                noDampActive = false;
                horizontalImpulse = Vector3.zero;
            }

            return horizontalImpulse;
        }

        return DampVector(horizontalImpulse, 3);
    }

    public void Reset()
    {
        horizontalImpulse = Vector3.zero;
        verticalImpulse = Vector3.zero;

        noDampActive = false;
        noDampDuration = 0;
    }
}
