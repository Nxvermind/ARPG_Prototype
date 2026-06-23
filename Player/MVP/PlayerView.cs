using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public BarData healthData;

    public BarData staminaData;

    public BarData postureData;

    [Header("UltimateSkill")]
    [SerializeField] private Image ultimateSkillBar;
    [SerializeField] private Material matOfUltimateSkillBar;

    private readonly List<BarData> datas = new();

    private void Start()
    {
        DisableUltimateSkillGlow();

        foreach(var data in datas)
        {
            data.timer = data.delay;
        }
    }

    private void Update()
    {
        UpdateDelayedBarView(healthData);
        UpdateFollowedBarView(staminaData);
        UpdateDelayedBarView(postureData);
    }

    public void UpdateHealthView(float currentHP, float maxHP)
    {
        UpdateBarView(healthData.mainBar, currentHP, maxHP);
        healthData.canUpdate = true;
    }

    public void UpdatePostureView(float currentPostureValue, float maxPostureValue)
    {
        UpdateBarView(postureData.mainBar, currentPostureValue, maxPostureValue);
        postureData.canUpdate = true;
    }

    public void UpdateStaminaView(float currentStamina, float maxStamina)
    {
        UpdateBarView(staminaData.mainBar, currentStamina, maxStamina);
    }

    public void UpdateBarView(Image imageBar, float currentValue, float maxValue)
    {
        imageBar.fillAmount = currentValue / maxValue;
    }

    #region UltimateSkill

    public void UpdateUltimateSkillBarView(float currentUltimateSkillValue, float maxUltimateSkillValue)
    {
        ultimateSkillBar.fillAmount = currentUltimateSkillValue / maxUltimateSkillValue;

        if (ultimateSkillBar.fillAmount >= 1) EnableUltimateSkillGlow();

        if (ultimateSkillBar.fillAmount <= 0 && matOfUltimateSkillBar.GetFloat("_Glow") != 0) DisableUltimateSkillGlow();
    }

    public void EnableUltimateSkillGlow()
    {
        matOfUltimateSkillBar.SetFloat("_Glow", 47f);
    }

    public void DisableUltimateSkillGlow()
    {
        matOfUltimateSkillBar.SetFloat("_Glow", 0f);
    }

    #endregion

    private void UpdateFollowedBarView(BarData barData)
    {
        if(barData.delayedBar.fillAmount >= barData.mainBar.fillAmount)
        {
            barData.canUpdate = true;
        }
        else
        {
            barData.canUpdate = false;
        }


        if (barData.canUpdate)
        {
            barData.delayedBar.fillAmount = Mathf.Lerp(barData.delayedBar.fillAmount, barData.mainBar.fillAmount, Time.deltaTime * barData.speed);
        }
        else
        {
            barData.delayedBar.fillAmount = barData.mainBar.fillAmount;
        }

    }

    private void UpdateDelayedBarView(BarData barData)
    {
        if (barData.canUpdate)
        {
            barData.timer -= Time.deltaTime;

            if (barData.timer < 0)
            {
                barData.delayedBar.fillAmount = Mathf.MoveTowards(barData.delayedBar.fillAmount, barData.mainBar.fillAmount, Time.deltaTime * barData.speed);
            }
        }

        if (Mathf.Abs(barData.mainBar.fillAmount - barData.delayedBar.fillAmount) < 0.0015f)
        {
            barData.canUpdate = false;
            barData.timer = barData.delay;
            barData.delayedBar.fillAmount = barData.mainBar.fillAmount;
        }
    }
}
