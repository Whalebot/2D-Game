using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Status : MonoBehaviour
{
    public Character character;

    public Alignment alignment = Alignment.Enemy;
    public GroundState groundState;
    public enum State { Neutral, Startup, Active, Recovery, Hitstun, Blockstun, Knockdown, Wakeup, InAnimation}
    public State currentState;

    [TabGroup("Current Stats")] public int hitstunValue;
    [TabGroup("Current Stats")] public int blockstunValue;
    [HideInInspector] public bool inBlockStun;
    [HideInInspector] public bool inHitStun;
    int staminaRegenCounter;
    [TabGroup("Current Stats")]
    [HideLabel] public Stats currentStats;
    [TabGroup("Modified Stats")]
    [HideLabel] public Stats modifiedStats;
    [TabGroup("Base Stats")]
    [HideLabel] public Stats baseStats;

    [TabGroup("Properties")] public bool hasArmor;
    [HideInInspector] public bool animationArmor;
    [TabGroup("Properties")] public bool counterhitState = false;
    [TabGroup("Properties")] public bool projectileInvul = false;
    [TabGroup("Properties")] public bool invincible = false;
    [TabGroup("Properties")] public bool airInvul = false;


    [TabGroup("Settings")] public bool debug;
    [TabGroup("Settings")] public int wakeupInvul;
    int counter;
    [Header("Auto destroy on death")]
    [TabGroup("Settings")] public bool autoDeath;
    [TabGroup("Settings")] public bool destroyParent;
    [TabGroup("Settings")] [ShowIf("autoDeath")] public float autoDeathTime = 1.5F;
    [HideInInspector] private Rigidbody rb;
    [HideInInspector] public Vector3 knockbackDirection;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool blocking;
    [HideInInspector] public bool parrying;
    [HideInInspector] public bool poiseBroken;
    [HideInInspector] public int parryStun;

    public event Action healthEvent;
    public event Action hurtEvent;
    public event Action deathEvent;
    public event Action pushbackEvent;

    public event Action neutralEvent;
    public event Action animationEvent;
    public event Action blockstunEvent;
    public event Action hitstunEvent;
    public event Action hitRecoveryEvent;
    public event Action invincibleEvent;

    CharacterSFX characterSFX;

    [HideInInspector] public bool godMode;
    [HideInInspector] public bool regenStamina;

    void Awake()
    {

        rb = GetComponent<Rigidbody>();
        characterSFX = GetComponentInChildren<CharacterSFX>();
        currentState = State.Neutral;

        ApplyCharacter();
    }

    private void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
    }

    void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
    }
    #region Properties
    public float Poise
    {
        get
        {
            return currentStats.poise;
        }
        set
        {
            currentStats.poise = Mathf.Clamp(value, 0, baseStats.poise);
        }
    }

    public int Health
    {
        get
        {
            return currentStats.currentHealth;
        }
        set
        {
            if (isDead)
                if (currentStats.currentHealth == value) return;
            if (godMode) return;

            currentStats.currentHealth = Mathf.Clamp(value, 0, currentStats.maxHealth);

            healthEvent?.Invoke();
            if (currentStats.currentHealth <= 0 && !isDead)
            {
                Death();
            }
        }
    }

    public int HitStun
    {
        get { return hitstunValue; }
        set
        {
            if (!hasArmor && !animationArmor)
            {
                if (value <= 0) return;

                hitstunValue = value;
                GoToState(State.Hitstun);
            }
        }
    }

    public int BlockStun
    {
        get { return blockstunValue; }
        set
        {
            blockstunValue = value;
            if (blockstunValue > 0)
                GoToState(State.Blockstun);
        }
    }
    #endregion
    void ExecuteFrame()
    {
        PoiseRegen();
        StateMachine();
    }

    public void EnableCollider() { }
    public void DisableCollider() { }

    public void RestoreStats()
    {
        ReplaceStats(currentStats, baseStats);
    }

    public void CalculateStats()
    {
        Stats tempStats = new Stats();
        tempStats.ResetValues();
        AddStats(tempStats, baseStats);
        AddStats(tempStats, modifiedStats);

        tempStats.currentHealth = Health;
        ReplaceStats(currentStats, tempStats);
    }

    public void ReplaceStats(Stats stat1, Stats stat2)
    {

        //Get stat definition and replace 1 with 2
        Stats def1 = stat1;
        Stats def2 = stat2;

        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;
            defInfo1[i].SetValue(obj, defInfo2[i].GetValue(obj2));
        }
    }

    public void AddStats(Stats oldStats, Stats newStats)
    {
        //Get stat definition and add 1 to 2
        Stats def1 = oldStats;
        Stats def2 = newStats;

        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;

            object var1 = defInfo1[i].GetValue(obj);
            object var2 = defInfo2[i].GetValue(obj2);

            if (var1 is int)
            {
                if ((int)var2 != 0)
                    defInfo1[i].SetValue(obj, (int)var1 + (int)var2);
            }
            else if (var1 is float)
            {
                if ((float)var2 != 0)
                    defInfo1[i].SetValue(obj, (float)var1 + (float)var2);
            }
        }
    }


    public void RemoveStats(Stats oldStats, Stats newStats)
    {
        //Get stat definition and remove 2 from 1
        Stats def1 = oldStats;
        Stats def2 = newStats;

        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;

            object var1 = defInfo1[i].GetValue(obj);
            object var2 = defInfo2[i].GetValue(obj2);

            if (var1 is int)
            {
                if ((int)var2 != 0)
                    defInfo1[i].SetValue(obj, (int)var1 - (int)var2);
            }
            else if (var1 is float)
            {
                if ((float)var2 != 0)
                    defInfo1[i].SetValue(obj, (float)var1 - (float)var2);
            }
        }
    }



    void StateMachine()
    {
        switch (currentState)
        {
            case State.Neutral:
                break;
            case State.InAnimation: break;
            case State.Hitstun: ResolveHitstun(); break;
            case State.Blockstun: ResolveBlockstun(); break;
            case State.Knockdown: ResolveHitstun(); break;
            case State.Wakeup:
                counter--;
                if (counter <= 0) GoToState(State.Neutral); break;
            default: break;
        }
    }
    public bool NonAttackState()
    {
        return currentState != State.Startup && currentState != State.Active && currentState != State.Recovery;
    }
    void PoiseRegen()
    {
        if (baseStats.poise > 0)
        {
            if (!poiseBroken)
                Poise = Mathf.Clamp(Poise + currentStats.poiseRegen, 0, baseStats.poise);
            else
            {
                Poise = Mathf.Clamp(Poise + currentStats.poiseRegen * 2, 0, baseStats.poise);
                if (Poise >= baseStats.poise)
                    poiseBroken = false;
            }
        }
    }

    public void GoToState(State transitionState)
    {
        if (debug) Debug.Log(transitionState);
        currentState = transitionState;

        switch (transitionState)
        {
            case State.Startup:
                blocking = false;
                break;
            case State.Active:
                blocking = false;
                break;
            case State.Recovery:
                if (rb != null)
                    rb.useGravity = true;
                blocking = false;
                break;
            case State.Knockdown:
                if (rb != null)
                    rb.useGravity = true;
                invincible = true;
                currentState = State.Knockdown;
                inHitStun = true;
                break;
            case State.Wakeup:
                invincible = true;
                counter = wakeupInvul;
                if (rb != null)
                    rb.useGravity = true;
                break;
            case State.Neutral:
                if (rb != null)
                    rb.useGravity = true;
                invincible = false;
                currentState = State.Neutral;
                neutralEvent?.Invoke();
                break;

            case State.InAnimation:
                currentState = State.InAnimation;
                animationEvent?.Invoke();
                break;
            case State.Hitstun:
                if (currentState != State.Knockdown)
                    currentState = State.Hitstun;
                hitstunEvent?.Invoke();
                if (rb != null)
                    rb.useGravity = true;
                inHitStun = true;
                break;
            case State.Blockstun:
                currentState = State.Blockstun;
                inBlockStun = true;
                blockstunEvent?.Invoke(); break;
            default: break;
        }
    }
    [Button]
    public void ApplyCharacter()
    {
        if (character != null)
        {
            ReplaceStats(baseStats, character.stats);
        }
        ReplaceStats(currentStats, baseStats);
    }


    public void TakeHit(int damage, Vector3 kb, int stunVal, int poiseBreak, Vector3 dir, float slowDur, HitState hitState)
    {
        if (damage == 0)
        {

            return;
        }
        float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, dir, Vector3.up));


        if (angle > 90)
        {
            if (parrying)
            {
                BlockStun = parryStun;
                return;
            }
            else if (blocking)
            {
                currentStats.currentStamina -= damage * 2;
                BlockStun = stunVal;

                TakePushback(kb);
                return;
            }
        }

        if (baseStats.poise > 0)
        {
            if (currentStats.poise > 0 && currentStats.poise <= poiseBreak && !poiseBroken)
            {
                TakePushback(kb);
                HitStun = stunVal;
                hurtEvent?.Invoke();
                PoiseBreak();
            }
            else if (currentStats.poise <= 0 || poiseBroken)
            {
                TakePushback(kb);
                HitStun = stunVal;
                hurtEvent?.Invoke();
                GameManager.Instance.Slowmotion(slowDur);
            }
        }
        else
        {
            TakePushback(kb);
            HitStun = stunVal;
            hurtEvent?.Invoke();
            GameManager.Instance.Slowmotion(slowDur);
        }

        if (hitState == HitState.Knockdown)
        {
            GoToState(State.Knockdown);
        }

        if (!poiseBroken)
            Poise -= poiseBreak;

        GameManager.Instance.DamageNumbers(transform, damage);
        Health -= damage;
    }
    public void PoiseBreak()
    {
        Poise = 0;
        poiseBroken = true;
        GameManager.Instance.Slowmotion(15);
    }

    public void TakePushback(Vector3 direction)
    {
        float temp = Vector3.SignedAngle(new Vector3(direction.x, 0, direction.z), transform.forward, Vector3.up);
        Vector3 tempVector = (Quaternion.Euler(0, temp, 0) * new Vector3(direction.x, 0, direction.z)).normalized;
        knockbackDirection = new Vector2(tempVector.x, tempVector.z);
        knockbackDirection = direction;

        if (!hasArmor && !animationArmor && rb != null)
        {
            pushbackEvent?.Invoke();
            rb.velocity = (Vector3.zero);
            rb.AddForce(direction, ForceMode.VelocityChange);
        }
    }

    public void Death()
    {
        isDead = true;
        deathEvent?.Invoke();
        Debug.Log(gameObject);
        if (autoDeath) StartCoroutine("DelayDeath");
    }

    IEnumerator DelayDeath()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(autoDeathTime);
        if (!destroyParent)
            Destroy(gameObject);
        else Destroy(transform.parent.gameObject);
    }

    void ResolveBlockstun()
    {
        if (blockstunValue > 0)
        {
            inBlockStun = true;
            blockstunValue--;
        }
        else if (blockstunValue <= 0 && inBlockStun)
        {

            GoToState(State.Neutral);
            blockstunValue = 0;
            inBlockStun = false;
        }


    }

    void ResolveHitstun()
    {
        if (hitstunValue > 0 && !hasArmor)
        {
            hitstunValue--;
            inHitStun = true;
        }

        if (hitstunValue <= 0 && inHitStun)
        {
            hitRecoveryEvent?.Invoke();
            if (currentState == State.Knockdown)
                GoToState(State.Wakeup);
            else
                GoToState(State.Neutral);
            hitstunValue = 0;
            inHitStun = false;
        }
    }
}

public enum Alignment
{
    Player,
    Enemy
}
public enum StatusEffect { Burning, Frozen };
public enum GroundState { Grounded, Airborne, Knockdown }