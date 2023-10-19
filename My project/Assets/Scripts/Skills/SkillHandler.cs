using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;

public class SkillHandler : MonoBehaviour
{
    Status status;
    [TabGroup("Skill Info")] public Stats modifiedStats;
    [TabGroup("Skill Info")] public List<SkillSO> learnedSkills;
    [TabGroup("Components")] public AttackScript attackScript;
    [TabGroup("Components")] public DescriptionWindow window;
    [TabGroup("Components")] public GameObject skillLevelUpWindow;
    [TabGroup("Components")] public Transform skillLevelUpPanel;
    [TabGroup("Components")] public GameObject kbQuickslots;

    public delegate void SkillEvent();
    public SkillEvent expEvent;
    public SkillEvent lvlEvent;

    private void Awake()
    {
        learnedSkills = new List<SkillSO>();
        status = GetComponent<Status>();

        SaveManager.Instance.loadEvent += LoadData;
        DuplicateMoveset();
    }
    void Start()
    {
        UpdateStats();
        LateBehaviour();
    }
    void DuplicateMoveset()
    {
        Moveset moveset = Instantiate(attackScript.moveset);
        moveset.name = attackScript.moveset.name;
        Combo skillCombo = Instantiate(moveset.skillCombo);
        skillCombo.name = moveset.skillCombo.name;
        moveset.skillCombo = skillCombo;
        attackScript.moveset = moveset;
    }
    public void ReplaceMove(Move newMove, Move oldMove)
    {
        Moveset def1 = attackScript.moveset;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object var1 = defInfo1[i].GetValue(obj);

            if (var1 is Combo)
            {
                Combo tempCombo = (Combo)var1;
                //Debug.Log(tempCombo);
                for (int j = 0; j < tempCombo.moves.Count; j++)
                {
                    //Debug.Log(tempCombo.moves[j]);
                    if (tempCombo.moves[j].name == oldMove.name)
                    {
                        //Debug.Log(newMove);
                        tempCombo.moves[j] = newMove;
                    }
                }
            }
        }
    }

    void ActivateBehaviour()
    {
    }
    void LateBehaviour()
    {
        foreach (var item in learnedSkills)
        {
            item.LateBehaviour(this);
        }
    }
    void LoadData()
    {
        RemoveAllSkills();

        foreach (var item in SaveManager.Instance.saveData.learnedSkills)
        {
            LearnSkill(item);
        }
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

    public void RemoveAllSkills()
    {
        learnedSkills.Clear();
        status.ReplaceStats(status.currentStats, status.baseStats);
        UpdateStats();
    }

    public void UpdateStats()
    {
        modifiedStats = new Stats();
        modifiedStats.ResetValues();
        foreach (SkillSO skill in learnedSkills)
        {
            if (skill.type == SkillType.Passive)
                CalculateSkillStats(skill);
        }
        ApplySkillEffects();
        status.UpdateStats();
    }

    void UpdateSkillSlot()
    {

    }

    public void SkillLevelUI(Skill skill)
    {
        return;
        GameObject GO = Instantiate(skillLevelUpWindow, skillLevelUpPanel);
        GO.GetComponent<DescriptionWindow>().DisplayUI(skill);

    }
    public bool CanGetSkill(SkillSO skillSO)
    {
        bool hasRequiredSkill = false;
        bool hasRequiredMove = false;

        if (skillSO.prerequisiteSkills.Count > 0)
        {
            foreach (var item in learnedSkills)
            {
                if (item == skillSO)
                {
                    hasRequiredSkill = true;
                }
            }
        }
        else hasRequiredSkill = true;

        Moveset def1 = attackScript.moveset;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        if (skillSO.prerequisiteMoves.Count > 0)
        {
            for (int i = 0; i < defInfo1.Length; i++)
            {
                object obj = def1;
                object var1 = defInfo1[i].GetValue(obj);

                if (var1 is Combo)
                {
                    Combo temp = (Combo)var1;
                    foreach (var move in skillSO.prerequisiteMoves)
                    {
                        foreach (var item in temp.moves)
                        {
                            Debug.Log($"{item.name} {move.name}");
                            if (item.name == move.name)
                                hasRequiredMove = true;
                        }
                    }
                }
            }
        }
        else hasRequiredMove = true;


        return hasRequiredMove && hasRequiredSkill;
    }
    public void LearnSkill(SkillSO skill)
    {
        if (skill != null)
        {
            learnedSkills.Add(skill);
        }

        if (skill.newMoves.Count > 0)
        {
            foreach (var item in skill.newMoves)
            {
                GetCombo(item.combo).moves.Add(item.move);
            }

        }

        expEvent?.Invoke();
        lvlEvent?.Invoke();
        UpdateSkillSlot();
        UpdateStats();
        LateBehaviour();
    }
    public Combo GetCombo(Combo originalCombo)
    {
        Moveset def1 = attackScript.moveset;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object var1 = defInfo1[i].GetValue(obj);

            if (var1 is Combo)
            {
                Combo temp = (Combo)var1;
                if (temp.name == originalCombo.name)
                    return temp;
            }
        }
        return null;
    }

    public void SkillUI(Skill skill)
    {
        return;
        if (skill == null) window.gameObject.SetActive(false);
        else
        {
            window.gameObject.SetActive(true);
            window.DisplayUI(skill);
        }
    }

    public void CalculateSkillStats(SkillSO temp)
    {
        Stats def1 = modifiedStats;
        Stats def2 = temp.stats;
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
                    defInfo1[i].SetValue(obj, (float)var1 + (float)var2);
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
