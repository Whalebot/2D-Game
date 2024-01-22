using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonProperty", menuName = "ScriptableObjects/SkillProperty/SummonProperty")]
public class SummonSkillProperty : UniqueSkillProperty
{
    public GameObject summon;
    public Vector3 offset;

    public override void OnStartupFrame(AttackScript atk, int frame, SkillSO skill = null)
    {
        if (propertyType != UniquePropertyType.StartupFrame) return;
        base.OnStartupFrame(atk, frame);
        Summon(atk.transform.position + atk.transform.forward * offset.z + atk.transform.right * offset.x + atk.transform.up * offset.y, atk.transform.rotation);
    }

    public override void OnTimer(SkillHandler handler)
    {
        Summon(handler.transform.position + handler.transform.forward * offset.z + handler.transform.right * offset.x + handler.transform.up * offset.y, handler.transform.rotation);
    }

    void Summon(Vector3 position, Quaternion rotation)
    {
        Instantiate(summon, position, rotation);
    }
}
