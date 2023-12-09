using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitSkillProperty", menuName = "ScriptableObjects/SkillProperty/OnHitSkillProperty")]
public class OnHitSkillProperty : UniqueSkillProperty
{
    public Elemental elemental;

    public int baseDamage;
    public float damageModifier;
    public MoveGroup affectedMoves;
    public ChainLightning chainLightning;
    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);

        if (hitInfo.enemyStatus == null)
            return;

        if (!affectedMoves.moves.Contains(hitInfo.move))
            return;

        ChainLightning lightning = Instantiate(chainLightning, hitInfo.enemyStatus.transform.position, Quaternion.identity);

        if (hitInfo != null)
        {
            Stats stats = hitInfo.attackerStatus.currentStats;
            switch (elemental)
            {
                case Elemental.None:
                    break;
                case Elemental.Earth:
                    damageModifier = stats.faithModifier * stats.earthModifier;
                    break;
                case Elemental.Fire:
                    damageModifier = stats.faithModifier * stats.fireModifier;
                    break;
                case Elemental.Ice:
                    damageModifier = stats.faithModifier * stats.iceModifier;
                    break;
                case Elemental.Lightning:
                    damageModifier = stats.faithModifier * stats.lightningModifier;
                    break;
                case Elemental.Wind:
                    damageModifier = stats.faithModifier * stats.windModifier;
                    break;
                case Elemental.Poison:
                    damageModifier = stats.faithModifier * stats.poisonModifier;
                    break;
                default:
                    break;
            }
        }

        lightning.damage = (int)(baseDamage * damageModifier);
        lightning.StartChainLightning(hitInfo.enemyStatus);
    }
}
