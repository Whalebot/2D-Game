using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float baseDamage = 1;
    public int totalDamage;
    public int resetTimer = 30;
    int resetcounter;
    public Alignment alignment;
    public Attack attack;
    public VFX hitVFX;
    public GameObject hitSFX;
    public GameObject collider;

    Vector3 knockbackDirection;
    Vector3 aVector;

    public List<Status> enemyList;
    protected Transform colPos;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        resetcounter = resetTimer;
    }
    void ExecuteFrame()
    {
        if (enemyList.Count > 0)
        {
            resetcounter--;
            if (resetcounter <= 0)
            {
                collider.gameObject.SetActive(false);
                enemyList.Clear();
                resetcounter = resetTimer;
            }

        }
        else
        {
            collider.gameObject.SetActive(true);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Status enemyStatus = other.GetComponentInParent<Status>();
        Hitbox hitbox = other.GetComponent<Hitbox>();
        colPos = other.gameObject.transform;

        if (enemyStatus != null && hitbox == null)
        {
            if (alignment == enemyStatus.alignment) return;

            if (!enemyList.Contains(enemyStatus))
            {
                if (!CheckInvul(enemyStatus)) return;
                enemyList.Add(enemyStatus);
                ExecuteHit(attack.groundHitProperty, enemyStatus, attack);
                return;
            }
        }
    }
    public bool CheckInvul(Status enemyStatus)
    {
        if (enemyStatus.invincible)
        {
            return false;
        }
        return true;
    }
    void ExecuteHit(HitProperty hit, Status other, Attack atk)
    {
        //Calculate direction
        aVector = transform.forward * hit.pushback.x + Vector3.up * hit.pushback.y;


        totalDamage = (int)(baseDamage * (atk.damage));


        int damageDealt = Mathf.RoundToInt((totalDamage * (1 - other.currentStats.defense)) - other.currentStats.resistance);


        if (damageDealt <= 0)
        {
            Instantiate(VFXManager.Instance.defaultBlockVFX, colPos.position, colPos.rotation);
        }
        else
        {
            //Hit FX
            if (hitVFX.prefab != null)
            {
                GameObject VFX = Instantiate(hitVFX.prefab, colPos.position, colPos.rotation);
                VFX.transform.localScale = hitVFX.scale;
            }
            else
                Instantiate(VFXManager.Instance.defaultHitVFX, colPos.position, colPos.rotation);

            if (hitSFX != null)
                Instantiate(hitSFX, colPos.position, colPos.rotation);
            else
                Instantiate(VFXManager.Instance.defaultHitSFX, colPos.position, colPos.rotation);

        }

        switch (atk.attackLevel)
        {
            case AttackLevel.Level1:
                other.TakeHit(damageDealt, aVector, CombatManager.Instance.lvl1.stun, CombatManager.Instance.lvl1.poiseBreak, aVector, CombatManager.Instance.lvl1.hitstop, hit.hitState, false);
                break;
            case AttackLevel.Level2:
                other.TakeHit(damageDealt, aVector, CombatManager.Instance.lvl2.stun, CombatManager.Instance.lvl2.poiseBreak, aVector, CombatManager.Instance.lvl2.hitstop, hit.hitState, false);
                break;
            case AttackLevel.Level3:
                other.TakeHit(damageDealt, aVector, CombatManager.Instance.lvl3.stun, CombatManager.Instance.lvl3.poiseBreak, aVector, CombatManager.Instance.lvl3.hitstop, hit.hitState, false);
                break;
            case AttackLevel.Custom:
                other.TakeHit(damageDealt, aVector, atk.stun, atk.poiseBreak, aVector, atk.hitstop, hit.hitState, false);
                break;
        }
    }
}