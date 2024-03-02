using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FrozenStatusEffect", menuName = "ScriptableObjects/StatusEffects/FrozenStatusEffect")]
public class FrozenStatusEffect : StatusEffect
{
    public VFX iceVFX;
    public float attackSpeedReduction = 0.1f;
    public float movementSpeedReduction = 0.2f;

    public override void ActivateBehaviour(Status s, HitInfo hitInfo = null, Rank rank = Rank.D)
    {
        base.ActivateBehaviour(s, hitInfo, rank);
        IceDamage();
    }

    public override void RefreshBehaviour(Status s, HitInfo hitInfo = null, Rank rank = Rank.D)
    {
        base.RefreshBehaviour(s, hitInfo, rank);
        IceDamage();
    }

    void ReduceStats()
    {
        status.currentStats.attackSpeed -= attackSpeedReduction;
        status.currentStats.movementSpeedModifier -= movementSpeedReduction;
    }

    void IceDamage()
    {
        GameObject fx = Instantiate(iceVFX.prefab, status.transform.position, status.transform.rotation);

        int damage = (int)(baseDamage * damageModifier);
        status.Health -= damage;
        GameManager.Instance.DamageNumbers(status.transform, damage, false,status.alignment);

        //fx.transform.localPosition = item.position;
        //fx.transform.localRotation = Quaternion.Euler(item.rotation);
        fx.transform.localScale = iceVFX.scale;
        //if (item.destroyOnRecovery)
        //    fx.GetComponent<VFXScript>().SetupVFX(status);
        if (iceVFX.deattachFromPlayer)
            fx.transform.SetParent(null);

        if (stacks < maxStacks)
        {
            stacks++;
            ReduceStats();
        }
    }

    void RevertStats()
    {
        for (int i = 0; i < stacks; i++)
        {
            status.currentStats.attackSpeed += attackSpeedReduction;
            status.currentStats.movementSpeedModifier += movementSpeedReduction;
        }

        stacks = 0;
    }

    public override void EndBehaviour()
    {
        base.EndBehaviour();
        RevertStats();
    }
}
