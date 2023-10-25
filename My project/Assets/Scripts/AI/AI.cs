using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;

public class AI : MonoBehaviour
{
    public Character character;
    public int cooldown = 0;
    public int recoveryCooldown = 30;
    public AIAction currentAction;

    float startDistance = 0;
    float endDistance = 0;

    public int id;
    public enum State { Idle, Move, Alert, Combat, Passive, Flee };
    public State currentState = State.Idle;
    private Seeker seeker;

    public bool killOncePerDay = true;

    [Space(10)]
    [HeaderAttribute("Field of View")]
    [TabGroup("Pathfinding")] [SerializeField] public float range;
    [TabGroup("Pathfinding")] [Range(0, 360)] public float viewAngle;
    [TabGroup("Pathfinding")] [SerializeField] LayerMask mask;
    RaycastHit hit;

    [FoldoutGroup("Debug")] public Vector3 directionVector;
    int reachedCorner;
    [TabGroup("Pathfinding")] [HideInInspector] public float cornerMinDistance;
    [TabGroup("Pathfinding")] public float minDistance;
    private bool hasValidPath;
    [TabGroup("Pathfinding")] public int elapsed = 0;
    [TabGroup("Pathfinding")] public int pathUpdateTime;
    public delegate void AIEvent();
    [TabGroup("Pathfinding")] public AIEvent detectEvent;
    [TabGroup("Pathfinding")] public float detectionDelay = 0.5F;
    [TabGroup("Pathfinding")] public float detectionSpread = 5F;

    [TabGroup("Pathfinding")] public LayerMask enemyMask;
    [TabGroup("Pathfinding")] public float crowdRange = 5F;
    public bool isWalking;

    protected Movement movement;
    protected bool run;

    public float stoppingDistance = 4;
    public float enemyDetectionRadius = 8;

    [TabGroup("Patrol")] public bool willPatrol;
    [TabGroup("Patrol")] public float patrolRange;
    [TabGroup("Patrol")] public int patrolInterval;
    [TabGroup("Patrol")] public List<Vector3> patrolPoints;
    [TabGroup("Patrol")] public int patrolID;

    protected Status status;
    protected AttackScript attack;
    protected AIManager manager;

    bool detectOnce;
    public bool detected;
    protected float lastAttackTime;

    [FoldoutGroup("Debug")] public Transform target;
    [FoldoutGroup("Debug")] public Vector3 currentTarget;
    [FoldoutGroup("Debug")] public List<Move> attackQueue;

    [FoldoutGroup("Debug")] [SerializeField] public float yOffset = 0.5F;
    [FoldoutGroup("Debug")] public bool clearLine;
    [FoldoutGroup("Debug")] public bool withinAngle;
    [FoldoutGroup("Debug")] public bool inRange;

    protected void Awake()
    {
        //tree = tree.Clone();
        // context = CreateBehaviourTreeContext();
        // tree.Bind(context);



        status = GetComponent<Status>();
        attack = GetComponent<AttackScript>();

        if (character == null)
            character = status.character;

        target = GameObject.FindGameObjectWithTag("Player").transform;
        currentTarget = target.position;
        currentState = State.Idle;
    }

    protected void Start()
    {
        seeker = GetComponent<Seeker>();
        movement = GetComponent<Movement>();
        GameManager.Instance.advanceGameState += ExecuteFrame;

        status.hitstunEvent += ResetAttack;
        status.healthEvent += Detect;
        status.hitRecoveryEvent += RecoveryCooldown;
        status.deathEvent += DeathEvent;

        elapsed = pathUpdateTime;
        AIManager.Instance.allEnemies.Add(this);

        if (willPatrol)
        {
            SetupPatrolPoints();
        }
    }
    protected void OnDisable()
    {
        status.hitRecoveryEvent -= RecoveryCooldown;
        status.deathEvent -= DeathEvent;

        GameManager.Instance.advanceGameState -= ExecuteFrame;

        if (AIManager.Instance.allEnemies.Contains(this))
            AIManager.Instance.allEnemies.Remove(this);
    }

    void SetupPatrolPoints()
    {
        // if (patrolPoints.Count == 0)
        {
            patrolPoints.Clear();


            for (int i = 0; i < 5; i++)
            {
                Vector2 RNG = Random.insideUnitCircle * patrolRange;
                Vector3 temp = new Vector3(transform.position.x + RNG.x, transform.position.y, transform.position.z);
                patrolPoints.Add(temp);
            }
        }
    }

    void ResetAttack()
    {
        startDistance = 0;
        endDistance = 0;
        attackQueue.Clear();
    }

    void DeathEvent()
    {
        GameManager.Instance.Gold += status.currentStats.gold;
        RemoveFromAIManager();
    }

    void RecoveryCooldown()
    {
        cooldown = recoveryCooldown;
    }

    void RemoveFromAIManager()
    {


        AIManager.Instance.EnemyKilled(this);

        Destroy(this);
    }

