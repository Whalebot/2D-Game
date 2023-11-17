using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffects", menuName = "ScriptableObjects/StatusEffects")]
public class StatusEffect : ScriptableObject
{
    public int duration;
    public int stacks = 0;
    public int maxStacks = 1;
    int durationCounter;
    public int tickInterval;
    int tickCounter;
    protected Status status;
    public VFX statusVFX;
    public GameObject currentVFX;

    public virtual void ActivateBehaviour(Status s)
    {
        status = s;
        stacks = 1;
        durationCounter = duration;
        tickCounter = 0;
        SpawnVFX();
    }
    void SpawnVFX()
    {
        if (statusVFX.prefab != null)
        {
            currentVFX = Instantiate(statusVFX.prefab, status.transform.position, status.transform.rotation, status.transform);
            currentVFX.transform.localScale = statusVFX.scale;
            currentVFX.transform.localPosition = statusVFX.position;
            currentVFX.transform.localRotation = Quaternion.Euler(statusVFX.rotation);
        }
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
        //tickCounter = 0;
        if (stacks < maxStacks)
            stacks++;
        durationCounter = duration;
    }
    public virtual void EndBehaviour()
    {
        stacks = 0;
        status.RemoveStatusEffect(this);
        if (currentVFX != null)
            Destroy(currentVFX);
        //Remove status effect
    }
    public virtual void TickBehaviour()
    {

    }
}
