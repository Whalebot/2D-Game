﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class CharacterAnimator : MonoBehaviour
{
    private Status status;
    private Animator anim;
    private Movement movement;
    private AttackScript attack;
    private AI ai;
    [TabGroup("Debug")] public int frame;
    [TabGroup("Debug")] public float normalizedTime;
    [TabGroup("Debug")] public bool hitstop;
    [TabGroup("Debug")] public bool blend;

    private float runSpeed;
    float x, y;
    float zeroFloat = 0f;
    [SerializeField]
    [TabGroup("Settings")] float maxSpeed;
    [SerializeField]
    [TabGroup("Settings")] private float deaccelerateSpeed;
    float tempDirection = 0F;
    [TabGroup("Settings")] public float fallThreshold;

    public List<Transform> renderers;

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponentInParent<Status>();
        anim = GetComponent<Animator>();
        movement = GetComponentInParent<Movement>();
        attack = GetComponentInParent<AttackScript>();
        ai = GetComponentInParent<AI>();

        if (status != null)
        {
            //character = status.character;
            status.hitstunEvent += HitStun;
            status.blockstunEvent += Block;
        }

        if (movement != null)
        {
            movement.jumpEvent += Jump;
            movement.preLandEvent += PreLand;
            movement.doubleJumpEvent += DoubleJump;

        }


        GameManager.Instance.advanceGameState += ExecuteFrame;

        if (attack != null)
        {
            attack.startupEvent += StartAttack;
            attack.blendAttackEvent += BlendAttack;
            attack.emptyAttackEvent += EmptyAttack;
            attack.recoveryEvent += AttackRecovery;
        }
        ExecuteFrame();
    }

    private void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;

        if (status != null)
        {
            status.hitstunEvent -= HitStun;
            status.blockstunEvent -= Block;
        }
        if (attack != null)
        {
            attack.startupEvent -= StartAttack;
            attack.recoveryEvent -= AttackRecovery;
        }
        if (movement != null)
        {
            movement.jumpEvent -= Jump;
            movement.preLandEvent -= PreLand;
            movement.doubleJumpEvent -= DoubleJump;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.isPaused)
        {
            anim.enabled = false;
        }
    }
    void ExecuteFrame()
    {
        anim.enabled = true;
        normalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        frame = Mathf.RoundToInt(anim.GetCurrentAnimatorStateInfo(0).normalizedTime * anim.GetCurrentAnimatorStateInfo(0).length / (1f / 60f) / anim.GetCurrentAnimatorStateInfo(0).speed);

        if (movement.isMoving && !attack.attacking)
        {
            if (movement.direction != Vector3.zero)
            {
                if (GameManager.Instance.flipGraphics)
                {
                    if (GameManager.Instance.flipGraphics)
                    {
                        foreach (var item in renderers)
                        {
                            item.localScale = new Vector3(Mathf.Sign(movement.direction.x) * Mathf.Abs(item.localScale.x), item.localScale.y, item.localScale.z);
                            //float angle = 15f;
                            float angle = item.localEulerAngles.y;

                            if (angle > 180)
                                angle = angle - 360;
                            angle = (angle > 180) ? angle - 360 : angle;
                            //Debug.Log(angle);
                            item.localRotation = Quaternion.Euler(0, Mathf.Sign(transform.rotation.y) * Mathf.Abs(angle), 0);
                        }
                    }
                }

            }
        }
        else
        {
            if (GameManager.Instance.flipGraphics)
            {
                foreach (var item in renderers)
                {
                    item.localScale = new Vector3(Mathf.Sign(status.transform.rotation.y) * Mathf.Abs(item.localScale.x), item.localScale.y, item.localScale.z);
                    float angle = item.localEulerAngles.y;
                    angle = (angle > 180) ? angle - 360 : angle;
                    item.localRotation = Quaternion.Euler(0, Mathf.Sign(transform.rotation.y) * Mathf.Abs(angle), 0);
                }
            }
        }

        if (blend)
        {
            if (anim.GetNextAnimatorStateInfo(0).speed > 0)
            {
                //Debug.Log($"{attack.attackFrames} {attack.attackFrames/60} {anim.GetNextAnimatorStateInfo(0).length} {((attack.attackFrames / 60))/ anim.GetNextAnimatorStateInfo(0).length}");
                anim.Play(anim.GetNextAnimatorStateInfo(0).fullPathHash, 0,
                    (((attack.attackFrames / 60)))
                    / anim.GetNextAnimatorStateInfo(0).length);

                blend = false;
            }

            //anim.SetFloat("AnimationStartTime", (float)(attack.attackFrames / 60) / anim.GetCurrentAnimatorStateInfo(0).speed * anim.GetCurrentAnimatorStateInfo(0).length);
        }

        if (movement != null)
            MovementAnimation();
        if (hitstop)
            anim.enabled = false;
        if (status != null)
            StatusAnimation();
        if (ai != null)
        {
            AIAnimation();
        }
        //if(!attack.attacking)
        //    anim.SetFloat("AnimationStartTime", 0);

        if (!GameManager.Instance.runNormally) StartCoroutine(PauseAnimation());
    }

    void AIAnimation()
    {
        anim.SetBool("Detected", ai.detected);
    }

    public void HitStop()
    {
        StartCoroutine(HitstopStart());
    }


    IEnumerator HitstopStart()
    {
        hitstop = false;
        yield return new WaitForFixedUpdate();
        // yield return new WaitForFixedUpdate();
        hitstop = true;
    }

    IEnumerator PauseAnimation()
    {
        yield return new WaitForFixedUpdate();
        //Debug.Log("DisableAnim");
        anim.enabled = false;
    }

    void EmptyAttack()
    {
        anim.SetInteger("ID", attack.attackID);
        anim.SetTrigger("Top");
        anim.SetBool("Attacking", true);


    }
    void StatusAnimation()
    {
        if (status.NonAttackState())
        {
            blend = false;
            anim.SetFloat("AnimationStartTime", 0);
        }

        anim.SetBool("Dead", status.isDead);
        anim.SetBool("Hitstun", status.inHitStun);
        anim.SetBool("Knockdown", status.currentState == Status.State.Knockdown);

        anim.SetFloat("AttackSpeed", status.currentStats.attackSpeed);
        anim.SetFloat("MovementSpeed", status.currentStats.movementSpeedModifier);
    }
    void Block()
    {
        anim.SetTrigger("Block");
    }

    void HitStun()
    {
        anim.SetFloat("HitX", status.knockbackDirection.x);
        anim.SetFloat("HitY", status.knockbackDirection.y);
        anim.SetTrigger("Hit");
    }

    void PreLand()
    {
        //anim.SetTrigger("PreLand");
    }

    void MovementAnimation()
    {
        if (movement == null) return;
        RunSpeed();

        anim.SetBool("Walking", movement.isMoving);

        x = Mathf.Lerp(x, movement.RelativeToForward().normalized.x, maxSpeed);
        y = Mathf.Lerp(y, movement.RelativeToForward().normalized.z, maxSpeed);

        anim.SetBool("Ground", movement.ground);
        anim.SetBool("Flying", movement.isFlying);

        anim.SetFloat("Horizontal", x);
        anim.SetFloat("Vertical", y);

        if (movement._rb != null)
            if (movement._rb.velocity.y < fallThreshold)
                anim.SetInteger("Falling", -1);
            else anim.SetInteger("Falling", 1);
    }

    private void RunSpeed()
    {
        if (!movement.isMoving) runSpeed = Mathf.Lerp(runSpeed, 0, deaccelerateSpeed);
        else runSpeed = Mathf.Lerp(runSpeed, 0.6F, deaccelerateSpeed);
        //else if (movement.isMoving) runSpeed = Mathf.Lerp(runSpeed, 0.25F, deaccelerateSpeed);


        anim.SetFloat("RunSpeed", Mathf.Abs(runSpeed));
    }

    void Release() { anim.SetTrigger("Release"); }

    void StartAttack()
    {
        blend = false;
        //anim.SetFloat("AnimationStartTime", 0);
        anim.SetTrigger("Attack");
        anim.SetBool("Attacking", true);
        anim.SetInteger("ID", attack.attackID);
    }
    void BlendAttack()
    {
        anim.SetTrigger("Attack");
        anim.SetBool("Attacking", true);
        anim.SetInteger("ID", attack.attackID);
        blend = true;
    }

    void AttackRecovery()
    {
        anim.SetBool("Attacking", false);
    }


    void Jump()
    {
        anim.SetTrigger("Jump");
    }

    void DoubleJump()
    {
        anim.SetTrigger("DoubleJump");
    }

    void Land()
    {
    }
    void Hit()
    {

    }

    void Step() { }
}
