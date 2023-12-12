using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffects", menuName = "ScriptableObjects/StatusEffects")]
public class StatusEffect : ScriptableObject
{
    public Elemental elemental;
    public Rank skillRank;
    public int baseDamage;
    public float damageModifier = 1;
    float elementalModifier = 1;

    public int duration;
    public int stacks = 0;
    public int maxStacks = 1;
    int durationCounter;
    public int tickInterval;
    int tickCounter;
    protected Status status;
    public VFX statusVFX;
    public GameObject currentVFX;



    public virtual void ActivateBehaviour(Status s, HitInfo hitInfo = null, Rank rank = Rank.D)
    {
        status = s;
        stacks = 1;
        durationCounter = duration;
        tickCounter = 0;
        skillRank = rank;
        if (hitInfo != null)
        {
            Stats stats = hitInfo.attackerStatus.currentStats;
            switch (elemental)
            {
                case Elemental.None:
                    break;
                case Elemental.Earth:
                    damageModifier = stats.faithModifier * stats.earthModifier;
                    break;
                case Elemental.Fire:
                    damageModifier = stats.faithModifier * stats.fireModifier;
                    break;
                case Elemental.Ice:
                    damageModifier = stats.faithModifier * stats.iceModifier;
                    break;
                case Elemental.Lightning:
                    damageModifier = stats.faithModifier * stats.lightningModifier;
                    break;
                case Elemental.Wind:
                    damageModifier = stats.faithModifier * stats.windModifier;
                    break;
                case Elemental.Poison:
                    damageModifier = stats.faithModifier * stats.poisonModifier;
                    break;
                default:
                    break;
            }

            float rarityModifier = (1 + (int)rank * 0.25f);

            damageModifier = damageModifier * rarityModifier;
        }
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
