using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatStatusEffect", menuName = "ScriptableObjects/StatusEffects/StatStatusEffect")]
public class StatStatusEffect : StatusEffect
{
    public Stats stats;

    public override void ActivateBehaviour(Status s, HitInfo hitInfo = null, Rank rank = Rank.D)
    {
        base.ActivateBehaviour(s, hitInfo, rank);
        AddStats();
    }

    public override void RefreshBehaviour(Status s, HitInfo hitInfo = null, Rank rank = Rank.D)
    {
        base.RefreshBehaviour(s, hitInfo, rank);
    }

    void AddStats()
    {

        status.AddStats(status.currentStats, stats);
    }

    void RemoveStats()
    {
        status.RemoveStats(status.currentStats, stats);
    }
    public override void EndBehaviour()
    {
        base.EndBehaviour();
        RemoveStats();
    }
}
