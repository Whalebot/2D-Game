using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicInteractable : Treasure
{
    AIEnemy ai;

    public override void Start()
    {
        ai = GetComponent<AIEnemy>();
        status = GetComponent<Status>();
    }
    public override bool CanSouth()
    {
        return !ai.detected || status.isDead;
    }

    private void OnValidate()
    {
        
    }

    public override void South()
    {
        if (!ai.detected) {
            ai.Detect();
        }
        else
        {
            base.South();
        }
    }
}
