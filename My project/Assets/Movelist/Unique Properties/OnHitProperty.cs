using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "On Hit Property", menuName = "MoveUniqueProperty/On Hit Property")]
public class OnHitProperty : MoveUniqueProperty
{
    public ChainLightning chainLightning;
    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);

        if (hitInfo.status == null)
            return;

        ChainLightning lightning = Instantiate(chainLightning, hitInfo.status.transform.position, Quaternion.identity);
        lightning.StartChainLightning(hitInfo.status);
    }
}
