using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonStatusEffect", menuName = "ScriptableObjects/StatusEffects/PoisonStatusEffect")]
public class PoisonStatusEffect : StatusEffect
{
    public VFX poisonVFX;
    public bool spread;
    public override void TickBehaviour()
    {
        base.TickBehaviour();
        PoisonDamage();
    }

    void PoisonDamage()
    {
        GameObject fx = Instantiate(poisonVFX.prefab, status.transform.position, status.transform.rotation, status.transform);

        int dmgDone = (int)(baseDamage * stacks * damageModifier);

        status.Health -= dmgDone;
        GameManager.Instance.DamageNumbers(status.transform, dmgDone, false,status.alignment);
        fx.transform.localPosition = poisonVFX.position;
        fx.transform.localRotation = Quaternion.Euler(poisonVFX.rotation);
        fx.transform.localScale = poisonVFX.scale;

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


        if (poisonVFX.deattachFromPlayer)
            fx.transform.SetParent(null);
    }

    public override void EndBehaviour()
    {
        base.EndBehaviour();
    }
}
