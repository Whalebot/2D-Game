using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnRoomSkillProperty", menuName = "ScriptableObjects/SkillProperty/OnRoomSkillProperty")]
public class OnRoomSkillProperty : UniqueSkillProperty
{
    public Stats recoveredStats;
    public override void WaveBehaviour(SkillHandler handler)
    {
        base.WaveBehaviour(handler);
        Status status = handler.GetComponent<Status>();
        status.AddStats(status.currentStats, recoveredStats);
    }
}
