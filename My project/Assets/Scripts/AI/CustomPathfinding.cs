using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class CustomPathfinding : MonoBehaviour
{
    public enum State { Idle, Move, Alert, Combat, Passive, Flee, Pathfinding };
    public State currentState = State.Idle;

    [Space(10)]
    [HeaderAttribute("Field of View")]
    [TabGroup("Pathfinding")] [SerializeField] public float range;
    [TabGroup("Pathfinding")] [Range(0, 360)] public float viewAngle;
    [TabGroup("Pathfinding")] [SerializeField] LayerMask mask;
    [TabGroup("Pathfinding")] public float stoppingDistance = 4;
    RaycastHit hit;
    [FoldoutGroup("Debug")] public Vector3 directionVector;
    [FoldoutGroup("Debug")] public Transform target;
    [FoldoutGroup("Debug")] public Vector3 currentTarget;
    [FoldoutGroup("Debug")] public List<Move> attackQueue;

    [FoldoutGroup("Debug")] [SerializeField] private float yOffset = 0.5F;
    [FoldoutGroup("Debug")] public bool clearLine;
    [FoldoutGroup("Debug")] public bool withinAngle;
    [FoldoutGroup("Debug")] public bool inRange;



    [TabGroup("Pathfinding")] [HideInInspector] public float cornerMinDistance;
    [TabGroup("Pathfinding")] public float minDistance;
    int reachedCorner;
    private bool hasValidPath;
    [TabGroup("Pathfinding")] private float elapsed = 0.0f;
    [TabGroup("Pathfinding")] public float pathUpdateTime;
    public delegate void AIEvent();
    [TabGroup("Pathfinding")] public AIEvent detectEvent;
    [TabGroup("Pathfinding")] public float detectionDelay = 0.5F;
    [TabGroup("Pathfinding")] public float detectionSpread = 5F;

    [TabGroup("Pathfinding")] public LayerMask enemyMask;
    [TabGroup("Pathfinding")] public float crowdRange = 5F;
    public bool isWalking;

    private NavMeshPath path;
    protected Movement movement;
    protected bool run;

    float startDistance = 0;
    float endDistance = 0;

    protected void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = State.Idle;
    }

    protected void Start()
    {
        movement = GetComponent<Movement>();
        GameManager.Instance.advanceGameState += ExecuteFrame;
        path = new NavMeshPath();
        elapsed = pathUpdateTime;
    }
    protected void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
    }

    private void ExecuteFrame()
    {
        StateMachine();
        clearLine = ClearLine();
        withinAngle = WithinAngle();
        inRange = TargetInRange();
    }
    [Button]
    public void MoveToPosition(Vector3 p)
    {
        GoToState(State.Pathfinding);
        currentTarget = p;
    }
    [Button]
    public void MoveToPosition(Transform t)
    {
        GoToState(State.Pathfinding);
        currentTarget = t.position;
    }

    public virtual void StateMachine()
    {
        if (movement.ground)
            switch (currentState)
            {
                case State.Idle:
                    Idle();
                    break;
                case State.Pathfinding:
                    GoToPosition();
                    break;
            }
    }

    protected void GoToState(State state)
    {
        currentState = state;
    }

    protected void GoToPosition()
    {
        if (Vector3.Distance(transform.position, currentTarget) > stoppingDistance)
        {
            movement.direction = FindPath();
        }
        else
        {
            //Reached position
            GoToState(State.Idle);
            //if (WithinAngle())
            //    Idle();
            //else
            //{
            //    movement.strafeTarget = target;
            //    movement.strafe = true;
            //    movement.direction = FindPath();
            //}
        }
    }

    protected void Idle()
    {
        movement.isMoving = false;
        movement.direction = new Vector3(0, 0, 0);
    }

    #region Pathfinding
    protected void CalculatePath()
    {
        elapsed += Time.deltaTime;
        if (elapsed > pathUpdateTime && currentTarget != null)
        {
            reachedCorner = 0;
            elapsed -= pathUpdateTime;
            NavMesh.CalculatePath(transform.position, currentTarget, NavMesh.AllAreas, path);
        }
    }

    protected void CalculatePath(Vector3 v)
    {
        elapsed += Time.deltaTime;
        if (elapsed > pathUpdateTime)
        {
            reachedCorner = 0;
            elapsed -= pathUpdateTime;
            NavMesh.CalculatePath(transform.position, v, NavMesh.AllAreas, path);
        }
    }
    public Vector3 FindPath(Vector3 v)
    {
        CalculatePath(v);
        DebugPath();

        if (path.corners.Length > 1)
        {
            directionVector = (path.corners[reachedCorner + 1] - path.corners[reachedCorner]).normalized;

            if (isWalking) directionVector = directionVector * 0.5F;

            if (Vector3.Distance(transform.position, path.corners[reachedCorner + 1]) < cornerMinDistance)
            {
                hasValidPath = false;
                if (path.corners.Length > reachedCorner + 2)
                    reachedCorner++;
            }
            else
            {
                hasValidPath = true;
            }
        }
        return directionVector;
    }

    public void DebugPath()
    {
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.blue);
    }

    public Vector3 FindPath()
    {
        CalculatePath();
        DebugPath();

        if (path.corners.Length > 1)
        {
            directionVector = (path.corners[reachedCorner + 1] - path.corners[reachedCorner]).normalized;

            if (isWalking) directionVector = directionVector * 0.5F;

            if (Vector3.Distance(transform.position, path.corners[reachedCorner + 1]) < cornerMinDistance)
            {
                hasValidPath = false;
                if (path.corners.Length > reachedCorner + 2)
                    reachedCorner++;
            }
            else
            {
                hasValidPath = true;
            }
        }

        foreach (var item in AIManager.Instance.allEnemies)
        {
            if (item != this)
            {
                float f = Vector3.Distance(transform.position, item.transform.position);
                if (f < crowdRange && f != 0)
                {
                    directionVector += (transform.position - item.transform.position).normalized * (1 - (f / crowdRange));
                }
            }
        }
        if (Mathf.Abs(directionVector.x) > 0)
            movement.isMoving = true;
        return directionVector;
    }
    #endregion

    #region Help Functions

    public float Distance()
    {
        if (target == null) return 0;
        return Vector3.Distance(transform.position, target.transform.position);
    }

    protected bool ReachedNewDistance()
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
        return clearLine;
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

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
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
