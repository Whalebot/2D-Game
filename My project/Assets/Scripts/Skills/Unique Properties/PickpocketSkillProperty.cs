using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickpocketProperty", menuName = "ScriptableObjects/SkillProperty/Pickpocket")]
public class PickpocketSkillProperty : UniqueSkillProperty
{
    public override void HitBehaviour(HitInfo hitInfo, SkillSO skill)
    {
        base.HitBehaviour(hitInfo, skill);

        if (hitInfo.enemyStatus.character == null)
            return;

        if (hitInfo.crit && hitInfo.backstab)
        {
            GameManager.Instance.Gold += hitInfo.enemyStatus.character.stats.gold;
        }
    }
}
