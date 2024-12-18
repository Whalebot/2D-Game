using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PushbackStatusEffect", menuName = "ScriptableObjects/StatusEffects/PushbackStatusEffect")]
public class PushbackStatusEffect : StatusEffect
{
    public GameObject hazardPrefab;
    public GameObject tempGO;
    public Hazard hazard;
    public override void ActivateBehaviour(Status s, HitInfo hitInfo = null,Rank rank = Rank.D)
    {
        base.ActivateBehaviour(s, hitInfo, rank);
        SpawnHitbox();
    }

    public override void RefreshBehaviour(Status s, HitInfo hitInfo = null, Rank rank = Rank.D)
    {
        base.RefreshBehaviour(s, hitInfo, rank);
        DestroyHitbox();
        SpawnHitbox();

    }

    void SpawnHitbox()
    {
        tempGO = Instantiate(hazardPrefab, status.transform.position, status.transform.rotation, status.transform);
        hazard = tempGO.GetComponent<Hazard>();
        hazard.enemyList.Add(status);
        hazard.baseDamage = baseDamage * damageModifier;
        //hazard.

        tempGO.transform.localPosition = hazardPrefab.transform.position;
        tempGO.transform.localRotation = hazardPrefab.transform.localRotation;
        tempGO.transform.localScale = hazardPrefab.transform.localScale;

    }
    void DestroyHitbox()
    {
        if (tempGO != null)
            Destroy(tempGO);
    }

    public override void EndBehaviour()
    {
        DestroyHitbox();
        base.EndBehaviour();

    }
}
