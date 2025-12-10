using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ImpulseSystem
{
    public Vector3 horizontalImpulse;
    public Vector3 verticalImpulse;

    public void AddImpulse(Vector3 dir, float force)
    {
        horizontalImpulse += dir.normalized * force;
    }

    public void AddVerticalImpulse(float force)
    {
        verticalImpulse += Vector3.up * force;
    }

    public IEnumerator AddCurvedVerticalImpulse(AnimationCurve curve)
    {
        float elapsedTime = 0;
        float duration = 1;

        float t;

        while(elapsedTime < duration)
        {
            t = elapsedTime / duration;

            float evaluated = curve.Evaluate(t);

            verticalImpulse = evaluated * Vector3.up;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Reset();
    }

    public Vector3 ConsumeImpulse()
    {
        horizontalImpulse = Vector3.Lerp(horizontalImpulse, Vector3.zero, 3 * Time.deltaTime);

        verticalImpulse = Vector3.Lerp(verticalImpulse, Vector3.zero, 5 * Time.deltaTime);

        Vector3 total = horizontalImpulse + verticalImpulse;

        
        return total;
    }

    public void Reset()
    {
        horizontalImpulse = Vector3.zero;
        verticalImpulse = Vector3.zero;
    }
}
