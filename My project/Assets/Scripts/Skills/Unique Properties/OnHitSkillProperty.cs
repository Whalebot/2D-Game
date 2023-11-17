using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitSkillProperty", menuName = "ScriptableObjects/SkillProperty/OnHitSkillProperty")]
public class OnHitSkillProperty : UniqueSkillProperty
{
    public MoveGroup affectedMoves;
    public ChainLightning chainLightning;
    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);

        if (hitInfo.status == null)
            return;

        if (!affectedMoves.moves.Contains(hitInfo.move))
            return;

        ChainLightning lightning = Instantiate(chainLightning, hitInfo.status.transform.position, Quaternion.identity);
        lightning.StartChainLightning(hitInfo.status);
    }
}
