using Sirenix.OdinInspector;
using UnityEngine;

public class MoveUniqueProperty : ScriptableObject
{
    public UniquePropertyType propertyType;


    [HideIf("@propertyType != UniquePropertyType.StartupFrame")] public int frame;

    public virtual void OnStartupFrame(AttackScript atk, int frame)
    {
        if (propertyType != UniquePropertyType.StartupFrame) return;
    }

    public virtual void OnActiveFrames(AttackScript atk)
    {
        if (propertyType != UniquePropertyType.ActiveFrames) return;
    }

    public virtual void HitBehaviour(HitInfo hitInfo)
    {
    }
}

public enum UniquePropertyType
{
    StartupFrame, ActiveFrames, OnHit
}