using System.Collections;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class CharacterAnimator : MonoBehaviour
{
    private Status status;
    private Animator anim;
    private Movement movement;
    private AttackScript attack;
    [TabGroup("Debug")] public int frame;
    [TabGroup("Debug")] public bool hitstop;
    private float runSpeed;
    float x, y;
    float zeroFloat = 0f;
    [SerializeField]
    [TabGroup("Settings")] float maxSpeed;
    [SerializeField]
    [TabGroup("Settings")] private float deaccelerateSpeed;
    float tempDirection = 0F;
    [TabGroup("Settings")] public float fallThreshold;

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponentInParent<Status>();
        anim = GetComponent<Animator>();
        movement = GetComponentInParent<Movement>();
        attack = GetComponentInParent<AttackScript>();
        if (status != null)
        {
            //character = status.character;
            status.hitstunEvent += HitStun;
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
            attack.emptyAttackEvent += EmptyAttack;
            attack.recoveryEvent += AttackRecovery;
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;

        if (status != null)
        {
            status.hitstunEvent += HitStun;
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

    void ExecuteFrame()
    {
        anim.enabled = true;
        frame = Mathf.RoundToInt(anim.GetCurrentAnimatorStateInfo(0).normalizedTime * anim.GetCurrentAnimatorStateInfo(0).length / (1f / 60f));

        if (movement != null)
            MovementAnimation();
        if (hitstop)
            anim.enabled = false;

        if (!GameManager.Instance.runNormally) StartCoroutine(PauseAnimation());
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
    public void UnequipAnimation()
    {
        anim.SetInteger("ID", 101);
        anim.SetTrigger("Top");
        anim.SetBool("Attacking", true);
    }

    void StatusAnimation()
    {
        anim.SetBool("Dead", status.isDead);
        anim.SetBool("Hitstun", status.inHitStun);
        anim.SetBool("Knockdown", status.currentState == Status.State.Knockdown);

        anim.SetFloat("AttackSpeed", status.currentStats.attackSpeed);
        anim.SetFloat("MovementSpeed", status.currentStats.movementSpeedModifier);
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

        anim.SetFloat("Horizontal", x);
        anim.SetFloat("Vertical", y);

        if (movement._rb.velocity.y < fallThreshold)
            anim.SetInteger("Falling", -1);
        else anim.SetInteger("Falling", 1);
    }

    private void RunSpeed()
    {
        if (!movement.isMoving) runSpeed = Mathf.Lerp(runSpeed, 0, deaccelerateSpeed);
        else  runSpeed = Mathf.Lerp(runSpeed, 0.6F, deaccelerateSpeed);
        //else if (movement.isMoving) runSpeed = Mathf.Lerp(runSpeed, 0.25F, deaccelerateSpeed);


        anim.SetFloat("RunSpeed", Mathf.Abs(runSpeed));
    }

    void Release() { anim.SetTrigger("Release"); }

    void StartAttack()
    {

        anim.SetTrigger("Attack");
        anim.SetBool("Attacking", true);
        anim.SetInteger("ID", attack.attackID);

    }
    void StartTopAnimation()
    {
        anim.SetTrigger("Top");
        anim.SetBool("Attacking", true);
        anim.SetInteger("ID", attack.attackID);

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
