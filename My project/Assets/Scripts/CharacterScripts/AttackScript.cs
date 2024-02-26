using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class AttackScript : MonoBehaviour
{
    [HideInInspector] public Status status;
    [FoldoutGroup("Components")] public Transform hitboxContainer;
    [FoldoutGroup("Components")] public List<GameObject> hitboxes;
    [FoldoutGroup("Components")] public List<GameObject> graphics;
    [FoldoutGroup("Components")] public Transform vfxContainer;

    Movement movement;

    public event Action emptyAttackEvent;
    public event Action<int> frameEvent;
    public event Action startupEvent;
    public event Action blendAttackEvent;
    public event Action topAnimationEvent;
    public event Action activeEvent;
    public event Action lastActiveFrameEvent;
    public event Action recoveryEvent;
    public event Action parryEvent;
    public event Action blockEvent;
    public event Action jumpEvent;
    public event Action jumpCancelEvent;

    public event Action<HitInfo> hitEvent;

    public delegate void MoveEvent(Move move);
    public MoveEvent activateHitboxEvent;
    public MoveEvent deactivateHitboxEvent;
    public MoveEvent attackHitEvent;
    public MoveEvent attackPerformedEvent;

    public Moveset moveset;
    [HeaderAttribute("Attack attributes")]
    [TabGroup("Debug")] public bool attacking;
    [TabGroup("Debug")] public Move activeMove;
    [TabGroup("Debug")] public Combo activeCombo;
    [TabGroup("Debug")] public Vector3 homingDirection;
    [TabGroup("Debug")] public int releaseMinimum;
    [TabGroup("Debug")] public bool delayRelease;
    [TabGroup("Debug")] public bool hit;
    [TabGroup("Debug")] public int gatlingFrame;
    [TabGroup("Debug")] public int attackID;
    [TabGroup("Debug")] public float attackFrames;
    [TabGroup("Debug")] public int lastFrame;
    [TabGroup("Debug")] public int airActionCounter;
    [TabGroup("Debug")] public int extendedBuffer;

    [TabGroup("Debug")] public bool isDashing;
    [TabGroup("Debug")] public bool holdAttack;
    [TabGroup("Debug")] public bool attackString;
    [TabGroup("Debug")] public bool canTargetCombo;
    [TabGroup("Debug")] public bool landCancel;
    [TabGroup("Debug")] public bool recoverOnlyOnLand;
    [TabGroup("Debug")] public int movementFrames;
    [TabGroup("Debug")] public List<GameObject> projectiles;
    [TabGroup("Debug")] public bool inMomentum;
    [TabGroup("Debug")] public int combo;
    [TabGroup("Jump Startup")] public int jumpFrameCounter;
    [TabGroup("Jump Startup")] public int jumpActionDelay;
    [TabGroup("Jump Startup")] public int jumpActionDelayCounter;
    [TabGroup("Jump Startup")] public bool jumpDelay;

    [HideInInspector] public bool newAttack;
    [HideInInspector] public bool landCancelFrame;

    List<Move> usedMoves;

    private void Awake()
    {
        usedMoves = new List<Move>();

        status = GetComponent<Status>();
        movement = GetComponent<Movement>();
        movement.jumpEvent += JumpCancel;
        movement.doubleJumpEvent += JumpCancel;
        movement.landEvent += Land;
        status.neutralEvent += ResetCombo;
        status.hurtEvent += HitstunEvent;
        status.deathEvent += DeathEvent;
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
        status.deathEvent -= DeathEvent;

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
        if (status.isDead) return;
        ExecutePreJump();

        if (attacking)
        {
            if (activeMove == null) { Debug.Log("No move"); }
            //Advance frame
            //Execute skipped frames if using attack speed

            float deltaFrame = (1 / 0.01666666F * Time.fixedDeltaTime);
            if (activeMove.useAttackSpeed)
            {
                deltaFrame *= status.currentStats.attackSpeed;
            }
            int frameSkips = (int)(attackFrames + deltaFrame - AttackFrame);

            if (frameSkips >= 1)
            {
                for (int i = frameSkips - 1; i >= 0; i--)
                {
                    int frameToExecute = (int)(attackFrames + deltaFrame - i);

                    //Debug.Log(frameToExecute);
                    if (frameToExecute > activeMove.totalMoveDuration)
                    {
                        Idle();
                        break;
                    }
                    //replay skipped frames
                    ExecuteAttackFrame(frameToExecute);
                    lastFrame = frameToExecute;
                }
            }
            attackFrames = attackFrames + deltaFrame;
        }

        if (status.currentState == Status.State.Neutral || status.currentState == Status.State.Blockstun || status.currentState == Status.State.Hitstun) usedMoves.Clear();
    }
    void ExecuteAttackFrame(int frame)
    {

        if (delayRelease && frame >= releaseMinimum)
        {
            ReleaseButton();
        }
        //Execute properties
        ExecuteUniqueProperties(frame);

        ProcessInvul(frame);
        ApplyScreenShake(frame);
        SpawnFX(frame);

        //Execute momentum
        ExecuteMomentum(frame);


        if (frame > activeMove.firstGatlingFrame)
        {
            attackString = true;
            newAttack = false;
            canTargetCombo = true;
        }

        if (extendedBuffer > 0)
            extendedBuffer--;


        if (frame == activeMove.firstStartupFrame && activeMove.consumeMeterOnActiveFrame)
        {
            status.Meter -= activeMove.meterCost;
        }

        if (frame < activeMove.firstStartupFrame)
        {
            StartupFrames();
        }
        else if (frame <= activeMove.lastActiveFrame)
        {
            ActiveFrames(frame);
            if (recoverOnlyOnLand) attackFrames--;
        }
        else if (frame <= activeMove.totalMoveDuration)
        {
            RecoveryFrames();
        }
    }
    void ExecuteUniqueProperties(int frame)
    {
        frameEvent?.Invoke(frame);

        //if (lastFrame == frame) { Debug.Log($"{lastFrame} {frame}"); }
        foreach (var item in activeMove.skillProperties)
        {
            if (frame == item.frame)
            {
                item.OnStartupFrame(this, frame);
            }
        }
        foreach (var item in activeMove.skillProperties)
        {
            if (frame <= activeMove.lastActiveFrame && (int)frame >= activeMove.firstStartupFrame)
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
            if (frame > activeMove.m[i].startFrame && frame < activeMove.m[i].startFrame + activeMove.m[i].duration)
            {
                tempMomentum = true;
            }

            if (activeMove.m[i].teleport)
            {
                if (frame == activeMove.m[i].startFrame)
                    transform.position = transform.position + (transform.forward * activeMove.m[i].momentum.x + transform.up * activeMove.m[i].momentum.y);
            }
            else
            {

                //Recovery
                if (frame == activeMove.m[i].startFrame + activeMove.m[i].duration)
                {
                    movement.forcedWalk = false;
                    if (activeMove.m[i].resetVelocityDuringRecovery)
                    {
                        movement.SetVelocity(Vector3.zero);
                    }
                }
                else
                if (frame >= activeMove.m[i].startFrame && frame < activeMove.m[i].startFrame + activeMove.m[i].duration)
                {
                    if (frame == activeMove.m[i].startFrame && activeMove.m[i].momentum.y > 0)
                    {
                        movement.CollisionPassthrough();
                    }

                    if (activeMove.m[i].freeMovement)
                    {
                        movement.AttackMovement();
                        return;
                    }

                    if (!movement.ground) movement._rb.useGravity = false;

                    //else if (activeMove.m[i].momentum.y < 0 && activeMove.noClip == false)
                    //    status.EnableCollider();


                    if (activeMove.homing)
                        movement.SetVelocity(activeMove.m[i].momentum.x * (Mathf.Abs(homingDirection.x) * transform.forward + transform.up * homingDirection.y));
                    else
                    {
                        if (activeMove.inheritForwardVelocity && activeMove.m[i].momentum.x == 0)
                        {
                            if (activeMove.m[i].momentum.y == 0 && movement._rb.velocity.y > 0)
                            {
                                movement.SetVelocity(Mathf.Abs(movement._rb.velocity.x) * transform.forward + transform.up * movement._rb.velocity.y);
                            }
                            else
                                movement.SetVelocity(Mathf.Abs(movement._rb.velocity.x) * transform.forward + transform.up * activeMove.m[i].momentum.y);
                        }
                        else
                        {
                            if (activeMove.m[i].useCurve)
                            {
                                float curvetime = ((float)frame - activeMove.m[i].startFrame) / (activeMove.m[i].startFrame + activeMove.m[i].duration);
                                float deltaCurve =
                                    activeMove.m[i].movementCurve.Evaluate((float)1 + frame - activeMove.m[i].startFrame) / (activeMove.m[i].startFrame + activeMove.m[i].duration) -
                                    activeMove.m[i].movementCurve.Evaluate((float)frame - activeMove.m[i].startFrame) / (activeMove.m[i].startFrame + activeMove.m[i].duration);
                                //Debug.Log(deltaCurve + " " + activeMove.m[i].movementCurve.Evaluate((float)1 + frame - activeMove.m[i].startFrame) / (activeMove.m[i].startFrame + activeMove.m[i].duration) + " " + activeMove.m[i].movementCurve.Evaluate((float)frame - activeMove.m[i].startFrame) / (activeMove.m[i].startFrame + activeMove.m[i].duration));
                                movement.SetVelocity(activeMove.m[i].xMovementCurve.Evaluate(curvetime) * transform.forward + transform.up * (activeMove.m[i].movementCurve.Evaluate(curvetime)));
                                if (activeMove.m[i].movementCurve.Evaluate(curvetime) < 0 && activeMove.noClip == false)
                                    status.EnableCollider();
                            }
                            else
                                movement.SetVelocity((activeMove.m[i].momentum.x * transform.forward + transform.up * activeMove.m[i].momentum.y));
                        }

                    }
                }
            }

        }
        if (movement.CheckEdge() && activeMove.stopAtEdges)
        {
            movement.SetVelocity(new Vector3(0, movement._rb.velocity.y));
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
                    if (frame == item.startup && item.type == ScreenShakeType.OnStartup)
                    {
                        GameObject fx = Instantiate(item.prefab, transform.position, transform.rotation, vfxContainer);

                        fx.transform.localPosition = item.position;
                        fx.transform.localRotation = Quaternion.Euler(item.rotation);
                        fx.transform.localRotation = Quaternion.Euler(fx.transform.localRotation.eulerAngles.x, fx.transform.localRotation.eulerAngles.y, Mathf.Sign(transform.rotation.y) * fx.transform.localRotation.eulerAngles.z);
                        fx.transform.localScale = item.scale;
                        fx.transform.localScale = new Vector3(Mathf.Sign(transform.rotation.y) * Mathf.Abs(fx.transform.localScale.x), fx.transform.localScale.y, fx.transform.localScale.z);

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
                        AudioManager.Instance.PlaySFX(item, transform.position);
                    }
                }
        }
    }
    void ApplyScreenShake(int frame = 0)
    {
        for (int i = 0; i < activeMove.screenShake.Length; i++)
        {
            if (frame == activeMove.screenShake[i].startup && activeMove.screenShake[i].type == ScreenShakeType.OnStartup)
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
    public void ActiveFrames(int frame)
    {
        for (int i = 0; i < activeMove.attacks.Length; i++)
        {
            if (frame < activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames && frame >= activeMove.attacks[i].startupFrame)
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

                        if (hitbox != null) hitbox.SetupHitbox(i, this, status, activeMove);

                        if (activeMove.attacks[i].attackType == AttackType.Projectile)
                        {
                            projectiles.Add(hitboxes[i]);
                            hitboxes[i] = null;
                        }
                    }
                }
            }
            else if (frame > activeMove.attacks[i].startupFrame + activeMove.attacks[i].activeFrames)
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
        //Hide Graphics
        if (activeMove.hideGraphics)
        {
            if (frame == activeMove.hideGraphicsStart)
            {
                foreach (var item in graphics)
                {
                    Debug.Log("Hide");
                    item.gameObject.SetActive(false);
                }
            }
            else if (frame >= activeMove.hideGraphicsStart + activeMove.hideGraphicsDuration)
            {
                foreach (var item in graphics)
                {
                    item.gameObject.SetActive(true);
                }
            }
        }
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

        combo++;

        if (move.resetGatling)
        {
            combo = 0;
            usedMoves.Clear();
        }

        if (move.type == MoveType.Movement)
        {
            movementFrames = GameManager.Instance.gameFrameCount;
        }

        if (status.Meter <= move.meterCost)
        {
            delayRelease = true;
        }

        if (!move.consumeMeterOnActiveFrame)
            status.Meter -= move.meterCost;

        recoverOnlyOnLand = move.recoverOnlyOnLand;
        activeMove = move;
        isDashing = move.isDashing;
        attackID = move.animationID;
        status.animationArmor = move.armor;
        attackString = false;
        canTargetCombo = false;
        hit = false;

        holdAttack = move.holdAttack;
        attackFrames = 0;

        if (move.instantStartupRotation) movement.Rotation();

        //Run momentum
        if (move.overrideVelocity) movement._rb.velocity = Vector3.zero;
        else if (move.runMomentum) movement._rb.velocity = movement._rb.velocity * 0.5F;

        //Air properties
        if (move.useAirAction) airActionCounter++;

        if (movement.passthroughPlatforms)
            status.EnableCollider();

        if (move.homing)
            homingDirection = movement.direction.normalized;

        Startup();
        landCancel = move.landCancel;

        attackPerformedEvent?.Invoke(move);


        startupEvent?.Invoke();

        attacking = true;
        newAttack = true;
        ExecuteFrame();
    }
    public bool IsHoldAttack()
    {
        if (activeMove == null)
            return false;

        return activeMove.holdAttack;
    }
    public bool ReleaseButton()
    {
        if (activeMove == null) return false;
        if (activeMove.releaseAttackMove == null || AttackFrame >= activeMove.firstStartupFrame)
        {
            return false;
        }

        if (AttackFrame < releaseMinimum)
        {
            delayRelease = true;
            return true;
        }
        delayRelease = false;
        Move move = activeMove.releaseAttackMove;

        usedMoves.Add(move);
        ClearHitboxes();

        if (move.resetGatling)
        {
            combo = 0;
            usedMoves.Clear();
        }

        if (move.type == MoveType.Movement)
        {
            movementFrames = GameManager.Instance.gameFrameCount;
        }

        if (!move.consumeMeterOnActiveFrame)
            status.Meter -= move.meterCost;

        if (AttackFrame > move.firstStartupFrame)
            attackFrames = move.firstStartupFrame - 1;

        recoverOnlyOnLand = move.recoverOnlyOnLand;
        activeMove = move;
        attackID = move.animationID;

        attackString = false;
        canTargetCombo = false;
        hit = false;

        holdAttack = move.holdAttack;

        if (move.instantStartupRotation) movement.Rotation();

        //Run momentum
        if (move.overrideVelocity) movement._rb.velocity = Vector3.zero;
        else if (move.runMomentum) movement._rb.velocity = movement._rb.velocity * 0.5F;

        //Air properties
        if (move.useAirAction) airActionCounter++;

        if (!movement.passthroughPlatforms)
            status.EnableCollider();

        Startup();
        landCancel = move.landCancel;

        attackPerformedEvent?.Invoke(move);
        blendAttackEvent?.Invoke();

        attacking = true;
        ExecuteFrame();
        return true;
    }
    public bool CanUseMove(Move move, bool isCombo = false)
    {
        if (move == null) return false;

        if (move.releaseAttackMove != null)
        {
            if (status.currentStats.currentMeter < move.releaseAttackMove.meterCost) return false;
        }
        else
        if (status.currentStats.currentMeter < move.meterCost) return false;
        //if (jumpFrameCounter > 0) return false;
        if (move.useAirAction)
        {
            if (airActionCounter < status.currentStats.airActions)
            {
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
            if (isCombo) return true;
            if (CanTargetCombo(move)) return true;
        }
        return false;
    }
    public bool CanTargetCombo(Move move)
    {
        if (attacking && canTargetCombo)
        {
            if (activeMove.targetComboMoves.Count > 0)
            {
                if (activeMove.targetComboMoves.Contains(move) && !usedMoves.Contains(move))
                {
                    return true;
                }

                //if (usedMoves.Contains(move) && activeMove == move || move.targetComboMoves.Contains(activeMove))
                //{
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

        if (c == activeCombo)
        {
            if (c.moves.Count <= combo)
                return false;
            if (!CanUseMove(c.moves[combo], true))
            {
                //Debug.Log("Can't use move");
                return false;
            }
        }
        else
        {
            if (!CanUseMove(c.moves[0]))
            {
                //Debug.Log("Can't use move");
                return false;
            }
            if (activeCombo != null)
            {
                if (!activeCombo.moves[0].keepComboCount)
                    combo = 0;
                else
                {
                    if (c.moves.Count <= combo)
                        return false;
                }

            }

        }

        activeCombo = c;
        AttackProperties(c.moves[combo]);
        return true;
    }
    public bool Attack(Move move)
    {
        if (move == null) return false;
        if (CanTargetCombo(move))
        {
            AttackProperties(move);
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
    void Land()
    {
        if (recoverOnlyOnLand)
        {
            attackFrames = activeMove.lastActiveFrame + 1;

        }
        airActionCounter = 0;

        recoverOnlyOnLand = false;
        if (activeMove != null)
        {
            for (int i = 0; i < activeMove.screenShake.Length; i++)
            {
                if (activeMove.screenShake[i].type == ScreenShakeType.OnLand)
                    CameraManager.Instance.ShakeCamera(activeMove.screenShake[i].amplitude, activeMove.screenShake[i].duration);
            }
            LandVFX();


        }

        if (landCancel)
        {
            newAttack = false;
            landCancelFrame = true;
            Idle();
        }
    }
    void LandVFX()
    {
        if (activeMove.vfx.Length > 0)
            foreach (var item in activeMove.vfx)
            {
                if (item.type == ScreenShakeType.OnLand)
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
    }
    void ResetCombo()
    {
        combo = 0;
    }
    void HitstunEvent()
    {
        ResetAllValues();
    }
    void DeathEvent()
    {

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
        foreach (var item in graphics)
        {
            item.gameObject.SetActive(true);
        }
        newAttack = false;
        attackString = false;

        delayRelease = false;
        extendedBuffer = 0;
        combo = 0;
        recoverOnlyOnLand = false;
        jumpFrameCounter = 0;
        attacking = false;
        isDashing = false;
        canTargetCombo = false;
        landCancel = false;
        hit = false;
        status.counterhitState = false;
        status.projectileInvul = false;
        status.EnableCollider();
        status.invincible = false;
        movement.forcedWalk = false;
        status.animationArmor = false;
        activeCombo = null;
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
    #region powerup stuff
    public void HitEvent(HitInfo hitInfo)
    {
        hitEvent?.Invoke(hitInfo);
    }
    #endregion
}

public class HitInfo
{
    public HitInfo(bool c = false, bool b = false, Status atkStatus = null, Status s = null, Move m = null)
    {
        crit = c;
        backstab = b;
        attackerStatus = atkStatus;
        enemyStatus = s;
        move = m;
    }
    public bool crit = false;
    public bool backstab = false;
    public Status attackerStatus = null;
    public Status enemyStatus = null;
    public Move move = null;
}