﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SkillSO", menuName = "ScriptableObjects/Skill", order = 1)]
[System.Serializable]
public class SkillSO : ScriptableObject
{
    public Rank skillRank;
    public SkillType type;
    public string title;
    [PreviewField(50)] public Sprite sprite;
    public List<SkillSO> prerequisiteSkills;
    public List<Move> prerequisiteMoves;
    public List<NewMove> newMoves;
    public Move upgradedMove;
    public Move oldMove;

    [TextArea(15, 20)]
    public string description;
    [ShowIf("@type == SkillType.Passive || type == SkillType.Item")]
    public Stats stats;

    public virtual void ActivateBehaviour(SkillHandler handler)
    {
        //Debug.Log("Set behaviour");
    }
    public virtual void LateBehaviour(SkillHandler handler)
    {
        if (upgradedMove != null)
            handler.ReplaceMove(upgradedMove, oldMove);
    }
    public virtual void RepeatBehaviour(SkillHandler handler)
    {
        //Debug.Log("Base behaviour");
    }

    public virtual void AttackActiveBehaviour(SkillHandler handler)
    {
        //Debug.Log("Base behaviour");
    }

    public virtual void WaveBehaviour(SkillHandler handler)
    {

        //Debug.Log("Base behaviour");
    }

    public virtual void HitBehaviour(SkillHandler handler)
    {
        //Debug.Log("Base behaviour");
    }

    private void OnValidate()
    {
        title = this.name;
    }
}
[System.Serializable]
public class NewMove {
    public Move move;
    public Combo combo;
}

public enum Rank { D, C, B, A, S }

public enum SkillType { Active, Passive, Item }