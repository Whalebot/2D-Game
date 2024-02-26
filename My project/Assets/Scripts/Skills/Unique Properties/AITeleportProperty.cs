using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI Teleport", menuName = "ScriptableObjects/SkillProperty/AI Teleport")]
public class AITeleportProperty : UniqueSkillProperty
{
    public enum TeleportLocation { Side, Top }
    public TeleportLocation teleportLocation;
    SuccubusBoss succubus;
    public override void OnStartupFrame(AttackScript atk, int frame, SkillSO skill = null)
    {
        if (propertyType != UniquePropertyType.StartupFrame) return;

        base.OnStartupFrame(atk, frame);
        succubus = atk.GetComponent<SuccubusBoss>();

        switch (teleportLocation)
        {
            case TeleportLocation.Side:
                succubus.TeleportSide();
                break;
            case TeleportLocation.Top:
                succubus.TeleportTop();
                break;
            default:
                break;
        }
    }
}
