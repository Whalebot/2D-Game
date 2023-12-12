using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BurningStatusEffect", menuName = "ScriptableObjects/StatusEffects/BurningStatusEffect")]
public class BurningStatusEffect : StatusEffect
{
    public VFX fireVFX;
    public override void TickBehaviour()
    {
        base.TickBehaviour();
        FireDamage();
    }

    void FireDamage()
    {
        GameObject fx = Instantiate(fireVFX.prefab, status.transform.position, status.transform.rotation, status.transform);

        int damage = (int) (baseDamage * damageModifier);

        status.Health -= damage;
        GameManager.Instance.DamageNumbers(status.transform, damage, false);

        fx.transform.localPosition = fireVFX.position;
        fx.transform.localRotation = Quaternion.Euler(fireVFX.rotation);
        fx.transform.localScale = fireVFX.scale;

        if (fireVFX.deattachFromPlayer)
            fx.transform.SetParent(null);
    }

    public override void EndBehaviour()
    {
        base.EndBehaviour();
    }
}
