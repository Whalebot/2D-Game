using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float baseDamage = 1;
    public int totalDamage;
    [HideInInspector] public int hitboxID;
    [HideInInspector] public AttackScript attack;
    [HideInInspector] public Move move;
    [HideInInspector] public Status status;
    [SerializeField] protected bool canClash = true;
    Vector3 knockbackDirection;
    Vector3 aVector;
    public Transform body;
    [SerializeField] public List<Status> enemyList;
    MeshRenderer mr;
    protected Transform colPos;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();

        status = GetComponentInParent<Status>();
        if (body == null) body = GetComponentInParent<Status>().transform;

        enemyList = new List<Status>();

    }
    private void OnEnable()
    {
        if (GameManager.Instance.showHitboxes)
        {
            mr.enabled = true;
        }
        else
        {
            mr.enabled = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Status enemyStatus = other.GetComponentInParent<Status>();
        Hitbox hitbox = other.GetComponent<Hitbox>();
        colPos = other.gameObject.transform;

        if (attack.landCancelFrame) return;

        if (enemyStatus != null)
        {
            if (status == enemyStatus || status.alignment == enemyStatus.alignment) return;

            if (!enemyList.Contains(enemyStatus))
            {
                canClash = false;
                if (!CheckInvul(enemyStatus)) return;
                enemyList.Add(enemyStatus);
                DoDamage(enemyStatus, 1);
                return;
            }
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
        knockbackDirection = body.forward;
        knockbackDirection.y = 0;
        knockbackDirection = knockbackDirection.normalized;

        //Check for block
        if (other.blocking)
        {
            ExecuteBlock(tempAttack.blockProperty, other, tempAttack);
        }
        else
        {
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
                ExecuteHit(tempAttack.airHitProperty, other, tempAttack);
            }
        }
    }


    void ExecuteBlock(HitProperty hit, Status other, Attack atk)
    {
        attack.hit = true;

        //Block FX
        if (move.blockFX != null)
        { Instantiate(move.blockFX, colPos.position, colPos.rotation); }
        else
            Instantiate(VFXManager.Instance.defaultBlockVFX, colPos.position, colPos.rotation);
        if (move.blockSFX != null)
        { Instantiate(move.blockSFX, colPos.position, colPos.rotation); }
        else
            Instantiate(VFXManager.Instance.defaultBlockSFX, colPos.position, colPos.rotation);


        //Calculate direction
        aVector = knockbackDirection * hit.pushback.x + Vector3.up * hit.pushback.y;

        totalDamage = (int)(baseDamage *
        (atk.damage * status.currentStats.damageModifierPercentage) + status.currentStats.damageModifierFlat);

        int damageDealt = Mathf.RoundToInt(totalDamage - other.currentStats.resistance);

        switch (atk.attackLevel)
        {
            case AttackLevel.Level1:
                other.TakeHit(damageDealt, aVector, CombatManager.Instance.lvl1.stun, CombatManager.Instance.lvl1.poiseBreak, aVector, CombatManager.Instance.lvl1.hitstop, hit.hitState);
                break;
            case AttackLevel.Level2:
                other.TakeHit(damageDealt, aVector, CombatManager.Instance.lvl2.stun, CombatManager.Instance.lvl2.poiseBreak, aVector, CombatManager.Instance.lvl2.hitstop, hit.hitState);
                break;
            case AttackLevel.Level3:
                other.TakeHit(damageDealt, aVector, CombatManager.Instance.lvl3.stun, CombatManager.Instance.lvl3.poiseBreak, aVector, CombatManager.Instance.lvl3.hitstop, hit.hitState);
                break;
            case AttackLevel.Custom:
                other.TakeHit(damageDealt, aVector, atk.stun, atk.poiseBreak, aVector, atk.hitstop, hit.hitState);
                break;
        }
    }



    void ExecuteHit(HitProperty hit, Status other, Attack atk)
    {
        attack.hit = true;

        attack.attackHitEvent?.Invoke(move);


        //Calculate direction
        aVector = knockbackDirection * hit.pushback.x + Vector3.up * hit.pushback.y;

        //Screen shake on hit
        for (int i = 0; i < move.screenShake.Length; i++)
        {
            if (move.screenShake[i].type == ScreenShakeType.OnHit)
                CameraManager.Instance.ShakeCamera(move.screenShake[i].amplitude, move.screenShake[i].duration);
        }

        int rng = Random.Range(1, 101);
        bool crit = false;



        if (rng <= status.currentStats.critChance * 100)
        {
            crit = true;
            status.Meter += (int)(move.meterGain * status.currentStats.meterGainModifier * status.currentStats.critMultiplier);
            totalDamage = (int)(baseDamage * (atk.damage * status.currentStats.damageModifierPercentage * status.Attack * status.currentStats.critMultiplier) + status.currentStats.damageModifierFlat);
        }
        else
        {
            status.Meter += (int)(move.meterGain * status.currentStats.meterGainModifier);
            totalDamage = (int)(baseDamage * (atk.damage * status.currentStats.damageModifierPercentage * status.Attack) + status.currentStats.damageModifierFlat);

        }
        //BACKSTAB
        if (Mathf.Sign(status.transform.localScale.x) == Mathf.Sign(other.transform.localScale.x))
        {
            totalDamage = (int)(totalDamage * (1 + status.currentStats.backstabModifier));
        }
        int damageDealt = Mathf.RoundToInt((totalDamage * (1 - other.currentStats.defense)) - other.currentStats.resistance);


        if (damageDealt <= 0)
        {
            if (move.blockFX != null)
                Instantiate(move.blockFX, colPos.position, colPos.rotation);
            else Instantiate(VFXManager.Instance.defaultBlockVFX, colPos.position, colPos.rotation);
            if (move.blockSFX != null)
                Instantiate(move.blockSFX, colPos.position, colPos.rotation);
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

            if (move.hitSFX != null)
                Instantiate(move.hitSFX, colPos.position, colPos.rotation);
            else
                Instantiate(VFXManager.Instance.defaultHitSFX, colPos.position, colPos.rotation);

        }

        switch (atk.attackLevel)
        {
            case AttackLevel.Level1:
                other.TakeHit(damageDealt, aVector, CombatManager.Instance.lvl1.stun, CombatManager.Instance.lvl1.poiseBreak, aVector, CombatManager.Instance.lvl1.hitstop, hit.hitState, crit);
                break;
            case AttackLevel.Level2:
                other.TakeHit(damageDealt, aVector, CombatManager.Instance.lvl2.stun, CombatManager.Instance.lvl2.poiseBreak, aVector, CombatManager.Instance.lvl2.hitstop, hit.hitState, crit);
                break;
            case AttackLevel.Level3:
                other.TakeHit(damageDealt, aVector, CombatManager.Instance.lvl3.stun, CombatManager.Instance.lvl3.poiseBreak, aVector, CombatManager.Instance.lvl3.hitstop, hit.hitState, crit);
                break;
            case AttackLevel.Custom:
                other.TakeHit(damageDealt, aVector, atk.stun, atk.poiseBreak, aVector, atk.hitstop, hit.hitState, crit);
                break;
        }
    }

    void Clash(Status enemyStatus)
    {
        canClash = false;
        Collider col = GetComponent<Collider>();
        col.enabled = false;

        //Hit FX
        if (move.hitFX != null)
        {
            GameObject VFX = Instantiate(move.hitFX.prefab, colPos.position, colPos.rotation);
            VFX.transform.localScale = move.hitFX.scale;
        }
        else
            Instantiate(VFXManager.Instance.defaultHitVFX, colPos.position, colPos.rotation);

        if (move.hitSFX != null)
            Instantiate(move.hitSFX, colPos.position, colPos.rotation);
        else
            Instantiate(VFXManager.Instance.defaultHitSFX, colPos.position, colPos.rotation);

        attack.newAttack = false;
        attack.Idle();
    }
}
