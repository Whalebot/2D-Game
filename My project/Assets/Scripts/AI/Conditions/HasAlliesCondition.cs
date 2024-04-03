using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Has Allies Condition", menuName = "ScriptableObjects/AICondition/HasAlliesCondition")]
public class HasAlliesCondition : AICondition
{
    public bool hasAllies;
    public float range;
    public override bool IsTrue()
    {
        if (range <= 0)
        {
            return hasAllies ? AIManager.Instance.enemyCount > 1 : AIManager.Instance.enemyCount <= 1;
        }
        else
        {

            return true;
        }
    }
}
