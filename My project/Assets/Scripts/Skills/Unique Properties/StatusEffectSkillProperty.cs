using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSkillProperty", menuName = "ScriptableObjects/SkillProperty/StatusEffectSkillProperty")]
public class StatusEffectSkillProperty : UniqueSkillProperty
{
    public List<StatusEffect> appliedEffects;
    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);

        if (hitInfo.status == null)
            return;

        foreach (var item in appliedEffects)
        {
           
            if (hitInfo.status.HasStatusEffect(item))
            {
                //Refresh effect
                hitInfo.status.RefreshStatusEffect(item);
            }
            else
                hitInfo.status.ApplyStatusEffect(item);
        }
    }
}