    private void ExecuteFrame()
    {
        DetectionEvent();
        CalculatePath();
        if (status.NonAttackState())
            if (!InCooldown() && detected || ReachedNewDistance())
            {
                RollAction();
            }
        StateMachine();
        ResolveCooldown();

        clearLine = ClearLine();
        withinAngle = WithinAngle();
        inRange = TargetInRange();

        if (!inRange && detected)
            LoseAggro();
    }

    private void LoseAggro()
    {
        //detected = false;
        //detectOnce = false;
        //currentState = State.Idle;
        //SetupPatrolPoints();
    }

    protected bool InCooldown()
    {
        return cooldown > 0;
    }

    void ResolveCooldown()
    {
        if (cooldown > 0 && status.NonAttackState()) cooldown--;
    }

    public virtual void StateMachine()
    {
        if (movement.ground && !status.inHitStun)
            switch (currentState)
            {
                case State.Idle:
                    Idle();
                    break;
                case State.Move:
                    Approach();
                    break;
                case State.Alert:
                    Alert();
                    break;
                case State.Combat:
                    Attacking();
                    break;
                case State.Flee:
                    Flee();
                    break;
            }
    }

    protected virtual void Alert()
    {
        movement.direction = FindPath();
        currentState = State.Combat;
    }

    void RollAction()
    {
        startDistance = 0;
        endDistance = 0;

        List<AIAction> temp = new List<AIAction>();
        if (status.character == null) return;

        foreach (var item in status.character.actions)
        {
            if (Distance() < item.distance && Distance() > item.minDistance)
                temp.Add(item);
        }

        int RNG = Random.Range(0, temp.Count);
        if (temp.Count > 0)
        {
            //Debug.Log(RNG);
            currentAction = temp[RNG];
            ExecuteAction(temp[RNG]);
        }
        //else Approach();

    }

    private void ExecuteAction(AIAction aiAction)
    {
        startDistance = Distance();
        cooldown = aiAction.cooldown;
        switch (aiAction.actionType)
        {
            case AIAction.ActionType.Attack:

                for (int i = 0; i < aiAction.combo.moves.Count; i++)
                {
                    attackQueue.Add(aiAction.combo.moves[i]);
                }

                currentState = State.Combat;

                break;
            case AIAction.ActionType.Approach:
                endDistance = aiAction.targetDistance;
                currentState = State.Move;
                break;
            case AIAction.ActionType.Flee:
                endDistance = aiAction.targetDistance;
                currentState = State.Flee;
                break;
            default:
                break;
        }
    }

    void Attacking()
    {
        currentTarget = target.position;
        if (!status.NonAttackState()) { movement.direction = FindPath(); }
        else
        {
            Idle();
        }

        if (attackQueue.Count > 0)
        {
            if (attack.attackString || status.currentState == Status.State.Neutral)
            {
                movement.direction = FindPath();
                AttackQueue();
                lastAttackTime = Time.time;
            }
        }
    }

    protected void AttackQueue()
    {
        if (attack.Attack(attackQueue[0]))
        {
            attackQueue.RemoveAt(0);
        }
    }

    public void DetectionEvent()
    {
        if (!detectOnce)
        {
            if (TargetInRange() && InLineOfSight())
            {
                Detect();
            }
        }
    }

