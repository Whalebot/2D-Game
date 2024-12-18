using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;

public class AI : MonoBehaviour
{
    private Seeker seeker;
    public enum State { Idle, Move, Alert, Combat, Passive, Flee };
    public State currentState = State.Idle;

    [Space(10)]
    [HeaderAttribute("Field of View")]
    [TabGroup("Pathfinding")] [SerializeField] public float range;
    [TabGroup("Pathfinding")] [Range(0, 360)] public float viewAngle;
    [TabGroup("Pathfinding")] [SerializeField] protected LayerMask mask;

    [TabGroup("Pathfinding")] [HideInInspector] public float cornerMinDistance;
    [TabGroup("Pathfinding")] public float minDistance;
    [TabGroup("Pathfinding")] public int movementChangeTime = 30;
    protected int movementChangeCounter;
    protected RaycastHit hit;
    [TabGroup("Debug")] public Vector3 movementDirection;
    [TabGroup("Debug")] public Vector3 directionVector;
    [TabGroup("Debug")] public Vector3 movementOffset;
    [TabGroup("Debug")] public bool reachedEndOfPath;
    [TabGroup("Pathfinding")] public Path path;
    [HideInInspector] public int elapsed = 0;
    [TabGroup("Pathfinding")] public int pathUpdateTime;

    [TabGroup("Pathfinding")] public Action detectEvent;
    [TabGroup("Pathfinding")] public float detectionDelay = 0.5F;
    [TabGroup("Pathfinding")] public float detectionSpread = 5F;
    [TabGroup("Pathfinding")] public float nextWaypointDistance = 3;

    private int currentWaypoint = 0;

    [TabGroup("Pathfinding")] public LayerMask enemyMask;
    [TabGroup("Pathfinding")] public float stoppingDistance = 4;
    [TabGroup("Pathfinding")] public bool detected;


    public bool flying;

    protected Movement movement;
    protected Status status;
    protected AttackScript attack;
    protected AIManager manager;


    [TabGroup("Debug")] public bool debug = true;
    [TabGroup("Debug")] public Transform target;
    [TabGroup("Debug")] public Vector3 currentTarget;

    protected float startDistance = 0;
    protected float endDistance = 0;

    [TabGroup("Debug")] [SerializeField] public float yOffset = 0.5F;
    [TabGroup("Debug")] public float distance;
    [TabGroup("Debug")] public float horizontalDistance;
    [TabGroup("Debug")] public float verticalDistance;
    [TabGroup("Debug")] public bool clearLine;
    [TabGroup("Debug")] public bool withinAngle;
    [TabGroup("Debug")] public bool inRange;

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        seeker = GetComponent<Seeker>();
    }

    protected virtual void ExecuteFrame()
    {
    }

    public virtual void StateMachine()
    {
    }
    protected Vector3 MovementDirection()
    {
        movementChangeCounter--;

        if (movementChangeCounter <= 0)
        {
            movementChangeCounter = movementChangeTime;
            movementDirection = FindPath();
        }
        return movementDirection;
    }

    #region Pathfinding
    public void OnPathComplete(Path p)
    {
        // Path pooling. To avoid unnecessary allocations paths are reference counted.
        // Calling Claim will increase the reference count by 1 and Release will reduce
        // it by one, when it reaches zero the path will be pooled and then it may be used
        // by other scripts. The ABPath.Construct and Seeker.StartPath methods will
        // take a path from the pool if possible. See also the documentation page about path pooling.
        p.Claim(this);
        if (!p.error)
        {
            if (path != null) path.Release(this);
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
        else
        {
            p.Release(this);
        }
    }
    protected void CalculatePath()
    {
        elapsed++;

        if (elapsed > pathUpdateTime && target != null && seeker.IsDone())
        {
            if (currentState != State.Combat)
            {
                ////Find ground position under player
                //if (Physics.Raycast(target.position, Vector3.down * 10f, out RaycastHit hitInfo, 10f, movement.groundMask))
                //{
                //    currentTarget = hit.point + movementOffset;
                //}
                //else
                //{

                //}
                currentTarget = target.position + movementOffset;
            }
            else
            {
                currentTarget = target.position;
            }
            elapsed -= pathUpdateTime;
            seeker.StartPath(transform.position, currentTarget, OnPathComplete);
        }
    }
    public Vector3 FindPath()
    {
        CalculatePath();

        if (path == null)
        {
            Debug.Log("No path");
            return (currentTarget - transform.position).normalized;
        }

        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }
        }

        Vector3 temp = (path.vectorPath[currentWaypoint] - transform.position);
        if (!flying) temp.y = 0;
        temp.z = 0;
        directionVector = temp.normalized;

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
    public Vector3 DirectDirection()
    {
        movement.isMoving = true;
        return (target.position - transform.position).normalized;
    }
    #endregion

    #region Help Functions
    public float Distance()
    {
        if (target == null) return 0;
        Vector3 v1 = transform.position;
        Vector3 v2 = target.transform.position;

        return Vector3.Distance(v1, v2);
    }
    public float HorizontalDistance()
    {
        if (target == null) return 0;
        Vector3 v1 = transform.position;
        v1.y = 0;
        Vector3 v2 = target.transform.position;
        v2.y = 0;

        return Vector3.Distance(v1, v2);
    }
    public float HeightDistance()
    {
        if (target == null) return 0;
        Vector3 v1 = transform.position;
        Vector3 v2 = target.transform.position;

        return Mathf.Abs(v1.y - v2.y);
    }
    public float DistanceIncludeHeight()
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
            return HorizontalDistance() < endDistance;
        }
        else
            return HorizontalDistance() > endDistance;
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
        bool tempLine = Physics.Raycast(transform.position + Vector3.up * yOffset, (target.transform.position - (transform.position + Vector3.up * yOffset)).normalized, out hit, 1000, mask);

        Debug.DrawLine(transform.position + Vector3.up * yOffset, hit.point, Color.red);
        //if (hit.transform.IsChildOf(target))
        //clearLine = tempLine;
        // Debug.Log(hit.transform.gameObject + " " + tempLine);
        if (tempLine)
            return target.IsChildOf(hit.transform);
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
        foreach (AIEnemy enemy in AIManager.Instance.allEnemies)
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
