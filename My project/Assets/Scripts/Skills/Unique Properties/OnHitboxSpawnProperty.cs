using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitboxSpawn", menuName = "ScriptableObjects/SkillProperty/OnHitboxSpawn")]
public class OnHitboxSpawnProperty : UniqueSkillProperty
{
    public Elemental elemental;

    public float baseModifier;
    public float damageModifier;
    public MoveGroup affectedMoves;

    public override float GetModifier(Status status = null, Rank rank = Rank.D)
    {
        Stats stats = status.currentStats;
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
        damageModifier *= baseModifier * RarityModifier(rank);

        return (damageModifier);
    }
    public override void OnHitboxSpawnBehaviour(SkillHandler handler, Rank rank, Hitbox hitbox)
    {
        //base.OnHitboxSpawnBehaviour(handler, rank, hitbox);
        Move move = hitbox.move;

        if (affectedMoves != null)
            if (!affectedMoves.moves.Contains(move))
                return;

        Status attackerStatus = hitbox.status;
        Stats stats = attackerStatus.currentStats;

        hitbox.baseDamage *= (1 + GetModifier(attackerStatus, rank));
    }
}
