using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PostureComponent : MonoBehaviour
{
    private PlayerParameters playerParameters;

    [SerializeField] private Image postureBar;
    [SerializeField] private Image postureBar_Lerp;

    private Coroutine postureCoroutine;

    private float newPostureAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerParameters = GetComponent<PlayerParameters>();

        //EventBus.OnPlayerBlockEvent += UpdatePostureValue;
    }

    private void OnDestroy()
    {
        //EventBus.OnPlayerBlockEvent -= UpdatePostureValue;
    }

    public void UpdatePostureValue()
    {
        newPostureAmount = playerParameters.currentPostureValue / playerParameters.maxPostureValue;
        postureBar.fillAmount = newPostureAmount;

        if (postureCoroutine != null) StopCoroutine(postureCoroutine);
        postureCoroutine = StartCoroutine(LerpingPostureValue(postureBar_Lerp.fillAmount, newPostureAmount));
    }

    public void RegenPostureValue(float amount)
    {
        playerParameters.currentPostureValue = Mathf.Max(0, playerParameters.currentPostureValue * amount);
    }

    IEnumerator LerpingPostureValue(float currentValue, float newCurrentValue)
    {
        float elapsedTime = 0;
        float lerpDuration = 1;

        while (elapsedTime < lerpDuration)
        {
            postureBar_Lerp.fillAmount = Mathf.Lerp(currentValue, newCurrentValue, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        postureBar_Lerp.fillAmount = newCurrentValue;
    }
}
