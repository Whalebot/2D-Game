using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickpocketProperty", menuName = "ScriptableObjects/SkillProperty/Pickpocket")]
public class PickpocketSkillProperty : UniqueSkillProperty
{
    public override void HitBehaviour(HitInfo hitInfo)
    {
        base.HitBehaviour(hitInfo);

        if (hitInfo.status.character == null)
            return;

        if (hitInfo.crit && hitInfo.backstab)
        {
            GameManager.Instance.Gold += hitInfo.status.character.stats.gold;
        }
    }
}
