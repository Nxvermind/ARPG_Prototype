using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UltimateSkill_UI : MonoBehaviour
{
    [SerializeField] private PlayerParameters playerParameters;
    public Image ultimateSkillBar;

    private Material mat;

    private float newBerserkAmount;

    private void Start()
    {
        mat = GameObject.Find("ulti_bar_bg").GetComponent<Image>().material;
        DisableGlow();
    }

    //private void OnEnable()
    //{
    //    EventBus.OnEnemyGotHitEvent += UpdateBerserkBar;
    //}

    //private void OnDisable()
    //{
    //    EventBus.OnEnemyGotHitEvent -= UpdateBerserkBar;
    //}

    public void UpdateBerserkBar(Enemy enemy)
    {
        playerParameters.currentUltimateSkillValue += playerParameters.regenUltimateSkillValue;
        ultimateSkillBar.fillAmount = playerParameters.currentUltimateSkillValue / playerParameters.maxUltimateSkillValue;
    }

    public void BerserkBarZero()
    {
        ultimateSkillBar.fillAmount = 0;
        playerParameters.currentUltimateSkillValue = 0;
    }

    public void EnableGlow()
    {
        mat.SetFloat("_Glow", 47f);
    }
    public void DisableGlow()
    {
        mat.SetFloat("_Glow", 0f);
    }

}
