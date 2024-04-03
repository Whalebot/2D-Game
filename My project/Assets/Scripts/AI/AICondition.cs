using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICondition : ScriptableObject
{
    public virtual bool IsTrue()
    {
        return true;
    }
}
