using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHitbox : Hitbox
{
    public Move parryFollowup;
    public VFX parryVFX;

    new void OnTriggerEnter(Collider other)
    {
        bool foundTarget = false;
        colPos = other.gameObject.transform;
        Projectile proj = other.GetComponentInParent<Projectile>();
        Hitbox hitbox = other.GetComponent<Hitbox>();

        if (proj != null || hitbox != null)
        {
            foundTarget = true;
        }

        if (proj != null && proj.status != status)
        {
            ProjectileParry();
            proj.DestroyProjectile();
            return;
        }


        if (hitbox != null && hitbox.status != status)
        {
            Parry();
            hitbox.gameObject.SetActive(false) ;
            return;
        }
    }
    void Parry()
    {
        status.Meter += move.meterGain;

        GameObject VFX = Instantiate(parryVFX.prefab, colPos.position, colPos.rotation);
        VFX.transform.localScale = parryVFX.scale;

        if (parryFollowup != null)
            attack.AttackProperties(parryFollowup);
    }

    void ProjectileParry()
    {
        status.Meter += move.meterGain;

        GameObject VFX = Instantiate(parryVFX.prefab, colPos.position, colPos.rotation);
        VFX.transform.localScale = parryVFX.scale;
    }
}
