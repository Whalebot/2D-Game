using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffProperty", menuName = "ScriptableObjects/BuffProperty")]
public class BuffMoveProperty : UniqueSkillProperty
{
    public List<StatusEffect> appliedEffects;
    public override void OnStartupFrame(AttackScript atk, int frame, SkillSO skill = null)
    {
        base.OnStartupFrame(atk, frame);

        foreach (var item in appliedEffects)
        {
            SkillHandler skillHandler = atk.GetComponent<SkillHandler>();
            StatusEffect clone = Instantiate(item);
            Status status = atk.GetComponent<Status> ();
            clone = skillHandler.ModifyStatusEffect(clone);
            if (skill == null)
                status.ApplyStatusEffect(clone, null, Rank.D);
            else
                status.ApplyStatusEffect(clone, null, skill.skillRank);
        }
    }
}
