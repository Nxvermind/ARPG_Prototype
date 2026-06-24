using System.Collections;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    public static TimeScaler instance;

    private Coroutine cor;

    private void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        SkillsEvents.OnUltimateSkillStarted += ZeroTimeScale;
        SkillsEvents.OnUltimateSkillEnded += NormalTimeScale;
    }

    void OnDisable()
    {
        SkillsEvents.OnUltimateSkillStarted -= ZeroTimeScale;
        SkillsEvents.OnUltimateSkillEnded -= NormalTimeScale;
    }

    public void ZeroTimeScale()
    {
        if (cor != null)
        {
            StopCoroutine(cor);
        }

        Time.timeScale = 0;
    }

    public void NormalTimeScale()
    {
        Time.timeScale = 1;
    }

    public void ApplyHitstop(float scale, float duration)
    {
        scale = Mathf.Clamp01(scale);

        if (cor != null)
        {
            StopCoroutine(cor);
        }

        cor = StartCoroutine(HitstopCor(scale, duration));
    }

    public void ApplyTimeStop(float scale, float holdDuration, float recoveryDuration, AnimationCurve animationCurve)
    {
        scale = Mathf.Clamp01(scale);

        if (cor != null)
        {
            StopCoroutine(cor);
        }

        cor = StartCoroutine(TimeStopCor(scale, holdDuration, recoveryDuration, animationCurve));
    }

    public void ApplyHitstopWithLerp(float scale, float duration)
    {
        scale = Mathf.Clamp01(scale);

        if(cor != null)
        {
            StopCoroutine(cor);
        }

        cor = StartCoroutine(HitstopCor_Lerp(scale, duration));
    }

    IEnumerator HitstopCor(float scale, float duration)
    {
        Time.timeScale = scale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1;

        cor = null;
    }

    IEnumerator HitstopCor_Lerp(float scale, float duration)
    {
        float elapsedTime = 0;

        while(elapsedTime < duration)
        {
            Time.timeScale = Mathf.Lerp(scale, 1, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            Debug.Log(Time.timeScale);
            yield return null;
        }

        Time.timeScale = 1;

        cor = null;
    }

    IEnumerator TimeStopCor(float scale, float holdDuration, float recoveryDuration)
    {
        Time.timeScale = scale;

        yield return new WaitForSecondsRealtime(holdDuration);

        float elapsedTime = 0;

        while(elapsedTime < recoveryDuration)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale,1, elapsedTime / recoveryDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1;
        cor = null;
    }

    IEnumerator TimeStopCor(float scale, float holdDuration, float recoveryDuration, AnimationCurve animationCurve)
    {
        Time.timeScale = scale;

        yield return new WaitForSecondsRealtime(holdDuration);

        float elapsedTime = 0;

        while (elapsedTime < recoveryDuration)
        {
            float time = Mathf.Clamp01(elapsedTime / recoveryDuration);
            Time.timeScale = Mathf.Lerp(scale, 1, animationCurve.Evaluate(time));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1;

        cor = null;
    }
}
