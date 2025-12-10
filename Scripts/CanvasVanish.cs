using System.Collections;
using UnityEngine;

public class CanvasVanish : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Tooltip("time of the interpolation from visible to invisible and viceversa")]
    [SerializeField] private float vanishingDuration;

    public float canvasVanishDelay;
    public bool isCanvasVanished;

    public bool startIntroText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (startIntroText)
        {
            StartCoroutine(IntroText());
        }
    }

    public void StartVanishing()
    {
        StopAllCoroutines();
        StartCoroutine(VanishingCanvas(1, 0, vanishingDuration));
    }

    public void ReverseVanishing()
    {
        StopAllCoroutines();
        StartCoroutine(VanishingCanvas(0,1, vanishingDuration));
    }

    public void InvisibleCanvas()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0;
    }

    IEnumerator VanishingCanvas(float _start, float _end,float _duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(_start, _end, elapsedTime / _duration);
            yield return null;
        }

        canvasGroup.alpha = _end;
    }

    IEnumerator IntroText()
    {
        yield return new WaitForSeconds(1.2f);

        StartCoroutine(VanishingCanvas(0, 1, vanishingDuration));

        yield return new WaitForSeconds(2);

        StartCoroutine(VanishingCanvas(1, 0, vanishingDuration));
    }
}
