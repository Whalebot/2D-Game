using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float baseDamage = 1;
    public int totalDamage;
    public int resetTimer = 30;
    public int life = 0;
    public int lifeCounter;
    public bool followParent = false;
   public int resetcounter;
    public Alignment alignment;
    public Attack attack;
    public StatusEffect effect;
    public VFX hitVFX;
    public SFX hitSFX;
    public GameObject col;
    public ParticleSystem ps;
    Vector3 knockbackDirection;
    Vector3 aVector;
    Rigidbody rb;
    public List<Status> enemyList;
    protected Transform colPos;
    // Start is called before the first frame update
    void Start()
    {
        if (followParent)
            rb = transform.parent.GetComponentInParent<Rigidbody>();
        GameManager.Instance.advanceGameState += ExecuteFrame;
        resetcounter = resetTimer;
    }
    private void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
    }

    void ExecuteFrame()
    {
        if (followParent)
        {
            transform.position = transform.parent.position;
            attack.groundHitProperty.pushback = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        if (enemyList.Count > 0)
        {
            resetcounter--;
            if (resetcounter <= 0)
            {
                col.gameObject.SetActive(false);
                enemyList.Clear();
                resetcounter = resetTimer;
            }

        }
        else
        {
            col.gameObject.SetActive(true);
        }

        if (life > 0)
        {
            lifeCounter++;
            if (lifeCounter >= life)
                StartCoroutine(DelayDestroy());
        }
    }

    IEnumerator DelayDestroy()
    {
        col.gameObject.SetActive(false);
        if (ps != null)
        {
            ParticleSystem[] temp = GetComponentsInChildren<ParticleSystem>();
            foreach (var item in temp)
            {
                item.Stop();
            }

        }
        GameManager.Instance.advanceGameState -= ExecuteFrame;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    public void OnTriggerStay(Collider other)
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
                if (effect != null)
                    enemyStatus.ApplyStatusEffect(effect);
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
        aVector = Vector3.right * hit.pushback.x + Vector3.up * hit.pushback.y;


        totalDamage = (int)(baseDamage * (atk.damage));


        int damageDealt = Mathf.RoundToInt((totalDamage * (1 - other.currentStats.defense)) - other.currentStats.resistance);


        if (baseDamage == 0) { }
        else if (damageDealt <= 0)
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


            if (hitSFX.audioClips.Count > 0)
                AudioManager.Instance.PlaySFX(hitSFX, colPos.position);
            else
                AudioManager.Instance.PlaySFX(VFXManager.Instance.defaultHitSFX, colPos.position);
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
