using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonStatusEffect", menuName = "ScriptableObjects/StatusEffects/PoisonStatusEffect")]
public class PoisonStatusEffect : StatusEffect
{
    public int damagePerTick;
    public VFX poisonVFX;
    public override void TickBehaviour()
    {
        base.TickBehaviour();
        PoisonDamage();
    }

    void PoisonDamage()
    {
        GameObject fx = Instantiate(poisonVFX.prefab, status.transform.position, status.transform.rotation);
        status.Health -= damagePerTick;
        GameManager.Instance.DamageNumbers(status.transform, damagePerTick, false);
        //fx.transform.localPosition = item.position;
        //fx.transform.localRotation = Quaternion.Euler(item.rotation);
        fx.transform.localScale = poisonVFX.scale;
        //if (item.destroyOnRecovery)
        //    fx.GetComponent<VFXScript>().SetupVFX(status);
        if (poisonVFX.deattachFromPlayer)
            fx.transform.SetParent(null);
    }

    public override void EndBehaviour()
    {
        base.EndBehaviour();
    }
}