using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatStatusEffect", menuName = "ScriptableObjects/StatusEffects/StatStatusEffect")]
public class StatStatusEffect : StatusEffect
{
    public Stats stats;

    public override void ActivateBehaviour(Status s, HitInfo hitInfo = null, Rank rank = Rank.D)
    {
        base.ActivateBehaviour(s);
        AddStats();
    }

    public override void RefreshBehaviour()
    {
        base.RefreshBehaviour();
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
