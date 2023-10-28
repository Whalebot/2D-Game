using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitSkillProperty", menuName = "ScriptableObjects/SkillProperty/OnHitSkillProperty")]
public class OnHitSkillProperty : UniqueSkillProperty
{
    public int damage;
    public VFX hitVFX;
    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);

        if (hitInfo.status == null)
            return;

        GameObject fx = Instantiate(hitVFX.prefab, hitInfo.status.transform.position, hitInfo.status.transform.rotation);
        hitInfo.status.Health -= damage;
        GameManager.Instance.DamageNumbers(hitInfo.status.transform, damage, false);
        //fx.transform.localPosition = item.position;
        //fx.transform.localRotation = Quaternion.Euler(item.rotation);
        fx.transform.localScale = hitVFX.scale;
        //if (item.destroyOnRecovery)
        //    fx.GetComponent<VFXScript>().SetupVFX(status);
        if (hitVFX.deattachFromPlayer)
            fx.transform.SetParent(null);
    }
}
