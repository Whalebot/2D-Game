using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectProperty", menuName = "MoveUniqueProperty/StatusEffectProperty")]
public class StatusEffectProperty : MoveUniqueProperty
{
    public StatusEffect statusEffect;

    public override void OnStartupFrame(AttackScript atk, int frame)
    {
        base.OnStartupFrame(atk, frame);
        if (propertyType == UniquePropertyType.StartupFrame)
            atk.status.ApplyStatusEffect(statusEffect);
    }

    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);
        if (propertyType == UniquePropertyType.OnHit)
            hitInfo.enemyStatus.ApplyStatusEffect(statusEffect);
    }
}
