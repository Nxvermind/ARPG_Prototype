using System;
using UnityEngine;

public class MotionSystem
{
    private enum MotionType
    {
        Linear,
        Parabolic,
        Curved
    }

    private MotionType motionType;

    //Common
    private Vector3 start;
    private Vector3 end;
    private float duration;
    private float elapsed;
    private Vector3 lastPosition;

    //Linear
    private AnimationCurve animationCurve;

    //Parabolic
    private float maxHeight;
    private AnimationCurve horizontalCurve;
    private AnimationCurve verticalCurve;

    //Parabolic Dynamic
    private Func<Vector3> target;
    private bool useDynamicTarget;

    //Curved Lateral
    private float curveDistance;
    private AnimationCurve lateralCurve;
    private float curveSign;

    private bool isActive;
    public bool HasMotion => isActive;

    public void StartMotion(Vector3 start, Vector3 end, float duration)
    {
        this.start = start;
        this.end = end;
        this.duration = duration;
        animationCurve = null;
        elapsed = 0f;
        isActive = true;
        lastPosition = start;
        motionType = MotionType.Linear;
    }

    public void StartMotion(Vector3 start, Vector3 end, float duration, AnimationCurve animationCurve)
    {
        this.start = start;
        this.end = end;
        this.duration = duration;
        this.animationCurve = animationCurve;
        elapsed = 0f;
        isActive = true;
        lastPosition = start;
        motionType = MotionType.Linear;
    }

    public void StartParabolicMotion(Vector3 start, Vector3 end, float duration, float maxHeight, 
    AnimationCurve horizontalCurve, AnimationCurve verticalCurve)
    {
        this.start = start;
        this.end = end;
        this.duration = duration;
        this.maxHeight = maxHeight;
        this.horizontalCurve = horizontalCurve;
        this.verticalCurve = verticalCurve;
        elapsed = 0f;
        isActive = true;
        useDynamicTarget = false;
        target = null;
        motionType = MotionType.Parabolic;
        lastPosition = start;
    }

    public void StartParabolicMotion(Vector3 start, Func<Vector3> target, float duration, float maxHeight,
    AnimationCurve horizontalCurve, AnimationCurve verticalCurve)
    {
        this.start = start;
        this.target = target;
        this.end = target();
        this.duration = duration;
        this.maxHeight = maxHeight;
        this.horizontalCurve = horizontalCurve;
        this.verticalCurve = verticalCurve;
        elapsed = 0f;
        isActive = true;
        useDynamicTarget = true;
        motionType = MotionType.Parabolic;
        lastPosition = start;
    }

    public void StartCurvedMotion(Vector3 start, Vector3 end, float duration, float curveDistance,
    AnimationCurve horizontalCurve, AnimationCurve lateralCurve, float curveSign)
    {
        this.start = start;
        this.end = end;
        this.duration = duration;
        this.curveDistance = curveDistance;
        this.horizontalCurve = horizontalCurve;
        this.lateralCurve = lateralCurve;
        this.curveSign = curveSign;
        elapsed = 0;
        isActive = true;
        motionType = MotionType.Curved;
        lastPosition = start;
    }

    public Vector3 ConsumeMotion()
    {
        if (!isActive) return Vector3.zero;

        switch (motionType)
        {
            case MotionType.Linear:
                return ConsumeLinearMotion();

            case MotionType.Parabolic:
                return ConsumeParabolicMotion();

            case MotionType.Curved:
                return ConsumeCurvedMotion();

            default:
                return Vector3.zero;
        }
    }

    private Vector3 ConsumeLinearMotion()
    {
        elapsed += Time.deltaTime;

        Vector3 current;

        if (elapsed >= duration)
        {
            isActive = false;
            current = end;
        }
        else
        {
            float t = Mathf.Clamp01(elapsed / duration);
            float curvedT = animationCurve != null ? animationCurve.Evaluate(t) : t;
            current = Vector3.Lerp(start, end, curvedT);
        }

        Vector3 delta = current - lastPosition;
        lastPosition = current;

        return delta / Time.deltaTime;
    }

    private Vector3 ConsumeParabolicMotion()
    {
        elapsed += Time.deltaTime;

        float t = Mathf.Clamp01(elapsed / duration);

        if (elapsed >= duration)
        {
            isActive = false;

            if (useDynamicTarget)
            {
                end = target();
            }

            Vector3 finalDelta = end - lastPosition;

            lastPosition = end;

            target = null;
            useDynamicTarget = false;

            return finalDelta / Time.deltaTime;
        }

        if (useDynamicTarget)
        {
            end = target();
        }
        
        float horizontalT = horizontalCurve.Evaluate(t);

        Vector3 horizontalPosition = Vector3.Lerp(start, end, horizontalT);

        float verticalT = verticalCurve.Evaluate(t);

        float verticalOffset = verticalT * maxHeight;

        Vector3 current = horizontalPosition + Vector3.up * verticalOffset;

        Vector3 delta = current - lastPosition;

        lastPosition = current;

        return delta / Time.deltaTime;
    }

    private Vector3 ConsumeCurvedMotion()
    {
        elapsed += Time.deltaTime;

        float t = Mathf.Clamp01(elapsed / duration);

        if (elapsed >= duration)
        {
            isActive = false;

            Vector3 finalDelta = end - lastPosition;

            lastPosition = end;

            return finalDelta / Time.deltaTime;
        }

        float forwardT = horizontalCurve.Evaluate(t);

        Vector3 forwardPosition = Vector3.Lerp(start, end, forwardT);

        Vector3 moveDir = (end - start).normalized;

        Vector3 sideDir = Vector3.Cross(Vector3.up, moveDir);

        float sideT = lateralCurve.Evaluate(t);

        float sideOffset = sideT * curveDistance * curveSign;

        Vector3 current = forwardPosition + sideDir * sideOffset;

        Vector3 delta = current - lastPosition;

        lastPosition = current;

        return delta / Time.deltaTime;
    }

    public void LockTarget()
    {
        if (target == null) return;

        end = target();
        target = null;
        useDynamicTarget = false;
    }

    public void ResetMotion()
    {
        isActive = false;
    }
}
