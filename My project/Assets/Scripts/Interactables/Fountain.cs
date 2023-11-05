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
        GameManager.Instance.playerStatus.Health += stats.currentHealth;
        GameManager.Instance.playerStatus.Meter += stats.currentMeter;
    }
}
