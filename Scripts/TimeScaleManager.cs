using System.Collections;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    public static TimeScaleManager Instance { get; private set; }

    private int activeRequests;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Aplica hitstop global.
    /// scale: Time.timeScale deseado (0 = freeze total, 0.5 = cámara lenta)
    /// duration: Duración en tiempo real.
    /// </summary>
    public void ApplyHitstop(float scale, float duration)
    {
        scale = Mathf.Clamp(scale, 0f, 1f);

        if (duration <= 0f) return;

        activeRequests++;

        // Si es la primera pausa, aplica el timescale
        if (activeRequests == 1)
        {
            Time.timeScale = scale;
        }

        StartCoroutine(HitstopCoroutine(duration));
    }

    public void LerpScale(float startScale, float duration)
    {
        //Debug.Log("called");
        StartCoroutine(LerpTimeScale(startScale, duration));
    }

    private IEnumerator HitstopCoroutine(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        activeRequests = Mathf.Max(0, activeRequests - 1);

        if (activeRequests == 0)
        {
            Time.timeScale = 1f;
        }
    }

    IEnumerator LerpTimeScale(float startScale, float duration)
    {
        float elapsedTime = 0;

        while(elapsedTime < duration)
        {
            Time.timeScale = Mathf.Lerp(startScale, 1, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        Time.timeScale = 1;
    }
}
