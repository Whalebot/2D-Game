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

    private void Awake()
    {
        learnedSkills = new List<SkillSO>();
        status = GetComponent<Status>();

        SaveManager.Instance.startLoadEvent += LoadData;
        GameManager.Instance.advanceGameState += ExecuteFrame;
        DuplicateMoveset();
    }
    void Start()
    {
        //UpdateStats();
        LateBehaviour();
        attackScript.hitEvent += OnHitBehaviour;
        WaveBeahviour();
    }
    private void OnDisable()
    {
        SaveManager.Instance.startLoadEvent -= LoadData;
        GameManager.Instance.advanceGameState -= ExecuteFrame;
    }
    void ExecuteFrame()
    {
        if (attackScript.attacking)
        {
            foreach (var item in learnedSkills)
            {
                foreach (var prop in item.skillProperties)
                {
                    prop.AttackingBehaviour(this);
                }
            }
        }
    }
    void DuplicateMoveset()
    {
        Moveset moveset = Instantiate(attackScript.moveset);
        moveset.name = attackScript.moveset.name;
        DuplicateCombos(moveset);
        attackScript.moveset = moveset;
    }
    void DuplicateCombos(Moveset moveset)
    {
        Moveset def1 = moveset;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object var1 = defInfo1[i].GetValue(obj);

            if (var1 is Combo)
            {
                Combo tempCombo = (Combo)var1;
                Combo newCombo = Instantiate(tempCombo);
                newCombo.name = tempCombo.name;
                defInfo1[i].SetValue(obj, newCombo);
            }
        }
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
                    if (tempCombo.moves[j].name == oldMove.name)
                    {
                        tempCombo.moves[j] = newMove;
                    }
                }
            }
        }
    }
    void WaveBeahviour()
    {
        foreach (var item in learnedSkills)
        {
            item.WaveBehaviour(this);
            foreach (var prop in item.skillProperties)
            {
                prop.WaveBehaviour(this, item);
            }
        }
    }
    void ActivateBehaviour()
    {
    }
    public StatusEffect ModifyStatusEffect(StatusEffect statusEffect)
    {
        foreach (var item in learnedSkills)
        {
            foreach (var prop in item.skillProperties)
            {
                if (prop.GetType() == typeof(ModifyEffectSkillProperty))
                {
                    ModifyEffectSkillProperty mod = (ModifyEffectSkillProperty)prop;
                    mod.ModifyEffect(statusEffect);
                }
            }
        }

        return statusEffect;
    }
    void LateBehaviour()
    {
        foreach (var item in learnedSkills)
        {
            item.LateBehaviour(this);
        }
    }
    void OnHitBehaviour(HitInfo hitInfo)
    {
        foreach (var item in learnedSkills)
        {
            foreach (var prop in item.skillProperties)
            {

                prop.HitBehaviour(hitInfo, item);
            }
        }
    }
    void LoadData()
    {
        RemoveAllSkills();

        foreach (var item in SaveManager.Instance.LearnedSkills)
        {
            LearnSkill(item, true);
        }
    }

    public void RemoveAllSkills()
    {
        learnedSkills.Clear();
        status.RestoreStats();
        UpdateStats();
    }

    public void UpdateStats(bool loading = false)
    {
        modifiedStats = new Stats();
        modifiedStats.ResetValues();
        foreach (SkillSO skill in learnedSkills)
        {
            if (skill.type != SkillType.Skill)
                CalculateSkillStats(skill);
        }
        ApplySkillEffects(loading);
        status.UpdateStats();
    }


    public bool CanGetSkill(SkillSO skillSO)
    {
        bool hasRequiredSkill = false;
        bool hasBannedMove = false;
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
        if (skillSO.newMoves.Count > 0)
        {
            for (int i = 0; i < defInfo1.Length; i++)
            {
                object obj = def1;
                object var1 = defInfo1[i].GetValue(obj);
                if (var1 is Combo)
                {
                    Combo temp = (Combo)var1;
                    foreach (var item in skillSO.bannedMoves)
                    {
                        foreach (var oldMove in temp.moves)
                        {
                            if (item.name == oldMove.name)
                                hasBannedMove = true;
                        }
                    }

                    foreach (var newMove in skillSO.newMoves)
                    {
                        if (newMove.oldMove != null)
                            foreach (var item in temp.moves)
                            {
                                if (item.name == newMove.oldMove.name)
                                    hasRequiredMove = true;
                            }

                        if (newMove.combo != null)
                            if (temp.name == newMove.combo.name)
                                hasRequiredMove = true;
                    }
                }
            }
        }
        else hasRequiredMove = true;

        //Debug.Log($"{skillSO} Has required move {hasRequiredMove} & has required skill{hasRequiredSkill}");

        return hasRequiredMove && hasRequiredSkill && !hasBannedMove;
    }
    public void LearnSkill(SkillSO skill, bool loading = false)
    {
        if (skill == null)
        {
            Debug.Log("null skill");
            return;
        }
        if (skill != null)
        {
            learnedSkills.Add(skill);
        }

        if (skill.newMoves.Count > 0)
        {
            foreach (var item in skill.newMoves)
            {
                if (item.combo != null)
                {
                    Combo c = GetCombo(item.combo);
                    if (c != null)
                        c.moves.Add(item.move);
                }
            }
        }

        if (!loading)
        {
            status.Health += skill.stats.maxHealth;
            status.Meter += skill.stats.maxMeter;
        }
        UpdateStats(loading);
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
    public void CalculateSkillStats(SkillSO temp)
    {
        Stats def1 = modifiedStats;
        Stats def2 = temp.stats;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();

        float blessingModifier = 1;
        if (temp.type == SkillType.Blessing)
            switch (temp.skillRank)
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
            object obj2 = def2;

            object var1 = defInfo1[i].GetValue(obj);
            object var2 = defInfo2[i].GetValue(obj2);

            if (var1 is int)
            {
                if ((int)var2 != 0)
                {
                    if (defInfo1[i].Name != "currentHealth" && defInfo1[i].Name != "currentMeter")
                        defInfo1[i].SetValue(obj, (int)var1 + (int)(((int)var2) * blessingModifier));
                }

            }
            else if (var1 is float)
            {
                if ((float)var2 != 0)
                    defInfo1[i].SetValue(obj, (float)var1 + ((float)var2) * blessingModifier);
            }
        }
    }
    public void ApplySkillEffects(bool loading = false)
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
                {
                    if (defInfo1[i].Name == "currentHealth" || defInfo1[i].Name == "currentMeter")
                    {
                        if (!loading)
                            defInfo1[i].SetValue(obj, (int)var1 + (int)var2);
                    }
                    else
                        defInfo1[i].SetValue(obj, (int)var3 + (int)var2);
                }
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
