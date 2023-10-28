using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "Move")]
public class Move : ScriptableObject
{
    public int animationID;
    public int meterCost;
    public int meterGain = 10;
    public bool consumeMeterOnActiveFrame;

    public Sprite icon;
    [Header("Read Only")]
    public int firstStartupFrame;
    public int lastActiveFrame;
    public int totalMoveDuration;
    public int firstGatlingFrame;

    [Header("Editable")]
    public int recoveryFrames;

    [TabGroup("Attacks")] public Attack[] attacks;

    [Header("Hit properties")]
    [TabGroup("Attacks")] public MoveType type;

    public MoveUniqueProperty[] uniqueProperties;

    [Header("Screen shake")]
    [TabGroup("Group 2", "FX")]
    public ScreenShake[] screenShake;
    [TabGroup("Group 2", "FX")] public VFX[] vfx;
    [TabGroup("Group 2", "FX")] public SFX[] sfx;
    [TabGroup("Group 2", "FX")] public VFX hitFX;
    [TabGroup("Group 2", "FX")] public GameObject blockFX;
    [TabGroup("Group 2", "FX")] public GameObject counterhitFX;
    [TabGroup("Group 2", "FX")] public GameObject hitSFX;
    [TabGroup("Group 2", "FX")] public GameObject blockSFX;
    [TabGroup("Group 2", "FX")] public GameObject counterhitSFX;

    [Header("Move properties")]
    [TabGroup("Move properties")] public bool useAttackSpeed = true;
    [TabGroup("Move properties")] public bool keepVelocity = false;
    [TabGroup("Move properties")] public int particleID;
    [TabGroup("Move properties")] public bool holdAttack;
    [TabGroup("Move properties")] public Move releaseAttackMove;

    [TabGroup("Move properties")] public bool autoAim;
    [TabGroup("Move properties")] public bool armor;
    [TabGroup("Move properties")] public bool resetGatling;
    [TabGroup("Move properties")] public bool hitsGroundOnly;

    [TabGroup("Move properties")] public bool noClip;
    [ShowIf("noClip")]
    [TabGroup("Move properties")] public int noClipStart = 1;
    [ShowIf("noClip")]
    [TabGroup("Move properties")] public int noClipDuration;
    [TabGroup("Move properties")] public bool invincible;
    [ShowIf("invincible")]
    [TabGroup("Move properties")] public int invincibleStart = 1;
    [ShowIf("invincible")]
    [TabGroup("Move properties")] public int invincibleDuration;
    [TabGroup("Move properties")] public bool projectileInvul;
    [ShowIf("projectileInvul")]
    [TabGroup("Move properties")] public int projectileInvulStart = 1;
    [ShowIf("projectileInvul")]
    [TabGroup("Move properties")] public int projectileInvulDuration;

    [TabGroup("Move properties")] public bool airInvul;
    [ShowIf("airInvul")]
    [TabGroup("Move properties")] public int airInvulStart = 1;
    [ShowIf("airInvul")]
    [TabGroup("Move properties")] public int airInvulDuration;

    [TabGroup("Cancel properties")] public List<Move> targetComboMoves;
    [TabGroup("Cancel properties")] public bool fullCancel = false;
    [TabGroup("Cancel properties")] public bool gatlingCancel = false;
    [TabGroup("Cancel properties")] public bool uncancelable = false;

    [TabGroup("Air properties")] public bool useAirAction;
    [TabGroup("Air properties")] public bool landCancel;
    [TabGroup("Air properties")] public bool recoverOnlyOnLand;

    [Header("Momentum")]
    [TabGroup("Momentum")] public Momentum[] m;
    [TabGroup("Momentum")] public bool instantStartupRotation = true;
    [TabGroup("Momentum")] public bool overrideVelocity = true;
    [TabGroup("Momentum")] public bool runMomentum = true;
    [TabGroup("Momentum")] public bool inheritForwardVelocity = false;
    [TabGroup("Momentum")] public bool stopAtEdges = true;
    [TabGroup("Momentum")] public bool homing = false;

    private void OnValidate()
    {
        if (attacks != null)
        {
            if (attacks.Length > 0)
            {
                firstStartupFrame = attacks[0].startupFrame;
                firstGatlingFrame = attacks[0].startupFrame + attacks[0].gatlingFrames;
                lastActiveFrame = attacks[attacks.Length - 1].startupFrame + attacks[attacks.Length - 1].activeFrames - 1;
            }
            else lastActiveFrame = firstStartupFrame;
        }

        totalMoveDuration = lastActiveFrame + recoveryFrames;
    }
}

public enum DamageType { Slash, Blunt, Thrust, Chop, Fire, Water, Earth, Wind }


[System.Serializable]
public class SFX
{

    public int startup = 1;
    public GameObject prefab;
}

[System.Serializable]
public class ScreenShake
{
    public ScreenShakeType type;
    [HideIf("@type != ScreenShakeType.OnStartup")] public int startup = 1;
    public float amplitude = 2;
    public int duration = 10;

}

[System.Serializable]
public class VFX
{
    public int startup = 1;
    public GameObject prefab;
    public bool deattachFromPlayer = true;
    public bool destroyOnRecovery = false;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;
}


[System.Serializable]
public class Attack
{
    public AttackLevel attackLevel = AttackLevel.Level1;
    public AttackType attackType = AttackType.Normal;
    public GameObject hitbox;
    public float damage = 1;
    [ShowIf("@attackLevel == AttackLevel.Custom")]
    public int stun = 20;
    [ShowIf("@attackLevel == AttackLevel.Custom")]
    public int hitstop = 5;
    [ShowIf("@attackLevel == AttackLevel.Custom")]
    public int poiseBreak;

    public int startupFrame = 10;
    public int activeFrames = 5;
    public int gatlingFrames = 5;
    public HitProperty groundHitProperty;
    public HitProperty airHitProperty;
    public HitProperty blockProperty;
    public int hitID = 0;
}

[System.Serializable]
public class Momentum
{
    public int startFrame = 1;
    public int duration;
    public Vector2 momentum;
    public bool resetVelocityDuringRecovery = true;
    [ShowIf("useCurve")] public AnimationCurve xMovementCurve;
    [ShowIf("useCurve")] public AnimationCurve movementCurve;
   public bool useCurve = false;
    public bool teleport = false;

}

[System.Serializable]
public class HitProperty
{
    public Vector2 pushback;
    public HitState hitState;

}

public enum ScreenShakeType { OnStartup, OnHit, OnLand }

public enum AttackType { Normal, Projectile, Throw, Ground }
public enum AttackLevel { Level1, Level2, Level3, Custom }
public enum HitState { None, Knockdown, Launch };
public enum MoveType { Normal, Special, UniversalMechanics, Movement, EX, Super }