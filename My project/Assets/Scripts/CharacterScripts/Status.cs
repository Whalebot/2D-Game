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
    public enum State { Neutral, Startup, Active, Recovery, Hitstun, Blockstun, Knockdown, Wakeup, InAnimation }
    public State currentState;


    [HideInInspector] public bool inBlockStun;
    [HideInInspector] public bool inHitStun;
    int staminaRegenCounter;
    [TabGroup("Current Stats")]
    public List<StatusEffect> statusEffects;
    [HideInInspector] public List<StatusEffect> removedEffects;
    [HideLabel] public Stats currentStats;
    [TabGroup("Current Stats")] public int hitstunValue;
    [TabGroup("Current Stats")] public int blockstunValue;

    [TabGroup("Other Stats")]
    [Header("Modified stats")]
    [HideLabel] public Stats modifiedStats;
    [TabGroup("Other Stats")]
    [Header("Base stats")]
    [HideLabel] public Stats baseStats;

    [TabGroup("Properties")] public bool isFlying;
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

    [FoldoutGroup("Assign components")] public Collider hurtbox;
    [FoldoutGroup("Assign components")] public Collider col;
    MeshRenderer mr;

    public event Action healthEvent;
    public event Action statEvent;
    public event Action hurtEvent;
    public event Action deathEvent;
    public event Action pushbackEvent;

    public event Action neutralEvent;
    public event Action animationEvent;
    public event Action blockstunEvent;
    public event Action hitstunEvent;
    public event Action hitRecoveryEvent;
    public event Action invincibleEvent;

    [HideInInspector] public bool godMode;
    [HideInInspector] public bool regenStamina;

    void Awake()
    {

        rb = GetComponent<Rigidbody>();
        currentState = State.Neutral;

        ApplyCharacter();
    }

    private void Start()
    {
        if (hurtbox != null)
            mr = hurtbox.GetComponent<MeshRenderer>();

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
    public int Meter
    {
        get { return currentStats.currentMeter; }
        set
        {
            currentStats.currentMeter = Mathf.Clamp(value, 0, currentStats.maxMeter);
        }
    }


    public int Attack
    {
        get { return currentStats.attack; }
        set
        {
            currentStats.attack = value;
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
        ResolveStatusEffects();
        StateMachine();
    }
    public void ApplyStatusEffect(StatusEffect effect)
    {
        StatusEffect clone = null;
        foreach (var item in statusEffects)
        {
            if (effect.name == item.name)
            {
                clone = item;
            }
        }

        if (clone != null)
        {
            RefreshStatusEffect(clone);
        }
        else
        {
            clone = Instantiate(effect);
            clone.name = effect.name;
            statusEffects.Add(clone);
            clone.ActivateBehaviour(this);
        }
    }
    public bool HasStatusEffect(StatusEffect effect)
    {
        if (statusEffects.Contains(effect)) return true;

        return false;
    }
    public void RefreshStatusEffect(StatusEffect effect)
    {
        if (statusEffects.Contains(effect))
            effect.RefreshBehaviour();
    }
    void ResolveStatusEffects()
    {
        //Do stuff every frame
        foreach (var item in statusEffects)
        {
            item.ExecuteFrame();
        }
        if (removedEffects.Count > 0)
        {
            foreach (var item in removedEffects)
            {
                if (statusEffects.Contains(item))
                    statusEffects.Remove(item);
            }
            removedEffects.Clear();
        }
    }
    public void RemoveStatusEffect(StatusEffect effect)
    {
        removedEffects.Add(effect);
    }
    public void EnableCollider()
    {
        if (!isDead)
            col.gameObject.layer = LayerMask.NameToLayer("Collision");
    }
    public void DisableCollider()
    {
        if (!isDead)
            col.gameObject.layer = LayerMask.NameToLayer("Noclip");
        //Debug.Log(LayerMask.LayerToName(col.gameObject.layer));
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
        return currentState != State.Startup && currentState != State.Active && currentState != State.Recovery && currentState != State.Hitstun;
    }
    void PoiseRegen()
    {
        if (baseStats.poise > 0)
        {
            if (!poiseBroken)
                Poise = Mathf.Clamp(Poise + currentStats.poiseRegen, 0, baseStats.poise);
            else
            {
                Poise = Mathf.Clamp(Poise + currentStats.poiseRegen * 4, 0, baseStats.poise);
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
                if (rb != null && !isFlying)
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
                if (rb != null && !isFlying)
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
        if (alignment != Alignment.Player || !SaveManager.Instance.HasSaveData())
            ReplaceStats(currentStats, baseStats);
    }
    public void TakeHit(int damage, Vector3 kb, int stunVal, int poiseBreak, Vector3 dir, float slowDur, HitState hitState, bool crit = false)
    {
        if (isDead) return;

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
                BlockStun = stunVal;
                TakePushback(kb);
                return;
            }
        }

        GameManager.Instance.DamageNumbers(transform, damage, crit);
        Health -= damage;

        if (hasArmor)
        {
            return;
        }

        if (baseStats.poise > 0)
        {
            if (baseStats.poise > 0 && currentStats.poise <= poiseBreak && !poiseBroken)
            {
                TakePushback(kb);
                HitStun = stunVal;
                hurtEvent?.Invoke();
                PoiseBreak();
            }
            else if (currentStats.poise <= 0 || poiseBroken)
            {
                TakePushback(kb);

                if (stunVal > 0)
                {
                    HitStun = stunVal;
                    hurtEvent?.Invoke();
                    GameManager.Instance.Slowmotion(slowDur);
                }
            }
            else
            {
                Poise -= poiseBreak;
            }
        }
        else
        {

            TakePushback(kb);
            if (stunVal > 0)
            {
                HitStun = stunVal;
                hurtEvent?.Invoke();
                GameManager.Instance.Slowmotion(slowDur);
            }
        }

        if (hitState == HitState.Knockdown)
        {
            GoToState(State.Knockdown);
        }


    }
    public void PoiseBreak()
    {
        Poise = 0;
        poiseBroken = true;
        GameManager.Instance.Slowmotion(15);
    }
    public void TakePushback(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        float temp = Vector3.SignedAngle(new Vector3(direction.x, 0, direction.z), transform.forward, Vector3.up);
        Vector3 tempVector = (Quaternion.Euler(0, temp, 0) * new Vector3(direction.x, 0, direction.z)).normalized;
        knockbackDirection = new Vector2(tempVector.x, tempVector.z);
        knockbackDirection = direction;

        if (!hasArmor && !animationArmor && rb != null)
        {
            pushbackEvent?.Invoke();
            rb.velocity = direction;
        }
    }
    public void Death()
    {
        isDead = true;

        deathEvent?.Invoke();
        invincible = true;
        hurtbox.enabled = false;
        col.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        if (autoDeath) StartCoroutine(DelayDeath());
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
    public void UpdateStats()
    {
        statEvent?.Invoke();
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

    #region Calculate Stats
    public void RestoreStats()
    {
        //Get stat definition and replace 1 with 2
        Stats def1 = currentStats;
        Stats def2 = baseStats;

        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;
            if (defInfo1[i].Name != "currentHealth" && defInfo1[i].Name != "currentMeter" && defInfo1[i].Name != "gold")
                defInfo1[i].SetValue(obj, defInfo2[i].GetValue(obj2));
        }
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
    public void ReplaceStats(Stats oldStats, Stats newStats)
    {

        //Get stat definition and replace 1 with 2
        Stats def1 = oldStats;
        Stats def2 = newStats;

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
    #endregion
}

public enum Alignment
{
    Player,
    Enemy,
    Neutral
}
public enum GroundState { Grounded, Airborne, Knockdown }