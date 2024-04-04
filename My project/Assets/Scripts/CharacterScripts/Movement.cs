using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Variables
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Status status;

    [Header("Movement attributes")]
    [TabGroup("Movement"), ReadOnly] public Vector3 direction;
    //[TabGroup("Movement"), ReadOnly] public Vector3 airDirection;
    [TabGroup("Movement"), ReadOnly] public Vector3 vel;
    [TabGroup("Movement")] public bool isFlying = false;
    [TabGroup("Movement")] public bool isMoving = false;
    [TabGroup("Movement")] public bool forcedWalk;
    [TabGroup("Movement")] public float walkSpeed = 3;
    [TabGroup("Movement")] public float runSpeed = 8;
    [HideInInspector] public bool run;

    [TabGroup("Movement")] public float currentVel;
    [TabGroup("Movement"), ReadOnly] public float actualVelocity;
    [TabGroup("Movement")] public float smoothAcceleration = 0.5f;
    [TabGroup("Movement")] public float smoothDeacceleration = 0.5f;

    [TabGroup("Ground Detection")] public bool useWallDetection;
    [TabGroup("Ground Detection")] public bool ground;
    [TabGroup("Ground Detection")] public bool wall;
    [Header("Ground")]
    [TabGroup("Ground Detection")] public float groundRayLength = 0.2f;
    [TabGroup("Ground Detection")] public float frontGroundRayLength = 0.3f;
    [TabGroup("Ground Detection")] public float preLandRayLength = 0.3f;
    [TabGroup("Ground Detection")] public float groundCheckHeightOffset = 0.15F;
    [TabGroup("Ground Detection")] public float groundFrontOffset = 0.15F;
    [TabGroup("Ground Detection")] public float checkEdgeOffset = 0.15F;
    [TabGroup("Ground Detection")] public float groundBackOffset = 0.15F;
    [TabGroup("Ground Detection")] public LayerMask groundMask;
    [TabGroup("Ground Detection")] public LayerMask enemyMask;
    [TabGroup("Ground Detection")] public float enemyPush;

    [Header("Wall")]
    [TabGroup("Ground Detection")] public LayerMask wallMask;

    [TabGroup("Ground Detection")] public float wallSideOffset;
    RaycastHit hit, hit2;


    [TabGroup("Ground Detection")] public bool check;
    [TabGroup("Ground Detection")] public bool check2;
    [TabGroup("Ground Detection")] public bool checkEdge;
    [TabGroup("Ground Detection")] public float stepHeight;
    [TabGroup("Ground Detection")] public float stepAngle;
    [TabGroup("Ground Detection")] public float stepRayLength;
    [HeaderAttribute("Jump attributes")]
    [TabGroup("Jump")] public float jumpHeight;
    [TabGroup("Jump")] public float airSpeed;
    [TabGroup("Jump")] public float fallMultiplier;
    [TabGroup("Jump")] public int minimumJumpTime = 2;
    [TabGroup("Jump")] int jumpCounter;
    [TabGroup("Jump")] public int multiJumps;
    [TabGroup("Jump")] public int performedJumps;
    [TabGroup("Jump")] public float airDeceleration = 0.95F;
    [TabGroup("Jump")] public float airBrake = 0.8F;
    [TabGroup("Jump")] public bool passthroughPlatforms = false;
    [TabGroup("Jump")] public int passthroughDuration = 0;
    [TabGroup("Jump")] public int passthroughCounter = 0;

    public event Action jumpEvent;
    public event Action doubleJumpEvent;
    public event Action landEvent;
    public event Action preLandEvent;

    [HideInInspector] public float zeroFloat;

    [FoldoutGroup("Assign components")] public PhysicMaterial groundMat;
    [FoldoutGroup("Assign components")] public PhysicMaterial airMat;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        status = GetComponent<Status>();
        status.neutralEvent += Neutral;
        status.statEvent += InitializeMovement;
        status.deathEvent += DeathEvent;


        InitializeMovement();
    }
    void InitializeMovement()
    {
        multiJumps = status.currentStats.jumps;
    }
    void Neutral()
    {
        if (ground)
            status.col.material = groundMat;
    }

    private void OnDestroy()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
        status.neutralEvent -= Neutral;
        status.statEvent -= InitializeMovement;
        status.deathEvent -= DeathEvent;
    }
    void DeathEvent()
    {

    }

    void ExecuteFrame()
    {
        vel = _rb.velocity;
        if (GameManager.isPaused || status.isDead)
        {
            isMoving = false;
            return;
        }

        GroundDetection();
        if (ground)
        {
            status.GroundCollider();
            status.groundState = GroundState.Grounded;
        }
        else
        {
            status.AirCollider();
            status.groundState = GroundState.Airborne;
        }

        if (status.currentState == Status.State.Neutral)
        {

            ExecuteMovement();

        }
        if (_rb.velocity.y < 0) SetVelocity(_rb.velocity + Physics.gravity * fallMultiplier);
    }

    public void SetVelocityGround(Vector3 v)
    {
        if (ground && v.y == 0)
        {
            Vector3 groundDirection = hit.collider ? Vector3.Cross(new Vector3(0, 0, -v.x), hit.normal) : Vector3.Cross(new Vector3(0, 0, -v.x), hit2.normal);
            _rb.velocity = new Vector3(groundDirection.x, groundDirection.y, 0);
        }
        else _rb.velocity = new Vector3(v.x, v.y, 0);
    }

    public void SetVelocity(Vector3 v)
    {
        _rb.velocity = new Vector3(v.x, v.y, 0);
    }

    public void ResetVelocity()
    {
        actualVelocity = 0;
        currentVel = 0;
    }

    void CalculateVelocity()
    {
        if (currentVel > actualVelocity)
            actualVelocity = Mathf.SmoothDamp(actualVelocity, currentVel, ref zeroFloat, smoothAcceleration);
        else if (currentVel < actualVelocity)
            actualVelocity = Mathf.SmoothDamp(actualVelocity, currentVel, ref zeroFloat, smoothDeacceleration);
    }

    public void ExecuteMovement()
    {
        MovementProperties();
        Rotation();
        PlayerMovement();
    }

    public void AttackMovement()
    {
        if (!isMoving)
        {
            currentVel = 0;
        }
        else
        {
            if (ground)
                currentVel = runSpeed * status.currentStats.movementSpeedModifier / 2;
            else
                currentVel = airSpeed * status.currentStats.movementSpeedModifier / 2;
        }
        CalculateVelocity();
        Rotation();
        PlayerMovement();
    }
    #region Assistance functions

    void DisableMovement()
    {
        SetVelocity(Vector3.zero);
        direction = Vector3.zero;
        _rb.isKinematic = true;
        return;

    }
    #endregion
    public virtual void Rotation()
    {
        if (isMoving)
        {
            if (direction != Vector3.zero)
            {

                Vector3 tempDir = new Vector3(direction.x, 0, direction.z);
                if (direction.x == 0)
                {
                    tempDir.x = Mathf.Sign(transform.localRotation.y);
                }
                Quaternion desiredRotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, tempDir, Vector3.up), 0);
                transform.rotation = desiredRotation;
            }
        }
    }

    public virtual void MovementProperties()
    {
        if (!isMoving)
        {
            currentVel = 0;
        }
        else
        {
            if (ground)
                currentVel = runSpeed * status.currentStats.movementSpeedModifier;
            else
                currentVel = airSpeed * status.currentStats.movementSpeedModifier;
        }
        CalculateVelocity();
    }

    public void Jump()
    {
        jumpEvent?.Invoke();

        jumpCounter = minimumJumpTime;
        status.col.material = airMat;
        ground = false;
        CollisionPassthrough();
        status.groundState = GroundState.Airborne;
        if (isMoving)
        {
            if (currentVel == 0)
                SetVelocity(new Vector3(transform.forward.x * walkSpeed, _rb.velocity.y, 0));
            else SetVelocity(new Vector3(transform.forward.x * currentVel, _rb.velocity.y, 0));
        }


        _rb.velocity = new Vector3(_rb.velocity.x, jumpHeight, 0);
    }
    public void DoubleJump()
    {
        if (performedJumps >= multiJumps) return;
        performedJumps++;
        CollisionPassthrough();
        Rotation();
        jumpCounter = minimumJumpTime;
        status.col.material = airMat;
        ground = false;
        doubleJumpEvent?.Invoke();

        if (isMoving)
        {
            if (currentVel == 0)
                _rb.velocity = new Vector3(Mathf.Sign(direction.x) * walkSpeed, _rb.velocity.y, 0);
            else
                _rb.velocity = new Vector3(Mathf.Sign(direction.x) * currentVel, _rb.velocity.y, 0);
        }

        _rb.velocity = new Vector3(_rb.velocity.x, jumpHeight, _rb.velocity.z);
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.1F - transform.forward * groundBackOffset, Vector3.down * groundRayLength, Color.blue);
        Debug.DrawRay(hit.point, hit.normal * 2f, Color.gray);
        Debug.DrawRay(transform.position + Vector3.up * 0.1F + transform.forward * groundFrontOffset, Vector3.down * frontGroundRayLength, Color.blue);
        Debug.DrawRay(hit2.point, hit2.normal * 2f, Color.gray);

        Vector3 temp = direction.normalized;
        if (check)
            Debug.DrawRay(transform.position + Vector3.up * 0.1F, Vector3.Cross(new Vector3(temp.z, 0, -temp.x), hit2.normal) * 3, Color.red);
        if (check2)
            Debug.DrawRay(transform.position + Vector3.up * 0.1F, Vector3.Cross(new Vector3(temp.z, 0, -temp.x), hit.normal) * 3, Color.red);

        //Wall Detection Ray
        Debug.DrawRay(transform.position + Vector3.up * stepHeight + transform.right * wallSideOffset, transform.forward * stepRayLength, Color.magenta);
        Debug.DrawRay(transform.position + Vector3.up * stepHeight - transform.right * wallSideOffset, transform.forward * stepRayLength, Color.magenta);

    }

    public void WallDetection()
    {
        RaycastHit wallHit1;
        RaycastHit wallHit2;
        RaycastHit wallHit3;
        bool foundWall = false;
        Collider wallCollider = null;
        if (Physics.Raycast(transform.position + Vector3.up * stepHeight, transform.forward, out wallHit1, stepRayLength, wallMask))
        {
            wallCollider = wallHit1.collider;
            foundWall = true;
        }
        else
        if (Physics.Raycast(transform.position + Vector3.up * stepHeight + transform.right * wallSideOffset, transform.forward, out wallHit2, stepRayLength, wallMask))
        {
            wallCollider = wallHit2.collider;
            foundWall = true;
        }
        else
        if (Physics.Raycast(transform.position + Vector3.up * stepHeight - transform.right * wallSideOffset, transform.forward, out wallHit3, stepRayLength, wallMask))
        {
            wallCollider = wallHit3.collider;
            foundWall = true;
        }

        wall = foundWall;
        if (foundWall)
        {
            Vector3 closestPoint = wallCollider.ClosestPoint(transform.position + Vector3.up + transform.forward * 0.25F);
            Vector3 dir = RemoveFallVelocity(closestPoint - (transform.position + Vector3.up));
            Debug.DrawRay(transform.position + stepHeight * Vector3.up, -dir * 10, Color.blue);
            Debug.DrawRay(transform.position + stepHeight * Vector3.up, _rb.velocity, Color.cyan);

            Vector3 temp = Vector3.zero;
            //if (ground) temp = RemoveYAxis(RemoveFallVelocity(_rb.velocity), dir);
            //else temp = RemoveYAxis(_rb.velocity, dir);

            //rb.velocity = temp;

            Debug.DrawRay(transform.position + stepHeight * Vector3.up, _rb.velocity / 2, Color.green);
        }
    }
    public bool CheckEdge()
    {
        checkEdge = !Physics.Raycast(transform.position + Vector3.up * 0.1F + transform.forward * checkEdgeOffset, Vector3.down, out hit2, frontGroundRayLength, groundMask);
        if (ground)
            return checkEdge;
        else
            return false;
    }

    public bool GroundDetection()
    {
        check = Physics.Raycast(transform.position + Vector3.up * groundCheckHeightOffset - transform.forward * groundBackOffset, Vector3.down, out hit, groundRayLength, groundMask);
        check2 = Physics.Raycast(transform.position + Vector3.up * groundCheckHeightOffset + transform.forward * groundFrontOffset, Vector3.down, out hit2, frontGroundRayLength, groundMask);

        RaycastHit enemyHit;
        if (Physics.Raycast(transform.position + Vector3.up * groundCheckHeightOffset + transform.forward * groundFrontOffset, Vector3.down, out enemyHit, frontGroundRayLength * 2, enemyMask) || Physics.Raycast(transform.position + Vector3.up * groundCheckHeightOffset - transform.forward * groundBackOffset, Vector3.down, out enemyHit, frontGroundRayLength * 2, enemyMask))
        {
            if (_rb.velocity.y <= 0)
            {
                Vector3 tempDir = enemyHit.collider.attachedRigidbody.transform.position - transform.position;
                tempDir.y = 0;
                tempDir.z = 0;

                //If target is to the right of player
                if (tempDir.x > 0)
                {
                    transform.position += (-tempDir.normalized * enemyPush);
                    enemyHit.collider.attachedRigidbody.transform.position += (tempDir.normalized * enemyPush);
                }
                //If target is to the left of player
                else if (tempDir.x < 0)
                {
                    transform.position += (-tempDir.normalized * enemyPush);
                    enemyHit.collider.attachedRigidbody.transform.position += (tempDir.normalized * enemyPush);
                }
                // enemyHit.collider.attachedRigidbody.transform.Translate(transform.right * enemyPush);
            }

        }
        if (jumpCounter > 0)
        {
            jumpCounter--;
            return false;
        }

        if (check)
        {
            float angle = Vector3.Angle(hit2.normal, Vector3.up);
            if (angle > stepAngle) check = false;
        }
        if (check2)
        {
            float angle2 = Vector3.Angle(hit2.normal, Vector3.up);
            if (angle2 > stepAngle) check2 = false;
        }

        if (passthroughCounter > 0)
            passthroughCounter--;

        if (!ground && status.NonAttackState())
        {
            if (_rb.velocity.y <= 0.01f && passthroughCounter <= 0)
            {
                passthroughPlatforms = false;
                status.EnableCollider();
            }
            else
            {
                CollisionPassthrough();
            }
        }
        else if (status.NonAttackState() && passthroughCounter <= 0)
        {
            if (status.NonAttackState())
            {
                passthroughPlatforms = false;
                status.EnableCollider();
            }
        }



        //Check collission for landing
        if (!ground)
        {
            if (_rb.velocity.y <= 1f)
            {
                if (check || check2)
                {
                    Landing();
                }
                bool check3 = Physics.Raycast(transform.position + Vector3.up * 0.1F, Vector3.down, out hit, preLandRayLength, groundMask);

                if (check3)
                {
                    preLandEvent?.Invoke();
                }
            }
        }
        //Check collission for while on ground
        else
        {
            if (!check && !check2)
            {
                ground = false;
            }
        }

        if (isFlying)
            _rb.useGravity = !ground;

        if (ground) status.col.material = groundMat;
        else status.col.material = airMat;
        return ground;
    }

    public void CollisionPassthrough()
    {
        if (status.col.gameObject.layer != LayerMask.NameToLayer("Noclip"))
        {
            status.col.gameObject.layer = LayerMask.NameToLayer("CollisionPassthrough");
            status.airCol.gameObject.layer = LayerMask.NameToLayer("CollisionPassthrough");
            passthroughPlatforms = true;
        }
    }

    public bool OnPlatform()
    {
        if (hit.collider)
        { return ground && !Physics.Raycast(transform.position + Vector3.up * groundCheckHeightOffset - transform.forward * groundBackOffset, Vector3.down, groundRayLength, LayerMask.NameToLayer("Platforms")); }
        else
        {
            return ground && !Physics.Raycast(transform.position + Vector3.up * groundCheckHeightOffset + transform.forward * groundFrontOffset, Vector3.down, groundRayLength, LayerMask.NameToLayer("Platforms"));
        }

        //return ground && hit.collider ?
        //    !Physics.Raycast(transform.position + Vector3.up * groundCheckHeightOffset - transform.forward * groundBackOffset, Vector3.down, groundRayLength, LayerMask.NameToLayer("Platforms")) :
        //    !Physics.Raycast(transform.position + Vector3.up * groundCheckHeightOffset + transform.forward * groundFrontOffset, Vector3.down, groundRayLength, LayerMask.NameToLayer("Platforms"));
    }
    public void FallThroughPlatforms()
    {
        SetVelocity(new Vector3(_rb.velocity.x, -2, 0));
        passthroughPlatforms = true;
        passthroughCounter = passthroughDuration;
        status.col.gameObject.layer = LayerMask.NameToLayer("CollisionPassthrough");
        status.airCol.gameObject.layer = LayerMask.NameToLayer("CollisionPassthrough");
    }

    void Landing()
    {
        landEvent?.Invoke();
        performedJumps = 0;
        ground = true;
    }

    public void PlayerMovement()
    {
        Vector3 temp;

        if (isFlying)
        {
            SetVelocity((transform.forward * Mathf.Abs(direction.x) + transform.up * direction.y) * actualVelocity);
        }
        else
        {
            if (!ground)
            //Airborne
            {
                temp = _rb.velocity;
                temp.y = 0;
                //Fix
                if (!isMoving)
                {
                    //NEEDS TO FLY CORRECT WAY
                    //if (status.alignment == Alignment.Enemy)
                    //    Debug.Log(temp);
                    SetVelocity(temp * airDeceleration + _rb.velocity.y * Vector3.up);
                }
                else
                    SetVelocity(transform.forward * actualVelocity * Mathf.Abs(direction.x) + _rb.velocity.y * Vector3.up);
            }
            else
            //Normal Walking
            {
                temp = _rb.velocity;
                temp.y = 0;
                if (!isMoving)
                {
                    //if (status.alignment == Alignment.Enemy)
                    //    Debug.Log(temp);
                    SetVelocity(new Vector3(Mathf.Sign(temp.x) * actualVelocity, _rb.velocity.y, 0));

                }
                else
                {
                    //_rb.velocity = hit.collider ? Vector3.Cross(new Vector3(temp.z, 0, -temp.x), hit.normal) * actualVelocity

                    Vector3 groundDirection = hit.collider ? _rb.velocity = Vector3.Cross(new Vector3(0, 0, -direction.x), hit.normal) * actualVelocity : _rb.velocity = Vector3.Cross(new Vector3(0, 0, -direction.x), hit2.normal) * actualVelocity;
                    SetVelocity(groundDirection);
                    //SetVelocity(new Vector3(transform.forward.x * actualVelocity, _rb.velocity.y, 0));
                }
            }
        }
    }



    #region Math

    public Vector3 RelativeToForward()
    {
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        Vector3 temp = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        if (!isMoving) temp = Vector3.zero;
        return temp;
    }

    public Vector3 RemoveAxis(Vector3 vec, Vector3 removedAxis)
    {
        Vector3 n = removedAxis;
        Vector3 dir = vec;

        float d = Vector3.Dot(dir, n);


        return n * d;
    }
    Vector3 RemoveFallVelocity(Vector3 vec)
    {
        Vector3 n = Vector3.down;

        Vector3 dir = vec;
        float d = Vector3.Dot(dir, n);

        if (d > 0f)
        {
            dir -= n * d;

        }
        return dir;
    }
    public Vector3 RemoveYAxis(Vector3 vec, Vector3 removedAxis)
    {
        Vector3 n = removedAxis.normalized;

        Vector3 dir = vec;

        float d = Vector3.Dot(dir, n);
        if (d > 0f)
            dir -= n * d;
        return dir;
    }
    #endregion
}
