using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    [TabGroup("Debug")] public List<SkillSO> allSkills;
    [TabGroup("Debug")] public List<SkillSO> learnedSkills;
    [TabGroup("Debug")] public List<SkillSO> foundSkills;
    [TabGroup("Debug")] public List<SkillSO> selectedSkills;
    [TabGroup("Debug")] public List<SkillSO> items, activeSkills, blessings;
    [TabGroup("Debug")] public List<SkillSO> DSkills, CSkills, BSkills, ASkills, SSkills;
    [TabGroup("Debug")] public SkillSO emptyPoolSkill;
    [TabGroup("Components")] public List<Moveset> allMovesets;
    [TabGroup("Components")] public List<TextTags> colorTags;
    [TabGroup("Components")] public SkillGroupSO attackGroup, specialGroup, skillGroup;
    [TabGroup("Components")] public List<SkillSelectionButton> skillButtons;
    [TabGroup("Components")] public List<ShopButton> shopButtons;
    [TabGroup("Components")] public Sprite NBackground, RBackground, SRBackground, SSRBackground, URBackground;

    public event Action<SkillSO> pickedSkillEvent;

    SkillHandler skillHandler;
    private void Awake()
    {
        Instance = this;
        SaveManager.Instance.saveEvent += SaveSkills;
        foundSkills = SaveManager.Instance.LearnedSkills;
        GameManager.Instance.advanceGameState += ExecuteFrame;
    }

    private void Start()
    {
        skillHandler = GameManager.Instance.player.GetComponent<SkillHandler>();
    }

    void ExecuteFrame()
    {
    }
    public SkillSO CheckReplacementBlessing(SkillSO skill)
    {
        if (attackGroup.skills.Contains(skill))
        {
            for (int i = 0; i < foundSkills.Count; i++)
            {

                if (attackGroup.skills.Contains(foundSkills[i]))
                {
                    return foundSkills[i];
                    //foundSkills[i] = skill;
                }
            }
        }
        if (specialGroup.skills.Contains(skill))
        {
            for (int i = 0; i < foundSkills.Count; i++)
            {

                if (specialGroup.skills.Contains(foundSkills[i]))
                {
                    return foundSkills[i];
                    //foundSkills[i] = skill;
                }
            }
        }
        if (skillGroup.skills.Contains(skill))
        {
            for (int i = 0; i < foundSkills.Count; i++)
            {

                if (skillGroup.skills.Contains(foundSkills[i]))
                {
                    return foundSkills[i];
                    //foundSkills[i] = skill;
                }
            }
        }

        return null;
    }
    #region Roll Skills
    public void ReplaceSkill()
    {

    }
    public void GetSkill(SkillSO skillSO)
    {
        SkillSO replacement = CheckReplacementBlessing(skillSO);
        if (replacement != null)
        {
            learnedSkills.Remove(replacement);
        }

        //else
        {
            pickedSkillEvent?.Invoke(skillSO);
            skillHandler.LearnSkill(skillSO);

            foundSkills.Add(skillSO);
            learnedSkills.Add(skillSO);

            selectedSkills.Clear();
        }
    }
    void SaveSkills()
    {
        SaveManager.Instance.LearnedSkills = new List<SkillSO>(foundSkills);
    }
    [Button]
    public void RollShop(Rank r)
    {
        bool foundRank = false;
        selectedSkills.Clear();
        for (int i = 0; i < shopButtons.Count; i++)
        {
            SkillSO skill = null;

            {
                skill = RollSkill();
                if (skill.skillRank >= r)
                    foundRank = true;
            }
            shopButtons[i].skillSO = skill;
            if (skill != null)
                shopButtons[i].SetupSkill();
        }
    }
    public void RollBlessing(Rank rank)
    {
        selectedSkills.Clear();
        List<SkillSO> eligibleSkills = new List<SkillSO>();
        foreach (var item in allSkills)
        {
            if (item.type == SkillType.Blessing)
            {
                eligibleSkills.Add(item);
            }
        }

        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillSO skill = null;

            skill = RollSkill(eligibleSkills);
            skill.skillRank = (Rank)UnityEngine.Random.Range(0, 5);
            skillButtons[i].skillSO = skill;
            if (skill != null)
                skillButtons[i].SetupSkill();

        }
    }
    public void RollItem(Rank rank)
    {
        selectedSkills.Clear();
        List<SkillSO> eligibleSkills = new List<SkillSO>();
        foreach (var item in allSkills)
        {
            if (item.type == SkillType.Item)
            {
                eligibleSkills.Add(item);
            }
        }

        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillSO skill = null;

            skill = RollSkill(eligibleSkills);
            skillButtons[i].skillSO = skill;
            if (skill != null)
                skillButtons[i].SetupSkill();

        }
    }

    public void RollActiveSkill(Rank rank)
    {
        selectedSkills.Clear();
        List<SkillSO> eligibleSkills = new List<SkillSO>();
        foreach (var item in allSkills)
        {
            if (item.type == SkillType.Skill)
            {
                eligibleSkills.Add(item);
            }
        }

        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillSO skill = null;


            skill = RollSkill(eligibleSkills);

            skillButtons[i].skillSO = skill;
            if (skill != null)
                skillButtons[i].SetupSkill();

        }
    }

    [Button]
    public void RollSkills(Rank r)
    {
        bool foundRank = false;
        selectedSkills.Clear();
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
        selectedSkills.Clear();
    }

    public SkillSO RollSkill(List<SkillSO> skillList)
    {
        List<SkillSO> availableSkills = new List<SkillSO>();
        foreach (var item in skillList)
        {
            if (!foundSkills.Contains(item) && !selectedSkills.Contains(item) && skillHandler.CanGetSkill(item))
                availableSkills.Add(item);
        }

        int RNG = UnityEngine.Random.Range(0, availableSkills.Count);

        if (availableSkills.Count > 0)
        {
            selectedSkills.Add(availableSkills[RNG]);
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
            if (!foundSkills.Contains(item) && !selectedSkills.Contains(item) && skillHandler.CanGetSkill(item))
                availableSkills.Add(item);
        }

        int RNG = UnityEngine.Random.Range(0, availableSkills.Count);
        if (availableSkills.Count > 0)
        {
            selectedSkills.Add(availableSkills[RNG]);
            return availableSkills[RNG];
        }
        else
        {
            Debug.Log("Found all skills");
            return emptyPoolSkill;
        }
    }

    string CheckStringTags(string s)
    {
        List<string> words = new List<string>();
        string test = "";

        //Divide words
        foreach (char letter in s.ToCharArray())
        {
            test += letter;

            if (letter == ' ')
            {
                words.Add(test);
                test = "";
            }
        }

        //Add final word
        words.Add(test);

        //Paint text
        for (int i = 0; i < words.Count; i++)
        {
            foreach (var item in colorTags)
            {
                foreach (string tag in item.tags)
                {
                    if (words[i].ToLower().Contains(tag))
                    {
                        //words[i] = $"<color=#{ColorUtility.ToHtmlStringRGB(item.tagColor)}>" + words[i] + "</color>";
                        //words[i] = $"<color=#{ColorUtility.ToHtmlStringRGB(item.tagColor)}>" + words[i] + "</color>";
                        words[i] = ($"<link=\"0\"><color=#{ColorUtility.ToHtmlStringRGB(item.tagColor)}>" + words[i] + "</color>" + "</link>");
                        //words[i] = ($"<link=0><color=#{ColorUtility.ToHtmlStringRGB(item.tagColor)}>" + words[i] + "</color>" + "</link>");

                        // words[i] = ($"<link='0'><color=#{ColorUtility.ToHtmlStringRGB(item.tagColor)}>" + words[i] + "</color>" + "</link>").Replace("'", "\"");

                    }
                }
            }
        }

        string final = "";

        foreach (string word in words)
        {
            final += word;
        }
        return final;
    }

    public string SkillDescription(SkillSO skill)
    {
        string s = CheckStringTags(skill.description);
        return s;
    }

    public string SkillToolTip()
    {
        return "";
    }

#if UNITY_EDITOR
    [Button]
    void LoadItemSO()
    {
        allSkills.Clear();
        DSkills.Clear();
        CSkills.Clear();
        BSkills.Clear();
        ASkills.Clear();
        SSkills.Clear();

        items.Clear();
        blessings.Clear();
        selectedSkills.Clear();

        string[] skillNames = AssetDatabase.FindAssets("t:SkillSO", new[] { "Assets/Scriptable Objects" });
        foreach (var SOName in skillNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var item = AssetDatabase.LoadAssetAtPath<SkillSO>(SOpath);
            allSkills.Add(item);

            switch (item.type)
            {
                case SkillType.Skill:
                    selectedSkills.Add(item);
                    break;
                case SkillType.Blessing:
                    blessings.Add(item);
                    break;
                case SkillType.Item:
                    items.Add(item);
                    break;
                default:
                    break;
            }

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
    #endregion

}

public enum RewardType
{
    Blessing, Item, Skill, Gold
}

public enum Elemental
{
    None, Earth, Fire, Ice, Lightning, Wind, Poison
}