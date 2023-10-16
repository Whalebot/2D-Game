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
        AIManager.Instance.allEnemiesKilled += SpawnTreasure;
    }

    public void GetSkill(SkillSO skillSO)
    {
        pickedSkillEvent?.Invoke(skillSO);
        GameManager.Instance.player.GetComponent<SkillHandler>().SkillEXP(skillSO);
        SaveManager.Instance.saveData.learnedSkills.Add(skillSO);
        foundSkills.Add(skillSO);
        activeSkills.Clear();
    }
    void SpawnTreasure()
    {
        if (treasure != null)
            treasure.SetActive(true);
    }

    [Button]
    public void RollSkills()
    {
        activeSkills.Clear();
        foreach (var item in skillButtons)
        {
            SkillSO skill = RollSkill();
            while (skill == null && foundSkills.Count < allSkills.Count)
            {
                skill = RollSkill();
            }
            item.skillSO = skill;
            if (skill != null)
                item.SetupSkill();
        }
    }
    [Button]
    public void ResetSkills()
    {
        foundSkills.Clear();
        activeSkills.Clear();
        
    }

    public SkillSO RollSkill()
    {
        int RNG = UnityEngine.Random.Range(0, allSkills.Count);

        if (!foundSkills.Contains(allSkills[RNG]) && !activeSkills.Contains(allSkills[RNG]))
        {
            activeSkills.Add(allSkills[RNG]);
            return allSkills[RNG];
        }
        else if (foundSkills.Count + activeSkills.Count < allSkills.Count)
        {
            return null;
        }
        else
        {
            Debug.Log("Found all");
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
        }
    }
#endif
}
