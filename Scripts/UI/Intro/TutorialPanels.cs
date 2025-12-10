using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialPanels : MonoBehaviour
{
    public RectTransform panel;
    public Vector2 initialPos;
    public Vector2 targetPosition;


    void OnEnable()
    {
        StartCoroutine(MovePanelTo(targetPosition, 1.75f));
    }


    IEnumerator MovePanelTo(Vector2 target, float waitDuration)
    {
        Vector2 start = panel.anchoredPosition;
        float elapsedTime = 0;
        float duration = .75f;

        while (elapsedTime < duration)
        {
            panel.anchoredPosition = Vector2.Lerp(start, target, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = target; 

        yield return new WaitForSeconds(waitDuration);

        elapsedTime = 0;
        duration = .75f;

        while (elapsedTime < duration)
        {
            panel.anchoredPosition = Vector2.Lerp(target, initialPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = initialPos; 
    }
}
