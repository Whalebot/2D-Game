using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "On Hit Property", menuName = "MoveUniqueProperty/On Hit Property")]
public class OnHitProperty : MoveUniqueProperty
{
    public int baseDamage;
    public float damageModifier;
    public ChainLightning chainLightning;
    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);

        if (hitInfo.enemyStatus == null)
            return;

        ChainLightning lightning = Instantiate(chainLightning, hitInfo.enemyStatus.transform.position, Quaternion.identity);
        lightning.damage = (int)(baseDamage * damageModifier * hitInfo.attackerStatus.currentStats.faithModifier);
        lightning.StartChainLightning(hitInfo.enemyStatus);
    }
}
