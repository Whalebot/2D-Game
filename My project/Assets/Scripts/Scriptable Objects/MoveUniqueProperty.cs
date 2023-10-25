using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveUniqueProperty", menuName = "MoveUniqueProperty")]
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
}

public enum UniquePropertyType
{
    StartupFrame, ActiveFrames
}