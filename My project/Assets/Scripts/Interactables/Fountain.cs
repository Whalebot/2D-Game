using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fountain : Interactable
{
    public Stats stats;
    public override void South()
    {
        base.South();
        GiveStats();
        gameObject.SetActive(false);
    }

    public void GiveStats()
    {
        GameManager.Instance.playerStatus.AddStats(GameManager.Instance.playerStatus.currentStats, stats);
    }
}
