using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class Hitbox : MonoBehaviour
{
    [TabGroup("Settings")] public float baseDamage = 1;
    [HideInInspector] public int totalDamage;
    [HideInInspector] public int hitboxID;
    [HideInInspector] public AttackScript attack;
    [HideInInspector] public Move move;
    [TabGroup("Debug")] public Status status;
    [TabGroup("Debug")] public Alignment alignment = Alignment.Enemy;
    [TabGroup("Settings")] public bool canClash = true;
    [TabGroup("Settings")] public bool relativePushback = false;

    [Header("Multihit")]
    [TabGroup("Settings")] public int resetTimer = 0;
    protected int resetCounter;
    [TabGroup("Debug")] public GameObject col;
    Vector3 knockbackDirection;
    Vector3 pushbackDirection;
    [HideInInspector] protected Transform body;
    [HideInInspector] public List<Status> enemyList;
    [TabGroup("Debug")] [SerializeField] protected MeshRenderer mr;
    protected Transform colPos;

    protected void Awake()
    {
        if (mr == null)
            mr = GetComponent<MeshRenderer>();

        status = GetComponentInParent<Status>();
        if (body == null) body = GetComponentInParent<Status>().transform;
        enemyList = new List<Status>();
    }

    private void Start()
    {
        alignment = status.alignment;
    }

    private void OnEnable()
    {
        if (GameManager.Instance.showHitboxes)
        {
            if (mr != null)
                mr.enabled = true;
        }
        else
        {
            if (mr != null)
                mr.enabled = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Status enemyStatus = other.GetComponentInParent<Status>();
        Hitbox hitbox = other.GetComponent<Hitbox>();
        colPos = other.gameObject.transform;

        if (attack.landCancelFrame) return;

        if (other.TryGetComponent(out Projectile proj))
        {
            if (proj.deflectable && alignment != proj.alignment)
            {
                proj.DeflectProjectile();
                return;
            }
        }

        if (enemyStatus != null)
        {
            if (status == enemyStatus || alignment == enemyStatus.alignment) return;

            if (!enemyList.Contains(enemyStatus))
            {
                if (!CheckInvul(enemyStatus)) return;
                enemyList.Add(enemyStatus);
                DoDamage(enemyStatus, 1);
                return;
            }
        }
    }
    protected void MultiHitProperty()
    {
        if (resetTimer > 0)
        {
            if (enemyList.Count > 0)
            {
                resetCounter--;
                if (resetCounter <= 0)
                {
                    if (col != null)
                        col.gameObject.SetActive(false);

                    enemyList.Clear();
                    resetCounter = resetTimer;
                }
            }
            else
            {
                if (col != null)
                    col.gameObject.SetActive(true);
            }
        }
    }
    public void SetupHitbox(int i, AttackScript a, Status s, Move m)
    {
        hitboxID = i;
        attack = a;
        status = s;
        move = m;

        if (s.TryGetComponent(out SkillHandler skillHandler))
        {
            skillHandler.OnHitboxSpawnBehaviour(this);
        }
    }
    public void ResetHitbox()
    {
        enemyList.Clear();
    }

    public bool CheckInvul(Status enemyStatus)
    {
        if (enemyStatus.invincible)
        {
            return false;
        }
        if (enemyStatus.groundState != GroundState.Grounded && move.hitsGroundOnly)
            return false;

        return true;
    }

    public virtual void DoDamage(Status other, float dmgMod)
    {
        CheckAttack(other, move.attacks[hitboxID]);
    }

    public virtual void CheckAttack(Status other, Attack tempAttack)
    {
        if (relativePushback)
        {
            knockbackDirection = (other.transform.position - transform.position).normalized;
        }
        else
        {
            knockbackDirection = body.forward;
        }
        knockbackDirection.y = 0;
        knockbackDirection = knockbackDirection.normalized;


        if (other.groundState == GroundState.Grounded)
        {
            ExecuteHit(tempAttack.groundHitProperty, other, tempAttack);
        }
        else if (other.groundState == GroundState.Airborne && tempAttack.attackType == AttackType.Ground)
        {
            //Miss on airborne
        }
        //Check for airborne or knockdown state
        else if (other.groundState == GroundState.Airborne || other.groundState == GroundState.Knockdown)
        {
            if (tempAttack.airHitProperty.pushback.x == 0 && tempAttack.airHitProperty.pushback.y == 0)
                ExecuteHit(tempAttack.groundHitProperty, other, tempAttack);
            else
                ExecuteHit(tempAttack.airHitProperty, other, tempAttack);
        }
    }

    void ExecuteHit(HitProperty hit, Status other, Attack atk)
    {
        attack.hit = true;


        pushbackDirection = status.currentStats.knockbackModifier * (knockbackDirection * hit.pushback.x + Vector3.up * hit.pushback.y);

        //Screen shake on hit
        for (int i = 0; i < move.screenShake.Length; i++)
        {
            if (move.screenShake[i].type == ScreenShakeType.OnHit)
                CameraManager.Instance.ShakeCamera(move.screenShake[i].amplitude, move.screenShake[i].duration);
        }

        int rng = Random.Range(1, 101);
        bool crit = false;
        bool physicalDamage = atk.damageType == DamageType.Physical;



        if (rng <= status.currentStats.critChance * 100)
        {
            crit = true;
            status.Meter += (int)(move.meterGain * status.currentStats.meterGainModifier * status.currentStats.critMultiplier);
            switch (atk.damageType)
            {
                case DamageType.Physical:
                    totalDamage = (int)(baseDamage * (atk.damage * status.currentStats.damageModifierPercentage * status.Attack * status.currentStats.critMultiplier) + status.currentStats.damageModifierFlat);
                    break;
                case DamageType.Magic:
                    totalDamage = (int)(baseDamage * (atk.damage * status.currentStats.damageModifierPercentage * status.Magic * status.currentStats.critMultiplier) + status.currentStats.damageModifierFlat);
                    break;
                case DamageType.Blessing:
                    totalDamage = (int)(baseDamage * (atk.damage * status.currentStats.damageModifierPercentage * status.Magic * status.currentStats.critMultiplier) + status.currentStats.damageModifierFlat);
                    break;
                default:
                    break;
            }
        }
        else
        {
            status.Meter += (int)(move.meterGain * status.currentStats.meterGainModifier);

            switch (atk.damageType)
            {
                case DamageType.Physical:
                    totalDamage = (int)(baseDamage * (atk.damage * status.currentStats.damageModifierPercentage * status.Attack) + status.currentStats.damageModifierFlat);
                    break;
                case DamageType.Magic:
                    totalDamage = (int)(baseDamage * (atk.damage * status.currentStats.damageModifierPercentage * status.Magic) + status.currentStats.damageModifierFlat);
                    break;
                case DamageType.Blessing:
                    totalDamage = (int)(baseDamage * (atk.damage * status.currentStats.damageModifierPercentage * status.currentStats.faith) + status.currentStats.damageModifierFlat);
                    break;
                default:
                    break;
            }
        }

        //Send info to skill manager
        bool backstab = false;
        //BACKSTAB
        if (Mathf.Sign((other.transform.position - status.transform.position).x) == Mathf.Sign(other.transform.localRotation.y))
        {
            backstab = true;
            totalDamage = (int)(totalDamage * (1 + status.currentStats.backstabModifier));
        }
        int damageDealt = Mathf.RoundToInt((totalDamage * (1 - other.currentStats.defense)) - other.currentStats.resistance);
        if (damageDealt < 0) damageDealt = 0;

        if (status.currentStats.lifesteal > 0 && physicalDamage)
        {
            int clampedDamage = Mathf.Clamp(damageDealt, 0, other.currentStats.currentHealth);
            status.Health += (int)(clampedDamage * status.currentStats.lifesteal);
        }


        HitInfo hitInfo = new HitInfo(crit, backstab, status, other, move);
        attack.HitEvent(hitInfo);

        foreach (var item in move.skillProperties)
        {
            item.HitBehaviour(hitInfo);
        }


        if (damageDealt <= 0)
        {
            if (atk.damage != 0)
            {
                if (move.blockFX != null)
                    Instantiate(move.blockFX, colPos.position, colPos.rotation);
                else
                    Instantiate(VFXManager.Instance.defaultBlockVFX, colPos.position, colPos.rotation);

                if (move.hitSFX.audioClips.Count > 0)
                    AudioManager.Instance.PlaySFX(move.blockSFX, colPos.position);
                else
                    AudioManager.Instance.PlaySFX(VFXManager.Instance.defaultBlockSFX, colPos.position);
            }
        }
        else
        {
            //Hit FX
            if (move.hitFX.prefab != null)
            {
                GameObject VFX = Instantiate(move.hitFX.prefab, colPos.position, colPos.rotation);
                VFX.transform.localScale = move.hitFX.scale;
            }
            else
                Instantiate(VFXManager.Instance.defaultHitVFX, colPos.position, colPos.rotation);

            if (move.hitSFX.audioClips.Count > 0)
                AudioManager.Instance.PlaySFX(move.hitSFX, colPos.position);
            else
                AudioManager.Instance.PlaySFX(VFXManager.Instance.defaultHitSFX, colPos.position);

        }

        switch (atk.attackLevel)
        {
            case AttackLevel.Level1:
                other.TakeHit(damageDealt, pushbackDirection, CombatManager.Instance.lvl1.stun, CombatManager.Instance.lvl1.poiseBreak, pushbackDirection, CombatManager.Instance.lvl1.hitstop, hit.hitState, crit);
                break;
            case AttackLevel.Level2:
                other.TakeHit(damageDealt, pushbackDirection, CombatManager.Instance.lvl2.stun, CombatManager.Instance.lvl2.poiseBreak, pushbackDirection, CombatManager.Instance.lvl2.hitstop, hit.hitState, crit);
                break;
            case AttackLevel.Level3:
                other.TakeHit(damageDealt, pushbackDirection, CombatManager.Instance.lvl3.stun, CombatManager.Instance.lvl3.poiseBreak, pushbackDirection, CombatManager.Instance.lvl3.hitstop, hit.hitState, crit);
                break;
            case AttackLevel.Custom:
                other.TakeHit(damageDealt, pushbackDirection, atk.stun, atk.poiseBreak, pushbackDirection, atk.hitstop, hit.hitState, crit);
                break;
        }
    }
}
