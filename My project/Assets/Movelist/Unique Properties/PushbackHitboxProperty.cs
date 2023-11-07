using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushbackHitboxProperty : MoveUniqueProperty
{
    public GameObject hitboxPrefab;
    public GameObject tempGO;
    public Hitbox hitbox;
    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);
        SpawnHitbox(hitInfo);
    }

    void SpawnHitbox(HitInfo hitInfo)
    {
        //GameObject tempGO = Instantiate(hitboxPrefab, hitInfo.status.transform.position, hitInfo.status.transform.rotation, hitInfo.status.transform);
        //hitbox = tempGO.GetComponent<Hitbox>();
        //hitbox.enemyList.Add(hitInfo.status);
        //hitbox.SetupHitbox(0, hitInfo.a, hitInfo.a, hitInfo.status);

        //tempGO.transform.localPosition = hitboxPrefab.transform.position;
        //tempGO.transform.localRotation = hitboxPrefab.transform.localRotation;
        //tempGO.transform.localScale = hitboxPrefab.transform.localScale;

    }
    void DestroyHitbox()
    {

    }
}
