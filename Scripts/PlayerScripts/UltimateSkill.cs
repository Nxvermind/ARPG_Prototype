using System.Collections;
using UnityEngine;

public class UltimateSkill : MonoBehaviour
{
    [SerializeField] private GameObject ultiFX;
    [SerializeField] private UltimateSkill_UI ultimateSkill_UI;

    [SerializeField] private float damage;

    private BoxCollider boxCollider;

    public bool ultimateSkillAvailable;

    private bool firstTime;
    public GameObject panel;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        firstTime = true;
    }

    private void OnEnable()
    {
        PlayerEvents.OnUltimateSkillCalled += StartUltiSkill;
    }

    private void OnDisable()
    {
        PlayerEvents.OnUltimateSkillCalled -= StartUltiSkill;
    }

    void Update()
    {
        //if (ultimateSkill_UI.ultimateSkillBar.fillAmount >= 1)
        //{
        //    ultimateSkillAvailable = true;          
        //    ultimateSkill_UI.EnableGlow();

        //    if (firstTime)
        //    {
        //        firstTime = false;
        //        panel.SetActive(true);
        //    }
        //}
    }

    public float Damage()
    {
        return damage;
    }

    private void StartUltiSkill()
    {
        StartCoroutine(UltiSkill());
    }

    IEnumerator UltiSkill()
    {
        //ultimateSkill_UI.DisableGlow();
        //ultimateSkill_UI.BerserkBarZero();

        ultiFX.SetActive(true);

        //Time.timeScale = 0;
        TimeScaleManager.Instance.ApplyHitstop(0, 2.3f);

        yield return new WaitForSecondsRealtime(2);

        ultiFX.SetActive(false);

        ultimateSkillAvailable = false;

        yield return new WaitForSecondsRealtime(.4f);

        //Time.timeScale = 1;

        boxCollider.enabled = true;

        yield return new WaitForSecondsRealtime(.1f);

        boxCollider.enabled = false;
    }
}
