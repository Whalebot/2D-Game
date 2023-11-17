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
    public virtual void AttackingBehaviour(SkillHandler handler)
    {
    }
    public virtual void ActiveFrameBehaviour(SkillHandler handler)
    {
    }
    public virtual void WaveBehaviour(SkillHandler handler)
    {
    }
    public virtual void HitBehaviour(HitInfo hitInfo)
    {
    }

}
