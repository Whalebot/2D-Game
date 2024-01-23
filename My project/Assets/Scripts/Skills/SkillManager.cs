using System;
using System.Reflection;
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
    [TabGroup("Debug")] public SkillSO emptyPoolSkill;
    [TabGroup("Components")] public List<Moveset> allMovesets;
    [TabGroup("Components")] public List<TextTags> colorTags;
    [TabGroup("Components")] public SkillGroupSO attackGroup, specialGroup, skillGroup;
    [TabGroup("Components")] public List<SkillSelectionButton> skillButtons;
    [TabGroup("Components")] public List<ShopButton> shopItems;
    [TabGroup("Components")] public List<ShopButton> shopBlessings;
    [TabGroup("Components")] public List<ShopButton> shopSkills;
    [TabGroup("Components")] public Sprite NBackground, RBackground, SRBackground, SSRBackground, URBackground;
    RewardType rewardType;
    Rank rewardRank;
    public event Action<SkillSO> pickedSkillEvent;

    SkillHandler skillHandler;
    private void Awake()
    {
        Instance = this;
        SaveManager.Instance.saveEvent += SaveSkills;
        learnedSkills = SaveManager.Instance.LearnedSkills;
        foundSkills = SaveManager.Instance.FoundSkills;
        GameManager.Instance.advanceGameState += ExecuteFrame;
    }

    private void Start()
    {
        skillHandler = GameManager.Instance.player.GetComponent<SkillHandler>();
    }

    void ExecuteFrame()
    {
    }
    public SkillSO CheckUpgrade(SkillSO skill)
    {
        if (learnedSkills.Contains(skill))
        {
            return skill;
        }
        return null;
    }

    public SkillSO CheckReplacementBlessing(SkillSO skill)
    {
        if (attackGroup.skills.Contains(skill))
        {
            for (int i = 0; i < learnedSkills.Count; i++)
            {

                if (attackGroup.skills.Contains(learnedSkills[i]))
                {
                    return learnedSkills[i];
                    //foundSkills[i] = skill;
                }
            }
        }
        if (specialGroup.skills.Contains(skill))
        {
            for (int i = 0; i < learnedSkills.Count; i++)
            {

                if (specialGroup.skills.Contains(learnedSkills[i]))
                {
                    return learnedSkills[i];
                    //foundSkills[i] = skill;
                }
            }
        }
        if (skillGroup.skills.Contains(skill))
        {
            for (int i = 0; i < learnedSkills.Count; i++)
            {

                if (skillGroup.skills.Contains(learnedSkills[i]))
                {
                    return learnedSkills[i];
                    //foundSkills[i] = skill;
                }
            }
        }

        return null;
    }
    #region Roll Skills
    public void GetSkill(SkillSO skillSO)
    {
        SkillSO replacement = CheckReplacementBlessing(skillSO);
        if (replacement != null)
        {
            foundSkills.Add(replacement);
            learnedSkills.Remove(replacement);
        }

        SkillSO upgrade = CheckUpgrade(skillSO);
        if (upgrade != null)
        {
            skillSO.skillRank = skillSO.skillRank + 1;
        }
        //else
        {
            skillHandler.LearnSkill(skillSO);

            //If blessing, don't add to found list
            if (skillSO.type != SkillType.Blessing || skillSO.skillRank == Rank.S)
                foundSkills.Add(skillSO);

            learnedSkills.Add(skillSO);

            selectedSkills.Clear();
            pickedSkillEvent?.Invoke(skillSO);
        }
    }
    void SaveSkills()
    {
        SaveManager.Instance.LearnedSkills = new List<SkillSO>(learnedSkills);
    }

    [Button]
    public void Reroll()
    {
        switch (rewardType)
        {
            case RewardType.Blessing:
                RollBlessing(rewardRank);
                break;
            case RewardType.Item:
                RollItem(rewardRank);
                break;
            case RewardType.Skill:
                RollActiveSkill(rewardRank);
                break;
            case RewardType.Gold:
                break;
            default:
                break;
        }
    }

    [Button]
    public void RollShop(Rank r)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            SkillSO skill = null;
            skill = RollSkill(items);
            shopItems[i].skillSO = skill;
            if (skill != null)
                shopItems[i].SetupSkill();
        }

        for (int i = 0; i < shopBlessings.Count; i++)
        {
            SkillSO skill = null;
            skill = RollSkill(blessings);
            shopBlessings[i].skillSO = skill;
            if (skill != null)
                shopBlessings[i].SetupSkill();
        }

        for (int i = 0; i < shopSkills.Count; i++)
        {
            SkillSO skill = null;
            skill = RollSkill(activeSkills);
            shopSkills[i].skillSO = skill;
            if (skill != null)
                shopSkills[i].SetupSkill();
        }
    }
    public void RollBlessing(Rank rank)
    {
        rewardRank = rank;
        rewardType = RewardType.Blessing;
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
            skill.skillRank = rank;
            //skill.skillRank = (Rank)UnityEngine.Random.Range(0, 5);
            skillButtons[i].skillSO = skill;
            if (skill != null)
                skillButtons[i].SetupSkill();
        }
        SetActiveEventSystem();
    }

    void SetActiveEventSystem()
    {
        UIManager.Instance.SetActiveEventSystem(skillButtons[1].gameObject);
    }

    public void RollItem(Rank rank)
    {
        rewardType = RewardType.Item;
        selectedSkills.Clear();
        rewardRank = rank;
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
        SetActiveEventSystem();
    }

    public void RollActiveSkill(Rank rank)
    {
        rewardType = RewardType.Skill;
        selectedSkills.Clear();
        rewardRank = rank;
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
        SetActiveEventSystem();
    }

    [Button]
    public void ResetSkills()
    {
        learnedSkills.Clear();
        foundSkills.Clear();
        selectedSkills.Clear();
    }

    public SkillSO RollSkill(List<SkillSO> skillList)
    {
        List<SkillSO> availableSkills = new List<SkillSO>();
        foreach (var item in skillList)
        {
            if (!learnedSkills.Contains(item) && !foundSkills.Contains(item) && !selectedSkills.Contains(item) && skillHandler.CanGetSkill(item))
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
            return emptyPoolSkill;
        }
    }

    public SkillSO RollSkill()
    {
        List<SkillSO> availableSkills = new List<SkillSO>();
        foreach (var item in allSkills)
        {
            if (!learnedSkills.Contains(item) && !selectedSkills.Contains(item) && skillHandler.CanGetSkill(item))
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

    string CheckStringTags(string s, SkillSO skill = null)
    {
        List<string> words = new List<string>();
        string test = "";

        //Divide words
        foreach (char letter in s.ToCharArray())
        {
            test += letter;

            if (letter == ' ' || letter == '\n')
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
            if (skill != null)
            {
                if (words[i].Contains("DMGVAL"))
                {
                    words[i] = skill.CalculateDamageValue(GameManager.Instance.playerStatus) + " ";
                    words[i] = ($"<link=\"0\"><color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>" + words[i] + "</color>" + "</link>");
                }
                if (words[i].Contains("STATS"))
                {
                    words[i] = SkillStatDescription(skill) + " ";
                    words[i] = ($"<link=\"0\"><color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>" + words[i] + "</color>" + "</link>");
                }
            }


            foreach (var item in colorTags)
            {
                foreach (string tag in item.tags)
                {
                    if (words[i].ToLower().Contains(tag))
                    {
                        words[i] = words[i].Replace("(", "");
                        words[i] = words[i].Replace(")", "");
                        words[i] = ($"<link=\"1\"><color=#{ColorUtility.ToHtmlStringRGB(item.tagColor)}>" + words[i] + "</color>" + "</link>");

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

    public string SkillStatDescription(SkillSO skill)
    {
        string d = "";
        Stats def1 = skill.stats;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();

        float blessingModifier = 1;
        if (skill.type == SkillType.Blessing)
            switch (skill.skillRank)
            {
                case Rank.D:
                    blessingModifier = 1;
                    break;
                case Rank.C:
                    blessingModifier = 1.5f;
                    break;
                case Rank.B:
                    blessingModifier = 1.75f;
                    break;
                case Rank.A:
                    blessingModifier = 2;
                    break;
                case Rank.S:
                    blessingModifier = 2.5f;
                    break;
                default:
                    break;
            }

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;

            object var1 = defInfo1[i].GetValue(obj);

            if (var1 is int)
            {
                if ((int)var1 != 0)
                {
                    if ((int)var1 > 0)
                        d += $"Increase {defInfo1[i].Name} with {(int)var1 * blessingModifier} \n";
                    else
                    {
                        d += $"Decrease {defInfo1[i].Name} with {(int)var1 * blessingModifier} \n";
                    }

                }

            }
            else if (var1 is float)
            {
                if ((float)var1 != 0)
                {
                    if ((float)var1 > 0)
                        d += $"Increase {defInfo1[i].Name} with {(float)var1 * blessingModifier} \n";
                    else
                    {
                        d += $"Decrease {defInfo1[i].Name} with {(float)var1 * blessingModifier} \n";
                    }
                }
            }

        }
        return d;
    }

    public string SkillDescription(SkillSO skill)
    {
        string s = CheckStringTags(skill.description, skill);
        return s;
    }

    public string SkillToolTip()
    {
        return "";
    }
    #region RNG calculation


    [Button]
    public void CalculateRNG()
    {
        CalculateAllRNG();
        CalculateTypeRNG(items);
        CalculateTypeRNG(activeSkills);
        CalculateTypeRNG(blessings);
    }
    public void CalculateAllRNG()
    {
        List<SkillSO> dropTable = new List<SkillSO>();

        int totalWeight = 0;
        foreach (var item in allSkills)
        {
            totalWeight += item.rngWeight;
            for (int i = 0; i < item.rngWeight; i++)
            {
                dropTable.Add(item);
            }
        }

        foreach (var item in allSkills)
        {
            item.allChance = 100 * (float)item.rngWeight / (float)totalWeight;
        }

        Debug.Log(dropTable[UnityEngine.Random.Range(0, dropTable.Count)]);
    }
    public void CalculateTypeRNG(List<SkillSO> skillPool)
    {
        List<SkillSO> dropTable = new List<SkillSO>();

        int totalWeight = 0;
        foreach (var item in skillPool)
        {
            totalWeight += item.rngWeight;
            for (int i = 0; i < item.rngWeight; i++)
            {
                dropTable.Add(item);
            }
        }

        foreach (var item in skillPool)
        {
            item.typeChance = 100 * (float)item.rngWeight / (float)totalWeight;
        }

        Debug.Log(dropTable[UnityEngine.Random.Range(0, dropTable.Count)]);
    }


    #endregion
#if UNITY_EDITOR
    [Button]
    void LoadItemSO()
    {
        allSkills.Clear();

        items.Clear();
        blessings.Clear();
        activeSkills.Clear();

        string[] skillNames = AssetDatabase.FindAssets("t:SkillSO", new[] { "Assets/Scriptable Objects" });
        foreach (var SOName in skillNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var item = AssetDatabase.LoadAssetAtPath<SkillSO>(SOpath);
            allSkills.Add(item);

            switch (item.type)
            {
                case SkillType.Skill:
                    activeSkills.Add(item);
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