    void Detect()
    {
        if (detectOnce) return;
        currentTarget = target.position;
        detectOnce = true;
        detectEvent?.Invoke();
        if (!AIManager.Instance.activeEnemies.Contains(this))
        {
            AIManager.Instance.activeEnemies.Add(this);
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionSpread, enemyMask);
            foreach (var item in hitColliders)
            {

                AI ai = item.GetComponent<AI>();
                if (ai != null)
                    ai.Detect();

            }
        }
        StartCoroutine("DelayDetection");
    }

    IEnumerator DelayDetection()
    {
        yield return new WaitForSeconds(detectionDelay);
        detected = true;
    }

    protected void Approach()
    {
        if (target != null)
        {
            currentTarget = target.position;

            if (Vector3.Distance(transform.position, target.transform.position) > stoppingDistance)
            {
                movement.direction = FindPath();
            }
            else
            {
                if (WithinAngle())
                    Idle();
                else
                {
                    movement.direction = FindPath();
                    //movement.RotateInPlace((target.position - transform.position).normalized);
                }
            }
        }
        else
        {
            Idle();
        }
    }
    protected void Flee()
    {
        if (target != null)
        {
            currentTarget = target.position;

            movement.isMoving = true;
            movement.direction -= TargetDirectionIgnoreTilt();
        }
        else
        {
            Idle();
        }
    }
    protected void Idle()
    {

        if (willPatrol && !detected)
        {
            Patrol();
        }
        else
        {
            movement.isMoving = false;
            movement.direction = new Vector3(0, 0, 0);
        }
    }

    void Patrol()
    {
        currentTarget = patrolPoints[patrolID];
        if (Vector3.Distance(transform.position, patrolPoints[patrolID]) < 1 && !InCooldown())
        {
            cooldown = patrolInterval;
            patrolID++;
            if (patrolID >= patrolPoints.Count) patrolID = 0;
        }

        if (Vector3.Distance(transform.position, patrolPoints[patrolID]) > 1)
            movement.direction = FindPath();
    }

    #region Pathfinding
    void CalculatePath()
    {
        elapsed++;
        if (elapsed > pathUpdateTime && target != null)
        {
            currentTarget = target.position;
            reachedCorner = 0;
            elapsed -= pathUpdateTime;
        }
    }

    public Vector3 FindPath()
    {
        //CalculatePath();

        directionVector = (currentTarget - transform.position).normalized;
        //if (path.corners.Length > 1)
        //{
        //    directionVector = (path.corners[reachedCorner + 1] - path.corners[reachedCorner]).normalized;
        //    movement.isMoving = true;
        //    if (isWalking) directionVector = directionVector * 0.5F;

        //    if (Vector3.Distance(transform.position, path.corners[reachedCorner + 1]) < cornerMinDistance)
        //    {
        //        hasValidPath = false;
        //        if (path.corners.Length > reachedCorner + 2)
        //            reachedCorner++;
        //    }
        //    else
        //    {
        //        hasValidPath = true;
        //    }
        //}

        //foreach (var item in AIManager.Instance.allEnemies)
        //{
        //    if (item != this)
        //    {
        //        float f = Vector3.Distance(transform.position, item.transform.position);
        //        if (f < crowdRange && f != 0)
        //        {
        //            directionVector += (transform.position - item.transform.position).normalized * (1 - (f / crowdRange));
        //        }
        //    }
        //}

        if (Mathf.Abs(directionVector.x) > 0)
            movement.isMoving = true;
        return directionVector;
    }
    #endregion

    #region Help Functions

    public float Distance()
    {
        if (target == null) return 0;
        Vector3 v1 = transform.position;
        v1.y = 0;
        Vector3 v2 = target.transform.position;
        v2.y = 0;

        return Vector3.Distance(v1, v2);
    }
    public float DistanceIncludeHeight()
    {
        if (target == null) return 0;

        return Vector3.Distance(transform.position, target.transform.position);
    }

    bool ReachedNewDistance()
    {
        if (startDistance == 0 || endDistance == 0)
        {
            return false;
        }
        if (startDistance > endDistance)
        {
            return Distance() < endDistance;
        }
        else
            return Distance() > endDistance;
    }

    public bool TargetInRange()
    {
        return Vector3.Distance(transform.position, target.transform.position) < range;
    }

    public Quaternion TargetDirection()
    {
        Vector3 relativePos = target.transform.position + Vector3.up * AIManager.aimOffset - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        return rotation;
    }
    public Quaternion TargetDirection(Transform origin)
    {
        Vector3 relativePos = target.transform.position + Vector3.up * AIManager.aimOffset - origin.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        return rotation;
    }


    public bool WithinAngle()
    {
        bool withinAngle = (Vector3.Angle(transform.forward, TargetDirectionIgnoreTilt()) < viewAngle / 2);
        return withinAngle;
    }

    public bool CheckTargetAngle()
    {
        bool withinAngle = (Vector3.Angle(target.transform.forward, TargetDirectionIgnoreTilt()) < viewAngle / 2);
        return withinAngle;
    }

    public bool ClearLine()
    {
        bool clearLine = Physics.Raycast(transform.position + Vector3.up * yOffset, ((target.transform.position + Vector3.up) - (transform.position + Vector3.up * yOffset)).normalized, out hit, 1000, mask);

        Debug.DrawLine(transform.position + Vector3.up * yOffset, hit.point, Color.yellow);
        //if (hit.transform.IsChildOf(target))
        // Debug.Log(hit.transform.gameObject);
        if (clearLine)
            return hit.transform.IsChildOf(target);
        else
            return false;
    }

    public bool InLineOfSight()
    {
        bool seePlayer = ClearLine() && WithinAngle();
        return seePlayer;
    }

    public Vector3 TargetDirectionVector()
    {
        Vector3 relativePos = target.transform.position + Vector3.up * AIManager.aimOffset - transform.position;
        return relativePos.normalized;
    }

    public Vector3 TargetDirectionIgnoreTilt()
    {
        Vector3 relativePos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - new Vector3(transform.position.x, transform.position.y, transform.position.z);
        return relativePos.normalized;
    }

    public Vector3 TargetDirectionIgnoreTilt(Vector3 temp)
    {
        Vector3 relativePos = new Vector3(temp.x, transform.position.y, temp.z) - new Vector3(transform.position.x, transform.position.y, transform.position.z);
        return relativePos.normalized;
    }


    public Vector3 AngleToVector(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }
    public Transform GetClosestEnemy()
    {
        float minDistance = float.PositiveInfinity;
        GameObject closestEnemy = null;
        foreach (AI enemy in AIManager.Instance.allEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy.gameObject;
            }
        }
        if (AIManager.Instance.allEnemies.Count < 1)
            return null;
        else
            return closestEnemy.transform;
    }

    #endregion
}

