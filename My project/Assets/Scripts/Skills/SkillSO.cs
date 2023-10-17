using System.Collections;
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
 [PreviewField(50)]   public Sprite sprite;
    [ShowIf("@type == SkillType.Active")] public Move move;
    [ShowIf("@type == SkillType.Active")] public Combo combo;
    [TextArea(15, 20)]
    public string description;
    [ShowIf("@type == SkillType.Passive || type == SkillType.Item")]
    public Stats stats;

    public virtual void ActivateBehaviour(SkillHandler handler)
    {
        //Debug.Log("Set behaviour");
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

public enum Rank { D, C, B , A , S}

public enum SkillType { Active, Passive, Item }