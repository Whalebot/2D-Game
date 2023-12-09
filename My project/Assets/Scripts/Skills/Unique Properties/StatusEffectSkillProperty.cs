using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSkillProperty", menuName = "ScriptableObjects/SkillProperty/StatusEffectSkillProperty")]
public class StatusEffectSkillProperty : UniqueSkillProperty
{
    public MoveGroup affectedMoves;
    public List<StatusEffect> appliedEffects;
    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);

        if (hitInfo.enemyStatus == null)
            return;

        if (!affectedMoves.moves.Contains(hitInfo.move))
            return;

        foreach (var item in appliedEffects)
        {
            hitInfo.enemyStatus.ApplyStatusEffect(item, hitInfo);
        }
    }
}
