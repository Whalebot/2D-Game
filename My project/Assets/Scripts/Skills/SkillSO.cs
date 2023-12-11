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
    [PreviewField(50)] public Sprite sprite;

    public int damageValue = 0;

    public List<SkillSO> prerequisiteSkills;
    [ShowIf("@type == SkillType.Skill")] public List<Move> prerequisiteMoves;
    [ShowIf("@type == SkillType.Skill")] public List<Move> bannedMoves;
    [ShowIf("@type == SkillType.Skill")] public List<NewMove> newMoves;
    public List<UniqueSkillProperty> skillProperties;

    [TextArea(15, 20)]
    public string description;
    [ShowIf("@type == SkillType.Item")]
    public Stats stats;

    public virtual void ActivateBehaviour(SkillHandler handler)
    {
        //Debug.Log("Set behaviour");
    }
    public virtual void LateBehaviour(SkillHandler handler)
    {
        if (newMoves.Count > 0)
        {

            foreach (var item in newMoves)
            {
                if (item.oldMove != null)
                    handler.ReplaceMove(item.move, item.oldMove);
            }
        }
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

    public void CalculateDamageValue(Status status)
    {
        switch (type)
        {
            case SkillType.Skill:
                //Find new move hitbox damage
                if (newMoves.Count > 0)
                {
                    damageValue = (int)(status.Attack * newMoves[0].move.attacks[0].damage);
                }
                break;
            case SkillType.Blessing:
                if (skillProperties.Count > 0)
                {
                    StatusEffectSkillProperty prop = (StatusEffectSkillProperty)skillProperties[0];
                    float damageMod = 1f;
                    switch (prop.appliedEffects[0].elemental)
                    {
                        case Elemental.None:
                            break;
                        case Elemental.Earth:
                            damageMod = status.currentStats.faithModifier * status.currentStats.earthModifier;
                            break;
                        case Elemental.Fire:
                            damageMod = status.currentStats.faithModifier * status.currentStats.fireModifier;
                                break;
                        case Elemental.Ice:
                            damageMod = status.currentStats.faithModifier * status.currentStats.iceModifier;
                            break;
                        case Elemental.Lightning:
                            damageMod = status.currentStats.faithModifier * status.currentStats.lightningModifier;
                            break;
                        case Elemental.Wind:
                            damageMod = status.currentStats.faithModifier * status.currentStats.windModifier;
                            break;
                        case Elemental.Poison:
                            damageMod = status.currentStats.faithModifier * status.currentStats.poisonModifier;
                            break;
                        default:
                            break;
                    }
                    damageValue = (int)(prop.appliedEffects[0].baseDamage * damageMod);
           
                }
                break;
            case SkillType.Item:
                break;
            default:
                break;
        }

    }

    private void OnValidate()
    {
        title = this.name;
    }
}
[System.Serializable]
public class NewMove
{
    public Move move;
    public Combo combo;
    public Move oldMove;

}

public enum Rank { D, C, B, A, S }

public enum SkillType { Skill, Blessing, Item }