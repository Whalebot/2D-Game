using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    public List<SkillSO> allSkills;
    public List<SkillSO> foundSkills;
    public List<SkillSO> DSkills, CSkills, BSkills, ASkills, SSkills;
    public List<SkillSO> activeSkills;

    public List<SkillSelectionButton> skillButtons;
    public GameObject treasure;
    public Sprite NBackground, RBackground, SRBackground, SSRBackground, URBackground;

    public event Action<SkillSO> pickedSkillEvent;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //AIManager.Instance.allEnemiesKilled += RollSkills;
    }

    public void GetSkill(SkillSO skillSO)
    {
        pickedSkillEvent?.Invoke(skillSO);
        GameManager.Instance.player.GetComponent<SkillHandler>().LearnSkill(skillSO);
        SaveManager.Instance.saveData.learnedSkills.Add(skillSO);
        foundSkills.Add(skillSO);
        activeSkills.Clear();
    }


    [Button]
    public void RollSkills(Rank r)
    {
        bool foundRank = false;
        activeSkills.Clear();
        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillSO skill = null;
            //If last skill and haven't found sufficiently high skill
            if (!foundRank && i == skillButtons.Count - 1)
            {
                switch (r)
                {
                    case Rank.D:
                        skill = RollSkill(DSkills);
                        break;
                    case Rank.C:
                        skill = RollSkill(CSkills);
                        break;
                    case Rank.B:
                        skill = RollSkill(BSkills);
                        break;
                    case Rank.A:
                        skill = RollSkill(ASkills);
                        break;
                    case Rank.S:
                        skill = RollSkill(SSkills);
                        break;
                    default:
                        break;
                }

                if (skill.skillRank >= r)
                    foundRank = true;

                skillButtons[i].skillSO = skill;
                if (skill != null)
                    skillButtons[i].SetupSkill();
            }
            else
            {
                skill = RollSkill();
                if (skill.skillRank >= r)
                    foundRank = true;
            }
            skillButtons[i].skillSO = skill;
            if (skill != null)
                skillButtons[i].SetupSkill();
        }
    }
    [Button]
    public void ResetSkills()
    {
        foundSkills.Clear();
        activeSkills.Clear();
    }

    public SkillSO RollSkill(List<SkillSO> skillList)
    {
        List<SkillSO> availableSkills = new List<SkillSO>();
        foreach (var item in skillList)
        {
            if (!foundSkills.Contains(item) && !activeSkills.Contains(item))
                availableSkills.Add(item);
        }

        int RNG = UnityEngine.Random.Range(0, availableSkills.Count);


        if (availableSkills.Count > 0)
        {
            activeSkills.Add(availableSkills[RNG]);
            return availableSkills[RNG];
        }
        else
        {
            Debug.Log("Found from this rank, rolling generic");
            return RollSkill();
        }
    }

    public SkillSO RollSkill()
    {
        List<SkillSO> availableSkills = new List<SkillSO>();
        foreach (var item in allSkills)
        {
            if (!foundSkills.Contains(item) && !activeSkills.Contains(item))
                availableSkills.Add(item);
        }

        int RNG = UnityEngine.Random.Range(0, availableSkills.Count);
        if (availableSkills.Count > 0)
        {
            activeSkills.Add(availableSkills[RNG]);
            return availableSkills[RNG];
        }
        else
        {
            Debug.Log("Found all skills");
            return null;
        }
    }

#if UNITY_EDITOR
    [Button]
    void LoadItemSO()
    {
        allSkills.Clear();
        string[] skillNames = AssetDatabase.FindAssets("t:SkillSO", new[] { "Assets/Scriptable Objects" });
        foreach (var SOName in skillNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var item = AssetDatabase.LoadAssetAtPath<SkillSO>(SOpath);
            allSkills.Add(item);
            switch (item.skillRank)
            {
                case Rank.D:
                    DSkills.Add(item);
                    break;
                case Rank.C:
                    CSkills.Add(item);
                    break;
                case Rank.B:
                    BSkills.Add(item);
                    break;
                case Rank.A:
                    ASkills.Add(item);
                    break;
                case Rank.S:
                    SSkills.Add(item);
                    break;
                default:
                    break;
            }
        }
    }
#endif
}
