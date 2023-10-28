using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillProperty", menuName = "ScriptableObjects/SkillProperty", order = 1)]
public class UniqueSkillProperty : ScriptableObject
{

    public virtual void ActivateBehaviour(SkillHandler handler)
    {
    }
    public virtual void LateBehaviour(SkillHandler handler)
    {
    }
    public virtual void RepeatBehaviour(SkillHandler handler)
    {
    }
    public virtual void AttackActiveBehaviour(SkillHandler handler)
    {
    }
    public virtual void WaveBehaviour(SkillHandler handler)
    {
    }
    public virtual void HitBehaviour(HitInfo hitInfo)
    {
    }

}
