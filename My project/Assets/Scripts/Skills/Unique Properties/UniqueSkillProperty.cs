using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SkillProperty", menuName = "ScriptableObjects/SkillProperty", order = 1)]
public class UniqueSkillProperty : ScriptableObject
{
    public UniquePropertyType propertyType;
    [HideIf("@propertyType != UniquePropertyType.StartupFrame")] public int frame;
    [HideIf("@propertyType != UniquePropertyType.Timer")] public int delay;
    [HideIf("@propertyType != UniquePropertyType.Timer")] int counter;
    public virtual int GetDamage(Status status = null, Rank rank = Rank.D)
    {
        return -1;
    }
    public virtual float GetModifier(Status status = null, Rank rank = Rank.D)
    {
        return (RarityModifier(rank));
    }
    public virtual float RarityModifier(Rank r)
    {
        float rarityModifier = (1 + (int)r * 0.25f);
        return rarityModifier;
    }
    public virtual void OnFrameBehaviour(SkillHandler handler, SkillSO skill = null)
    {
        if (propertyType != UniquePropertyType.Timer) return;
        counter--;
        if (counter <= 0)
        {
            counter = delay;
            OnTimer(handler);
            // Debug.Log(counter + " " + delay);
        }

    }

    public virtual void OnTimer(SkillHandler handler)
    {

    }

    public virtual void ActivateBehaviour(SkillHandler handler)
    {
    }
    public virtual void LateBehaviour(SkillHandler handler)
    {
    }

    public virtual void OnStartupFrame(AttackScript atk, int frame, SkillSO skill = null)
    {
        if (propertyType != UniquePropertyType.StartupFrame) return;
    }

    public virtual void OnActiveFrames(AttackScript atk)
    {
        if (propertyType != UniquePropertyType.ActiveFrames) return;
    }

    public virtual void AttackingBehaviour(SkillHandler handler, Rank rank)
    {
    }
    public virtual void OnHitboxSpawnBehaviour(SkillHandler handler, Rank rank, Hitbox hitbox)
    {
        if (propertyType != UniquePropertyType.StartupFrame) return;
    }
    public virtual void ActiveFrameBehaviour(SkillHandler handler)
    {
    }
    public virtual void OnRecoveryBehaviour()
    {

    }
    public virtual void WaveBehaviour(SkillHandler handler, SkillSO skill)
    {
    }
    public virtual void HitBehaviour(HitInfo hitInfo, SkillSO skill = null)
    {
    }
    public virtual void DeathBehaviour(SkillSO skill = null)
    {
    }
    public enum UniquePropertyType
    {
        StartupFrame, ActiveFrames, OnHit, OnDeath, Timer, OnHitboxSpawn
    }
}
