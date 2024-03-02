using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Death Mark Effect", menuName = "ScriptableObjects/StatusEffects/Death Mark Effect")]
public class DeathMarkStatusEffect : StatusEffect
{
    public VFX deathMarkVFX;
    public bool frontHit;
    public bool backHit;

    public override void ActivateBehaviour(Status s, HitInfo hitInfo = null, Rank rank = Rank.D)
    {
        base.ActivateBehaviour(s, hitInfo, rank);
        if (hitInfo.backstab)
            BackHit();
        else
            FrontHit();
    }
    public override void SpawnVFX()
    {

    }
    void FrontHit()
    {
        frontHit = true;
        if (statusVFX.prefab != null)
        {
            currentVFX = Instantiate(statusVFX.prefab, status.transform.position + status.transform.forward * 0.5f, status.transform.rotation, status.transform);
            currentVFX.transform.localScale = statusVFX.scale;
            currentVFX.transform.localPosition = statusVFX.position;
            currentVFX.transform.localRotation = Quaternion.Euler(statusVFX.rotation);
        }
        DeathMark();
    }
    void BackHit()
    {
        backHit = true;
        if (statusVFX.prefab != null)
        {
            Quaternion rot = Quaternion.Euler(status.transform.rotation.x, -status.transform.rotation.y, status.transform.rotation.z);
            currentVFX = Instantiate(statusVFX.prefab, status.transform.position - status.transform.forward * 0.5f, rot, status.transform);
            currentVFX.transform.localScale = statusVFX.scale;
            currentVFX.transform.localPosition = statusVFX.position;
            currentVFX.transform.localRotation = Quaternion.Euler(statusVFX.rotation);
        }
        DeathMark();
    }

    public override void RefreshBehaviour(Status s, HitInfo hitInfo = null, Rank rank = Rank.D)
    {
        base.RefreshBehaviour(s, hitInfo, rank);

        if (hitInfo.backstab)
            BackHit();
        else
            FrontHit();

        if (frontHit && backHit)
        {
            TriggerDeathMark();
        }
    }
    void TriggerDeathMark()
    {
        int dmgDone = (int)(baseDamage * stacks * damageModifier);
        status.Health -= dmgDone;
        GameManager.Instance.DamageNumbers(status.transform, dmgDone, false, status.alignment);
        EndBehaviour();
    }
    void DeathMark()
    {
        //GameObject fx = Instantiate(deathMarkVFX.prefab, status.transform.position, status.transform.rotation, status.transform);
        //fx.transform.localPosition = deathMarkVFX.position;
        //fx.transform.localRotation = Quaternion.Euler(deathMarkVFX.rotation);
        //fx.transform.localScale = deathMarkVFX.scale;
    }
}
