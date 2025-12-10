using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthBar_Lerp;
    private float newHealthAmount;
    private Coroutine healthCoroutine;

    [Header("Stamina")]
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image staminaBar_Lerp;
    private float newStaminaAmount;

    [Header("Posture")]
    [SerializeField] private Image postureBar;
    [SerializeField] private Image postureBar_Lerp;
    private float newPostureAmount;
    private Coroutine postureCoroutine;

    [Header("UltimateSkill")]
    [SerializeField] private Image ultimateSkillBar;
    private Material mat;

    private void Awake()
    {
        mat = GameObject.Find("ulti_bar_bg").GetComponent<Image>().material;
    }

    private void Start()
    {
        DisableUltimateSkillGlow();
    }

    private void Update()
    {
        if (staminaBar.fillAmount >= 1) return;

        staminaBar_Lerp.fillAmount = Mathf.Lerp(staminaBar_Lerp.fillAmount, staminaBar.fillAmount, Time.deltaTime * 2);
    }

    #region Health
    public void UpdateHealthView(float currentHP, float maxHP)
    {
        newHealthAmount = currentHP / maxHP;
        healthBar.fillAmount = newHealthAmount;

        if (healthCoroutine != null) StopCoroutine(healthCoroutine);
        healthCoroutine = StartCoroutine(LerpImageValue(healthBar_Lerp, healthBar_Lerp.fillAmount, newHealthAmount));
    }
    #endregion

    #region Posture
    public void UpdatePostureView(float currentPostureValue, float maxPostureValue)
    {
        newPostureAmount = currentPostureValue / maxPostureValue;
        postureBar.fillAmount = newPostureAmount;

        if (postureCoroutine != null) StopCoroutine(postureCoroutine);
        postureCoroutine = StartCoroutine(LerpImageValue(postureBar_Lerp, postureBar_Lerp.fillAmount, newPostureAmount));
    }

    #endregion

    #region Stamina

    public void UpdateStaminaView(float currentStamina, float maxStamina)
    {
        newStaminaAmount = currentStamina / maxStamina;
        staminaBar.fillAmount = newStaminaAmount;
    }

    public void UpdateStaminaViewVariant(float currentStamina, float maxStamina)
    {
        newStaminaAmount = currentStamina / maxStamina;
        staminaBar.fillAmount = newStaminaAmount;
        staminaBar_Lerp.fillAmount = newStaminaAmount;
    }
    #endregion

    #region UltimateSkill

    public void UpdateUltimateSkillBarView(float currentUltimateSkillValue, float maxUltimateSkillValue)
    {
        ultimateSkillBar.fillAmount = currentUltimateSkillValue / maxUltimateSkillValue;

        if (ultimateSkillBar.fillAmount >= 1) EnableUltimateSkillGlow();
    }

    public void EnableUltimateSkillGlow()
    {
        mat.SetFloat("_Glow", 47f);
    }

    public void DisableUltimateSkillGlow()
    {
        mat.SetFloat("_Glow", 0f);
    }

    #endregion

    IEnumerator LerpImageValue(Image imgToLerp, float currentValue, float newCurrentValue, float duration = 1)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            imgToLerp.fillAmount = Mathf.Lerp(currentValue, newCurrentValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        imgToLerp.fillAmount = newCurrentValue;
    }
}
