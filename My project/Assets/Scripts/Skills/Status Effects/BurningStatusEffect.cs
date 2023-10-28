using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BurningStatusEffect", menuName = "ScriptableObjects/StatusEffects/BurningStatusEffect")]
public class BurningStatusEffect : StatusEffect
{
    public int damagePerTick;
    public VFX fireVFX;
    public override void TickBehaviour()
    {
        base.TickBehaviour();
        FireDamage();
    }

    void FireDamage()
    {
        GameObject fx = Instantiate(fireVFX.prefab, status.transform.position, status.transform.rotation);
        status.Health -= damagePerTick;
        GameManager.Instance.DamageNumbers(status.transform, damagePerTick, false);
        //fx.transform.localPosition = item.position;
        //fx.transform.localRotation = Quaternion.Euler(item.rotation);
        fx.transform.localScale = fireVFX.scale;
        //if (item.destroyOnRecovery)
        //    fx.GetComponent<VFXScript>().SetupVFX(status);
        if (fireVFX.deattachFromPlayer)
            fx.transform.SetParent(null);
    }

    public override void EndBehaviour()
    {
        base.EndBehaviour();
    }
}
