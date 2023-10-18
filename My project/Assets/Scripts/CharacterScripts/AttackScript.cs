using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class AttackScript : MonoBehaviour
{
    private Status status;
    [FoldoutGroup("Components")] public Transform hitboxContainer;
    [FoldoutGroup("Components")] public List<GameObject> hitboxes;
    [FoldoutGroup("Components")] public Transform vfxContainer;

    Movement movement;
    CharacterSFX sfx;

    public delegate void AttackEvent();
    public AttackEvent emptyAttackEvent;
    public AttackEvent startupEvent;
    public AttackEvent topAnimationEvent;
    public AttackEvent activeEvent;
    public AttackEvent recoveryEvent;
    public AttackEvent parryEvent;
    public AttackEvent blockEvent;
    public AttackEvent jumpEvent;
    public AttackEvent jumpCancelEvent;
    public delegate void MoveEvent(Move move);
    public MoveEvent activateHitboxEvent;
    public MoveEvent deactivateHitboxEvent;
    public MoveEvent attackHitEvent;
    public MoveEvent attackPerformedEvent;
    public Moveset moveset;
    [HeaderAttribute("Attack attributes")]
    [TabGroup("Debug")] public bool attacking;
    [TabGroup("Debug")] public Move activeMove;
    [TabGroup("Debug")] public bool hit;
    [TabGroup("Debug")] public int gatlingFrame;
    [TabGroup("Debug")] public int attackID;
    [TabGroup("Debug")] public float attackFrames;
    [TabGroup("Debug")] public int lastFrame;
    [TabGroup("Debug")] public int extendedBuffer;

    [TabGroup("Debug")] public bool holdAttack;
    [TabGroup("Debug")] public bool attackString;
    [TabGroup("Debug")] public bool canTargetCombo;
    [TabGroup("Debug")] public bool landCancel;
    [TabGroup("Debug")] public bool jumpCancel;
    [TabGroup("Debug")] public bool specialCancel;
    [TabGroup("Debug")] public bool recoverOnlyOnLand;
    [TabGroup("Debug")] public int movementFrames;
    [TabGroup("Debug")] public List<GameObject> projectiles;
    [TabGroup("Debug")] public bool inMomentum;
    [TabGroup("Jump Startup")] public int jumpFrameCounter;
    [TabGroup("Jump Startup")] public int jumpActionDelay;
    [TabGroup("Jump Startup")] public int jumpActionDelayCounter;
    [TabGroup("Jump Startup")] public bool jumpDelay;

    [HideInInspector] public bool newAttack;
    [HideInInspector] public bool landCancelFrame;
    [HideInInspector] public int combo;
    List<Move> usedMoves;

    private void Awake()
    {
        usedMoves = new List<Move>();

        status = GetComponent<Status>();
        movement = GetComponent<Movement>();
        sfx = GetComponentInChildren<CharacterSFX>();
        movement.jumpEvent += JumpCancel;
        movement.doubleJumpEvent += JumpCancel;
        movement.landEvent += Land;
        status.neutralEvent += ResetCombo;
        status.hurtEvent += HitstunEvent;
        status.deathEvent += HitstunEvent;
        GameManager.Instance.advanceGameState += ExecuteFrame;
        GameManager.Instance.resetEvent += ResetAttack;
    }

    private void OnDisable()
    {
        movement.jumpEvent -= JumpCancel;
        movement.doubleJumpEvent -= JumpCancel;
        movement.landEvent -= Land;
        status.neutralEvent -= ResetCombo;
        status.hurtEvent -= HitstunEvent;
        status.deathEvent -= HitstunEvent;

        GameManager.Instance.advanceGameState -= ExecuteFrame;
        GameManager.Instance.resetEvent -= ResetAttack;
    }

    public int AttackFrame
    {
        get { return (int)attackFrames; }
    }

    void ResetAttack()
    {
        ResetAllValues();
    }

    public void ExecuteFrame()
    {
        ExecutePreJump();

        if (attacking)
        {
            //Advance frame
            if (activeMove.useAttackSpeed)
            {
                float deltaFrame = (1 / 0.01666666F * Time.fixedDeltaTime) * status.currentStats.attackSpeed;
                int frameSkips = (int)(attackFrames + deltaFrame - AttackFrame);

                //Execute skipped frames if using attack speed
                if (frameSkips > 1)
                {
                    for (int i = frameSkips - 1; i > 0; i--)
                    {
                        int frameToExecute = (int)(attackFrames + deltaFrame - i);

                        ExecuteUniqueProperties(frameToExecute);

                        ProcessInvul(frameToExecute);
                        ApplyScreenShake(frameToExecute);
                        SpawnFX(frameToExecute);

                        //Execute momentum
                        ExecuteMomentum(frameToExecute, frameSkips);
                    }
                    //replay skipped frames
                }
                attackFrames = attackFrames + deltaFrame;
            }
            else attackFrames++;

            if (activeMove.attacks.Length > 0)
            {
                if (attackFrames > gatlingFrame + activeMove.attacks[0].gatlingFrames)
                {
                    attackString = true;
                    newAttack = false;
                }
                if (attackFrames > activeMove.firstStartupFrame + activeMove.attacks[0].gatlingFrames)
                {
                    canTargetCombo = true;
                }
                if (extendedBuffer > 0)
                    extendedBuffer--;
            }
            //Execute properties
            ExecuteUniqueProperties(AttackFrame);

            ProcessInvul(AttackFrame);
            ApplyScreenShake(AttackFrame);
            SpawnFX(AttackFrame);

            //Execute momentum
            ExecuteMomentum(AttackFrame);


            if (attackFrames > activeMove.totalMoveDuration)
            {
                Idle();
            }
            else if (attackFrames < activeMove.firstStartupFrame)
            {
                StartupFrames();
            }
            else if (attackFrames <= activeMove.lastActiveFrame)
            {
                ActiveFrames();
                if (recoverOnlyOnLand) attackFrames--;
            }


            else if (attackFrames <= activeMove.totalMoveDuration)
            {
                RecoveryFrames();
            }

            lastFrame = AttackFrame;
        }

        if (status.currentState == Status.State.Neutral || status.currentState == Status.State.Blockstun || status.currentState == Status.State.Hitstun) usedMoves.Clear();

    }
    void ExecuteUniqueProperties(int frame)
    {
        if (lastFrame == AttackFrame) return;
        foreach (var item in activeMove.uniqueProperties)
        {
            if ((int)frame == item.frame)
            {
                item.OnStartupFrame(this);
            }
        }
        foreach (var item in activeMove.uniqueProperties)
        {
            if ((int)frame <= activeMove.lastActiveFrame && (int)frame >= activeMove.firstStartupFrame)
            {
                item.OnActiveFrames(this);
            }
        }
    }
    private void ExecutePreJump()
    {
        landCancelFrame = false;
        if (jumpFrameCounter > 0)
        {
            jumpFrameCounter--;
            if (jumpFrameCounter <= 0)
            {
                status.GoToState(Status.State.Neutral);
                jumpActionDelayCounter = jumpActionDelay;
            }
        }
        if (jumpActionDelayCounter > 0)
        {
            jumpDelay = true;
            jumpActionDelayCounter--;
            if (jumpActionDelayCounter <= 0)
            {
                jumpDelay = false;
            }
        }
    }

    void ExecuteMomentum(int frame = 0, int skippedFrames = 1)
    {
        bool tempMomentum = false;

        for (int i = 0; i < activeMove.m.Length; i++)
        {
            if (AttackFrame > activeMove.m[i].startFrame && AttackFrame < activeMove.m[i].startFrame + activeMove.m[i].duration)
            {
                tempMomentum = true;
            }
            //Recovery
            if (AttackFrame > activeMove.m[i].startFrame + activeMove.m[i].duration)
            {
                movement.forcedWalk = false;
                if (activeMove.m[i].resetVelocityDuringRecovery)
                {
                    movement._rb.velocity = Vector3.zero;
                }
            }
            else if (AttackFrame >= activeMove.m[i].startFrame && AttackFrame < activeMove.m[i].startFrame + activeMove.m[i].duration)
            {
                //Debug.Log($"{AttackFrame} + {i} + {activeMove.m[i].startFrame + activeMove.m[i].duration}");
                if (!movement.ground) movement._rb.useGravity = false;
                //Debug.Log(status.currentStats.attackSpeed *  activeMove.m[i].duration/ activeMove.m[i].duration);
                movement.SetVelocity((activeMove.m[i].momentum.x * transform.forward + transform.up * activeMove.m[i].momentum.y));
            }
        }
        inMomentum = tempMomentum;
    }

    public void SpawnFX(int frame = 0)
    {
        if (activeMove != null)
        {
            if (activeMove.vfx.Length > 0)
                foreach (var item in activeMove.vfx)
                {
                    if (frame == item.startup)
                    {
                        GameObject fx = Instantiate(item.prefab, transform.position, transform.rotation, vfxContainer);
                        fx.transform.localPosition = item.position;
                        fx.transform.localRotation = Quaternion.Euler(item.rotation);
                        fx.transform.localScale = item.scale;
                        if (item.destroyOnRecovery)
                            fx.GetComponent<VFXScript>().SetupVFX(status);
                        if (item.deattachFromPlayer)
                            fx.transform.SetParent(null);
                    }
                }
            if (activeMove.sfx.Length > 0)
                foreach (var item in activeMove.sfx)
                {
                    if (frame == item.startup)
                    {
                        GameObject fx = Instantiate(item.prefab, transform.position, transform.rotation, hitboxContainer);
                        fx.transform.localPosition = item.prefab.transform.localPosition;
                        fx.transform.localRotation = item.prefab.transform.rotation;
                        fx.transform.SetParent(null);
                    }
                }
        }
    }

    void ApplyScreenShake(int frame = 0)
    {
        for (int i = 0; i < activeMove.screenShake.Length; i++)
        {
            if (attackFrames == activeMove.screenShake[i].startup && activeMove.screenShake[i].type == ScreenShakeType.OnStartup)
                CameraManager.Instance.ShakeCamera(activeMove.screenShake[i].amplitude, activeMove.screenShake[i].duration);
        }
    }
    public void StartupFrames()
    {
        status.GoToState(Status.State.Startup);
    }
    void ClearHitboxes()
    {
        deactivateHitboxEvent?.Invoke(activeMove);
        for (int i = 0; i < hitboxes.Count; i++)
        {
            if (hitboxes[i] != null)
            {
                Destroy(hitboxes[i]);
            }
        }
        hitboxes.Clear();
    }

    public void ActiveFrames()
    {
        for (int i = 0; i < activeMove.attacks.Length; i++)
        {
            if (AttackFrame < activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames && AttackFrame >= activeMove.attacks[i].startupFrame)
            {
                status.GoToState(Status.State.Active);

                if (activeMove.attacks[i].hitbox == null)
                    //Activate Weapon Default Hitbox
                    activateHitboxEvent?.Invoke(activeMove);
                //Else activate custom hitbox
                else if (activeMove.attacks[i].hitbox != null)
                {
                    if (hitboxes.Count < i + 1)
                    {

                        if (activeMove.attacks[i].attackType == AttackType.Projectile)
                        {
                            hitboxes.Add(Instantiate(activeMove.attacks[i].hitbox, hitboxContainer.position, transform.rotation, hitboxContainer));
                            hitboxes[i].transform.localPosition = activeMove.attacks[i].hitbox.transform.localPosition;
                            hitboxes[i].transform.localRotation = activeMove.attacks[i].hitbox.transform.rotation;
                            hitboxes[i].transform.SetParent(null);
                        }
                        else
                        {
                            hitboxes.Add(Instantiate(activeMove.attacks[i].hitbox, hitboxContainer.position, transform.rotation, hitboxContainer));
                            hitboxes[i].transform.localPosition = activeMove.attacks[i].hitbox.transform.localPosition;
                            hitboxes[i].transform.localScale = activeMove.attacks[i].hitbox.transform.localScale * status.currentStats.sizeModifier;
                            hitboxes[i].transform.localRotation = activeMove.attacks[i].hitbox.transform.rotation;
                        }
                        Hitbox hitbox = hitboxes[i].GetComponentInChildren<Hitbox>();
                        hitbox.hitboxID = i;
                        hitbox.attack = this;
                        hitbox.status = status;
                        hitbox.move = activeMove;
                        if (activeMove.attacks[i].attackType == AttackType.Projectile)
                        {
                            projectiles.Add(hitboxes[i]);
                            hitboxes[i] = null;
                        }
                    }
                }
            }
            else if (AttackFrame > activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames)
            {
                if (activeMove.attacks[i].hitbox == null)
                    deactivateHitboxEvent?.Invoke(activeMove);
                else if (activeMove.attacks[i].hitbox != null)
                {
                    if (hitboxes.Count == i + 1)
                    {
                        Destroy(hitboxes[i]);
                    }
                }
            }
        }
    }

    public void RecoveryFrames()
    {
        newAttack = false;
        status.GoToState(Status.State.Recovery);
        ClearHitboxes();
    }

    void ProcessInvul(int frame = 0)
    {
        //Execute properties
        //Invul
        if (activeMove.invincible)
        {
            if (frame == activeMove.invincibleStart)
            {
                status.invincible = true;
            }
            else if (frame >= activeMove.invincibleStart + activeMove.invincibleDuration)
            {
                status.invincible = false;
            }
        }
        //Noclip
        if (activeMove.noClip)
        {
            if (frame == activeMove.noClipStart
                //&& AttackFrame < activeMove.noClipStart + activeMove.noClipDuration
                )
                status.DisableCollider();
            else if (frame >= activeMove.noClipStart + activeMove.noClipDuration)
            {
                status.EnableCollider();
            }
        }
        //Projectile Invul
        if (activeMove.projectileInvul)
        {
            if (frame == activeMove.projectileInvulStart)
                status.projectileInvul = true;
            else if (frame >= activeMove.projectileInvulStart + activeMove.projectileInvulDuration)
            {
                status.projectileInvul = false;
            }
        }
        //air Invul
        if (activeMove.airInvul)
        {
            if (frame == activeMove.airInvulStart)
                status.airInvul = true;
            else if (frame >= activeMove.airInvulStart + activeMove.airInvulDuration)
            {
                status.airInvul = false;
            }
        }
    }

    public void AttackProperties(Move move)
    {
        if (move == null)
        {
            print(move);
            return;
        }
        usedMoves.Add(move);
        ClearHitboxes();

        if (move.resetGatling) usedMoves.Clear();

        if (move.type == MoveType.Movement)
        {
            movementFrames = GameManager.Instance.gameFrameCount;
        }

        status.Meter -= move.meterCost;

        recoverOnlyOnLand = move.recoverOnlyOnLand;
        activeMove = move;
        attackID = move.animationID;

        attackString = false;
        canTargetCombo = false;
        hit = false;
        jumpCancel = false;
        specialCancel = false;
        holdAttack = move.holdAttack;
        attackFrames = 0;

        if (move.instantStartupRotation) movement.Rotation();


        //Run momentum
        //if (move.overrideVelocity) movement._rb.velocity = Vector3.zero;
        //else if (move.runMomentum) movement._rb.velocity = movement._rb.velocity * 0.5F;

        //Air properties
        if (move.useAirAction) movement.performedJumps++;

        status.EnableCollider();
        Startup();
        landCancel = move.landCancel;

        attackPerformedEvent?.Invoke(move);

        if (move.attacks.Length > 0)
        {
            startupEvent?.Invoke();
        }
        else
        {
            startupEvent?.Invoke();
            //emptyAttackEvent?.Invoke();
        }
        attacking = true;
        newAttack = true;
        ExecuteFrame();
    }

    public bool CanUseMove(Move move)
    {
        if (move == null) return false;


        if (status.currentStats.currentMeter < move.meterCost) return false;
        //if (jumpFrameCounter > 0) return false;
        if (move.useAirAction && !attacking)
        {
            if (movement.performedJumps <= 0)
            {
                movement.performedJumps++;
                return true;
            }
            else return false;
        }

        if (!attacking) return true;

        if (activeMove != null)
        {
            if (activeMove.uncancelable) return false;
            else if (move.fullCancel) return true;
        }

        if (move != null && canTargetCombo)
        {
            if (move.gatlingCancel) return true;
        }
        //       Debug.Log("no true");
        return false;
    }

    public bool TargetCombo(Move move)
    {
        //if (jumpFrameCounter > 0) return false;
        if (move == null) return false;
        if (move.useAirAction)
        {
            if (movement.performedJumps > movement.multiJumps)
            {
                return false;
            }
        }
        if (attacking && canTargetCombo)
        {
            if (activeMove.targetComboMoves.Count > 0)
            {
                if (activeMove.targetComboMoves.Contains(move))
                {
                    AttackProperties(move);
                    return true;
                }

                //if (usedMoves.Contains(move) && activeMove == move || move.targetComboMoves.Contains(activeMove))
                //{
                //    Attack(move.targetComboMoves[0]);
                //    return true;
                //}

            }
        }
        return false;

    }

    public bool HasBeenUsed(Move move)
    {
        if (usedMoves.Contains(move))
        {
            int duplicates = 1;
            foreach (var item in usedMoves)
            {
                if (item == move) duplicates--;
            }
            return duplicates <= 0;
        }
        else return false;
    }

    public bool ComboAttack(Combo c)
    {
        if (c.moves.Count <= 0) return false;

        if (c.moves.Count <= combo)
            return Attack(c.moves[0]);

        if (!CanUseMove(c.moves[combo]))
        {
            //Debug.Log("Can't use move");
            return false;
        }
        else
            Attack(c.moves[combo]);

        AttackProperties(c.moves[combo]);
        combo++;
        return true;
    }

    public bool Attack(Move move)
    {
        if (move == null) return false;
        if (TargetCombo(move))
        {
            return true;
        }
        if (HasBeenUsed(move))
        {
            return false;
        }
        if (!CanUseMove(move))
        {
            return false;
        }
        else
        {
            AttackProperties(move);
            return true;
        }

    }

    void Startup()
    {
        status.GoToState(Status.State.Startup);
    }
    public void ReleaseButton()
    {
        if (holdAttack && attacking)
        {
            Debug.Log("Release Hold Move");
            attackFrames = activeMove.lastActiveFrame + 1;
        }
    }

    void Land()
    {
        if (recoverOnlyOnLand)
        {
            Debug.Log("Land Recovery");
            attackFrames = activeMove.lastActiveFrame + 1;

        }
        recoverOnlyOnLand = false;
        if (activeMove != null)
            for (int i = 0; i < activeMove.screenShake.Length; i++)
            {
                if (activeMove.screenShake[i].type == ScreenShakeType.OnLand)
                    CameraManager.Instance.ShakeCamera(activeMove.screenShake[i].amplitude, activeMove.screenShake[i].duration);
            }

        if (landCancel)
        {
            newAttack = false;
            landCancelFrame = true;
            Idle();
        }
    }

    void ResetCombo()
    {
        combo = 0;
    }

    void HitstunEvent()
    {
        ResetAllValues();
    }

    public void JumpCancel()
    {
        if (attacking)
        {
            movement._rb.velocity = Vector3.zero;
            jumpCancelEvent?.Invoke();
            status.GoToState(Status.State.Neutral);
            // Idle();
        }
        attackString = false;
        canTargetCombo = false;

        if (activeMove != null)
        {
            activeMove = null;
        }

        combo = 0;
        ClearHitboxes();
        attacking = false;
        landCancel = false;
        recoveryEvent?.Invoke();
    }

    void ClearVFX()
    {
        VFXManager.Instance.DestroyParticles(status);
    }

    public void ResetAllValues()
    {
        ClearHitboxes();
        ClearVFX();
        newAttack = false;
        attackString = false;


        extendedBuffer = 0;
        combo = 0;
        recoverOnlyOnLand = false;
        jumpFrameCounter = 0;
        specialCancel = false;
        attacking = false;
        canTargetCombo = false;
        landCancel = false;
        hit = false;
        status.counterhitState = false;
        status.projectileInvul = false;
        status.EnableCollider();
        status.invincible = false;
        movement.forcedWalk = false;

        if (activeMove != null)
        {
            if (movement.ground && !activeMove.keepVelocity)
                movement.ResetVelocity();

            activeMove = null;
        }
        else if (movement.ground)
            movement.ResetVelocity();


        recoveryEvent?.Invoke();
        usedMoves.Clear();
    }

    public void Idle()
    {
        ResetAllValues();
        status.GoToState(Status.State.Neutral);
    }
}
