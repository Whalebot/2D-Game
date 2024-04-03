using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StatDependencyProperty", menuName = "ScriptableObjects/SkillProperty/StatDependencyProperty")]
public class StatDependencyProperty : UniqueSkillProperty
{
    public float dmgPerGold = 0.001F;
    int savedGold;
    public override void OnFrameBehaviour(SkillHandler handler, SkillSO skill)
    {
        base.OnFrameBehaviour(handler);
        if (GameManager.Instance.Gold != savedGold)
        {
            savedGold = GameManager.Instance.Gold;
            skill.stats.damageModifierPercentage = dmgPerGold * savedGold;
            //Update stats
            handler.UpdateStats(false);
        }
    }
}
