using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    private PlayerParameters playerParameters;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthBar_Lerp;

    private Coroutine healthCoroutine;

    private float newHealthAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerParameters = GetComponent<PlayerParameters>();    
    }

    public void UpdateHealthValue()
    {
        newHealthAmount = playerParameters.currentHp / playerParameters.maxHP;
        healthBar.fillAmount = newHealthAmount;

        if (healthCoroutine != null) StopCoroutine(healthCoroutine);
        healthCoroutine = StartCoroutine(LerpingHealthValue(healthBar_Lerp.fillAmount, newHealthAmount));
    }

    IEnumerator LerpingHealthValue(float currentValue, float newCurrentValue)
    {
        float elapsedTime = 0;
        float lerpDuration = 1;

        while (elapsedTime < lerpDuration)
        {
            healthBar_Lerp.fillAmount = Mathf.Lerp(currentValue, newCurrentValue, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        healthBar_Lerp.fillAmount = newCurrentValue;
    }

    public void ResetHealthValue()
    {
        healthBar.fillAmount = 1;

    }
}
