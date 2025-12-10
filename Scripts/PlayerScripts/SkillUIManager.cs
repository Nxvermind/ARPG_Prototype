using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public Image skillImage;
        public float skillCooldownTime;
        public bool isSkillReady;
    }

    public Dictionary<string, SkillData> data = new Dictionary<string, SkillData>();

    public List<SkillData> skillDataList;


    void Start()
    {
        foreach(SkillData skill in skillDataList)
        {
            if (!data.ContainsKey(skill.skillName))
            {
                data.Add(skill.skillName, skill);
                skill.isSkillReady = true;
            }
        }
    }

    public void StartSkillCooldown(string _skillName)
    {
        if (data.ContainsKey(_skillName))
        {
            SkillData skill = data[_skillName];

            if (skill.isSkillReady)
            {
                StartCoroutine(SkillCooldown(skill));
            }
        }
    }

    public IEnumerator SkillCooldown(SkillData _skillData)
    {
        _skillData.isSkillReady = false;
        _skillData.skillImage.fillAmount = 0;
        float elapsedTime = 0;
        float start = 0;
        float end = 1;

        while (elapsedTime < _skillData.skillCooldownTime)
        {
            elapsedTime += Time.deltaTime;
            _skillData.skillImage.fillAmount = Mathf.Lerp(start, end, elapsedTime / _skillData.skillCooldownTime);
            yield return null;
        }

        _skillData.skillImage.fillAmount = end;
        _skillData.isSkillReady = true;

    }

    public void ResetSkills()
    {
        StopAllCoroutines();

        foreach(var skill in skillDataList)
        {
            skill.skillImage.fillAmount = 1;
            skill.isSkillReady = true;
        }
    }
}
