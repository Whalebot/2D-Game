using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffects", menuName = "ScriptableObjects/StatusEffects")]
public class StatusEffect : ScriptableObject
{
    public int duration;
    int durationCounter;
    public int tickInterval;
    int tickCounter;
    protected Status status;

    public virtual void ActivateBehaviour(Status s)
    {
        status = s;
        durationCounter = duration;
        tickCounter =  0;
    }
    public virtual void ExecuteFrame()
    {
        durationCounter--;
        tickCounter--;

        if (tickCounter <= 0)
        {
            tickCounter = tickInterval;
            TickBehaviour();
        }


        if (durationCounter <= 0)
        {
            durationCounter = duration;
            EndBehaviour();
        }
    }
    public virtual void RefreshBehaviour()
    {
        tickCounter = 0;
        durationCounter = duration;
    }
    public virtual void EndBehaviour()
    {
        status.RemoveStatusEffect(this);
        //Remove status effect
    }
    public virtual void TickBehaviour()
    {

    }
}
