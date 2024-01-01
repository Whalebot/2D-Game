using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BurningStatusEffect", menuName = "ScriptableObjects/StatusEffects/BurningStatusEffect")]
public class BurningStatusEffect : StatusEffect
{
    public VFX fireVFX;
    public bool spread;
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

        if (spread)
        {
            Collider[] col = Physics.OverlapSphere(status.transform.position, 1 * 0.5F);

            foreach (Collider item in col)
            {
                Status tempStatus = item.GetComponentInParent<Status>();
                if (tempStatus == null || tempStatus == status) continue;
                if (tempStatus.alignment != status.alignment) continue;

                tempStatus.ApplyStatusEffect(this);
            }
        }

        if (fireVFX.deattachFromPlayer)
            fx.transform.SetParent(null);
    }

    public override void EndBehaviour()
    {
        base.EndBehaviour();
    }
}
