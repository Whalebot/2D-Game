using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnRoomSkillProperty", menuName = "ScriptableObjects/SkillProperty/OnRoomSkillProperty")]
public class OnRoomSkillProperty : UniqueSkillProperty
{
    public Stats recoveredStats;
    public override void WaveBehaviour(SkillHandler handler, SkillSO skill)
    {
        base.WaveBehaviour(handler, skill);
        Status status = handler.GetComponent<Status>();
        float mod = 1f;
        if (skill.type == SkillType.Blessing)
            mod = RarityModifier(skill.skillRank);

        status.AddStats(status.currentStats, recoveredStats, mod);
    }
}
