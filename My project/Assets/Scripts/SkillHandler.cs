using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;

public class SkillHandler : MonoBehaviour
{
    Status status;
    [TabGroup("Skill Info")] public Stats modifiedStats;
    [TabGroup("Skill Info")] public List<Skill> learnedSkills;
    [TabGroup("Components")] public DescriptionWindow window;
    [TabGroup("Components")] public GameObject skillLevelUpWindow;
    [TabGroup("Components")] public Transform skillLevelUpPanel;
    [TabGroup("Components")] public GameObject kbQuickslots;

    public delegate void SkillEvent();
    public SkillEvent expEvent;
    public SkillEvent lvlEvent;

    private void Awake()
    {
    }

    void Start()
    {
        learnedSkills = new List<Skill>();
        status = GetComponent<Status>();
        UpdateStats();
    }

    private void OnEnable()
    {

    }

    void SaveSkill(SkillSO skill)
    {
        //if (skill == null) return;
        //SkillData data = new SkillData
        //{
        //    skillID = ItemDatabase.Instance.GetSkillID(skill),
        //    level = skill.level,
        //    experience = skill.experience
        //};
        //DataManager.Instance.currentSaveData.skillData.Add(data);
    }

    void LoadSkills()
    {
        //foreach (SkillData data in DataManager.Instance.currentSaveData.skillData)
        //{
        //    Skill skill = ItemDatabase.Instance.GetSkill(data.skillID);
        //    skill.level = data.level;
        //    skill.experience = data.experience;
        //}
    }

    public void UpdateStats()
    {
        modifiedStats = new Stats();
        modifiedStats.ResetValues();
        foreach (Skill skill in learnedSkills)
        {
            if (skill.skillSO.type == SkillType.Passive)
                CalculateSkillStats(skill);
        }
        ApplySkillEffects();
    }

    void UpdateSkillSlot()
    {

    }

    public void SkillLevelUI(Skill skill)
    {
        GameObject GO = Instantiate(skillLevelUpWindow, skillLevelUpPanel);
        GO.GetComponent<DescriptionWindow>().DisplayUI(skill);

    }

    public Skill GetSkill(SkillSO so)
    {

        foreach (var item in learnedSkills)
        {
            if (item.skillSO == so)
            {
                return item;
            }
        }
        return null;
    }

    public void SkillEXP(SkillSO skill, int exp)
    {

        Skill temp = null;
        int tempExp = exp;
        foreach (var item in learnedSkills)
        {
            if (item.skillSO == skill)
            {
                temp = item;
            }
        }
        if (temp == null)
        {
            temp = new Skill();
            temp.skillSO = skill;
            learnedSkills.Add(temp);
        }
        if (temp.level >= 10)
        {
            temp.experience = (int)Mathf.Pow(1.5F, temp.level) * 100;
            return;
        }

        expEvent?.Invoke();

        while (tempExp > 0)
        {
            int expToLevel = (int)(Mathf.Pow(1.5F, temp.level) * 100);
            //If level up
            if (temp.experience + tempExp > expToLevel || temp.level == 0)
            {
                // Debug.Log($"{temp.level} {temp.experience} {expToLevel}");
                temp.level++;
                tempExp -= (expToLevel - temp.experience);
                temp.experience = 0;
                lvlEvent?.Invoke();
                SkillLevelUI(temp);
                UpdateSkillSlot();
                UpdateStats();
                if (temp.level >= 10)
                {
                    temp.experience = (int)Mathf.Pow(1.5F, temp.level) * 100;
                    return;
                }
            }
            //No level up
            else
            {
                temp.experience += tempExp;
                tempExp = 0;
            }
        }
    }

    public void SkillUI(Skill skill)
    {
        if (skill == null) window.gameObject.SetActive(false);
        else
        {
            window.gameObject.SetActive(true);
            window.DisplayUI(skill);
        }
    }

    public void CalculateSkillStats(Skill temp)
    {
        Stats def1 = modifiedStats;
        Stats def2 = temp.skillSO.stats;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;

            object var1 = defInfo1[i].GetValue(obj);
            object var2 = defInfo2[i].GetValue(obj2);

            if (var1 is int)
            {
                if ((int)var2 != 0)
                    defInfo1[i].SetValue(obj, (int)var1 + (int)var2);
            }
            else if (var1 is float)
            {
                if ((float)var2 != 0)
                    defInfo1[i].SetValue(obj, (float)var1 + (float)var2 * temp.level / 10);
            }
        }
    }

    public void ApplySkillEffects()
    {
        Stats def1 = status.currentStats;
        Stats def2 = modifiedStats;
        Stats def3 = status.baseStats;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();
        FieldInfo[] defInfo3 = def3.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;
            object obj3 = def3;

            object var1 = defInfo1[i].GetValue(obj);
            object var2 = defInfo2[i].GetValue(obj2);
            object var3 = defInfo3[i].GetValue(obj3);

            if (var1 is int)
            {
                if ((int)var2 != 0)
                    defInfo1[i].SetValue(obj, (int)var3 + (int)var2);
            }
            else if (var1 is float)
            {
                if ((float)var2 != 0)
                    defInfo1[i].SetValue(obj, (float)var3 + (float)var2);
            }
            else if (var1 is bool)
            {
                //SET VALUES
                if ((bool)var2)
                    defInfo1[i].SetValue(obj, defInfo2[i].GetValue(obj2));
            }
        }
    }
}
