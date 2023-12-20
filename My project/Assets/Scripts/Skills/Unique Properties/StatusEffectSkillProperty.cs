using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSkillProperty", menuName = "ScriptableObjects/SkillProperty/StatusEffectSkillProperty")]
public class StatusEffectSkillProperty : UniqueSkillProperty
{
    public MoveGroup affectedMoves;
    public List<StatusEffect> appliedEffects;

    public override int GetDamage(Status status = null, Rank rank = Rank.D)
    {

        if (appliedEffects.Count > 0)
        {
            float damageMod = 1;

            switch (appliedEffects[0].elemental)
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

            return (int)(appliedEffects[0].baseDamage * damageMod * RarityModifier(rank));
        }
        else return base.GetDamage();
    }

    public override void HitBehaviour(HitInfo hitInfo, SkillSO skill)
    {
        base.HitBehaviour(hitInfo, skill);

        if (hitInfo.enemyStatus == null)
            return;

        if (!affectedMoves.moves.Contains(hitInfo.move))
            return;

        foreach (var item in appliedEffects)
        {
            SkillHandler skillHandler = hitInfo.attackerStatus.GetComponent<SkillHandler>();
            StatusEffect clone = Instantiate(item);
            clone = skillHandler.ModifyStatusEffect(clone);
            Debug.Log("skill effect Property" + skill.skillRank);
            hitInfo.enemyStatus.ApplyStatusEffect(clone, hitInfo, skill.skillRank);
        }
    }
}
