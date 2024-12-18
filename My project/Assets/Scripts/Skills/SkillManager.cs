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
    [TabGroup("Debug")] public List<SkillSO> fighterSkills, mageSkills, rogueSkills;
    [TabGroup("Debug")] public SkillSO emptyPoolSkill;
    [TabGroup("Components")] public List<TextTags> colorTags;
    [TabGroup("Components")] public TextTags xbInputTag, psInputTags, kbInputTags, mouseInputTags;
    [TabGroup("Components")] public SkillGroupSO attackGroup, specialGroup, skillGroup;
    [TabGroup("Components")] public List<SkillSelectionButton> skillButtons;

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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.advanceGameState += ExecuteFrame;
            skillHandler = GameManager.Instance.player.GetComponent<SkillHandler>();
        }
    }

    private void Start()
    {

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
    public void RemoveSkill(SkillSO skillToRemove)
    {

        learnedSkills.Remove(skillToRemove);

        pickedSkillEvent?.Invoke(null);
    }

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

    #region Roll Skills

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
    #endregion
    #region String checking
    public string CheckStringTags(string s, SkillSO skill = null)
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
                    if (GameManager.Instance == null)
                        words[i] = skill.CalculateDamageValue(null) + " ";
                    else
                        words[i] = skill.CalculateDamageValue(GameManager.Instance.playerStatus) + " ";
                    words[i] = ($"<link=\"0\"><color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>" + words[i] + "</color>" + "</link>");
                }
                if (words[i].Contains("DMGMOD"))
                {
                    if (GameManager.Instance == null)
                        words[i] = skill.GetModifier(null) + " ";
                    else
                        words[i] = skill.GetModifier(GameManager.Instance.playerStatus) + " ";
                    words[i] = ($"<link=\"0\"><color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>" + words[i] + "</color>" + "</link>");
                }
                if (words[i].Contains("STATS"))
                {
                    words[i] = SkillStatDescription(skill) + " ";
                    //words[i] = ($"<link=\"0\"><color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>" + words[i] + "</color>" + "</link>");
                }
            }


            foreach (var item in colorTags)
            {
                foreach (Tag tagTemplate in item.tags)
                {

                    if (words[i].ToLower().Contains(tagTemplate.tag.ToLower()))
                    {
                        string tempString = words[i].ToLower();
                        tempString = tempString.Replace(tagTemplate.tag.ToLower(), $"<link=\"1\"><color=#{ColorUtility.ToHtmlStringRGB(item.tagColor)}>" + tagTemplate.title + "</color>" + "</link>");
                        words[i] = tempString;
                    }
                }
            }

            TextTags inputTag = null;

            switch (InputManager.controlScheme)
            {
                case ControlScheme.PS4:
                    inputTag = psInputTags;
                    break;
                case ControlScheme.XBOX:
                    inputTag = xbInputTag;
                    break;
                case ControlScheme.Keyboard:
                    inputTag = kbInputTags;
                    break;
                case ControlScheme.Switch:
                    break;
                case ControlScheme.Mouse:
                    inputTag = mouseInputTags;
                    break;
                default:
                    inputTag = kbInputTags;
                    break;
            }

            foreach (Tag tagTemplate in inputTag.tags)
            {

                if (words[i].ToLower().Contains(tagTemplate.tag.ToLower()))
                {
                    string tempString = words[i].ToLower();
                    tempString = tempString.Replace(tagTemplate.tag.ToLower(), tagTemplate.title);
                    words[i] = tempString;
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

    public List<Tag> GetTags(SkillSO skill)
    {
        List<Tag> foundTextTags = new List<Tag>();
        string newString = skill.description;

        List<string> words = new List<string>();
        string test = "";



        //Divide words
        foreach (char letter in newString.ToCharArray())
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
        for (int i = 0; i < words.Count; i++)
        {
            if (words[i].Contains("STATS"))
            {
                words[i] = SkillStatDescription(skill) + " ";
                //words[i] = ($"<link=\"0\"><color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>" + words[i] + "</color>" + "</link>");
            }

            foreach (var item in colorTags)
            {
                foreach (Tag tagTemplate in item.tags)
                {

                    if (words[i].ToLower().Contains(tagTemplate.tag.ToLower()))
                    {
                        foundTextTags.Add(tagTemplate);
                    }
                }
            }
        }

        return foundTextTags;
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
                        d += $"Increase [{defInfo1[i].Name}] with {(int)var1 * blessingModifier} \n";
                    else
                    {
                        d += $"Decrease [{defInfo1[i].Name}] with {(int)var1 * blessingModifier} \n";
                    }

                }

            }
            else if (var1 is float)
            {
                if ((float)var1 != 0)
                {
                    if ((float)var1 > 0)
                    {
                        d += $"Increase [{defInfo1[i].Name}] with ";
                    }
                    else
                    {
                        d += $"Increase [{defInfo1[i].Name}] with ";
                    }
                    string value = (((float)var1 * blessingModifier) * 100).ToString("0.0");
                    d += $"{value}% \n";
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
    #endregion
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

        fighterSkills.Clear();
        mageSkills.Clear();
        rogueSkills.Clear();

        items.Clear();
        blessings.Clear();
        activeSkills.Clear();

        string[] skillNames = AssetDatabase.FindAssets("t:SkillSO", new[] { "Assets/Scriptable Objects" });
        foreach (var SOName in skillNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var item = AssetDatabase.LoadAssetAtPath<SkillSO>(SOpath);
            allSkills.Add(item);

            if (item.allowedCharacters.Contains(CharacterCreator.Instance.characters[0]))
            {
                fighterSkills.Add(item);
            }
            if (item.allowedCharacters.Contains(CharacterCreator.Instance.characters[1]))
            {
                rogueSkills.Add(item);
            }
            if (item.allowedCharacters.Contains(CharacterCreator.Instance.characters[2]))
            {
                mageSkills.Add(item);
            }

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

}

public enum RewardType
{
    Blessing, Item, Skill, Gold
}

public enum Elemental
{
    None, Earth, Fire, Ice, Lightning, Wind, Poison
}