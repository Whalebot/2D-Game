using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Movement Property", menuName = "ScriptableObjects/Movement Property")]
public class MovementSkillProperty : UniqueSkillProperty
{
    public Vector3 pogoMovement;
    public override void HitBehaviour(HitInfo hitInfo, SkillSO skill)
    {
        base.HitBehaviour(hitInfo, skill);

        if (hitInfo.enemyStatus == null)
            return;

        Movement mov = hitInfo.attackerStatus.GetComponent<Movement>();
        Vector3 v = mov.vel;
        if (pogoMovement.x != 0) { v.x = pogoMovement.x; }
        if (pogoMovement.y != 0) { v.y = pogoMovement.y; }
        if (pogoMovement.z != 0) { v.z = pogoMovement.z; }
        mov.SetVelocity(v);
    }
}
