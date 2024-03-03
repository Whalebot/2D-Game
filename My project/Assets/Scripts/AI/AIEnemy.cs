using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;

public class AIEnemy : AI
{
    [HideInInspector] public Character character;
    public EnemyType enemyType;

    public AIAction currentAction;

    bool detectOnce;


    [TabGroup("Debug")] public List<Move> attackQueue;
    [TabGroup("Debug")] public int cooldown = 0;
    [TabGroup("Debug")] public int recoveryCooldown = 30;


    protected void Awake()
    {
        status = GetComponent<Status>();
        attack = GetComponent<AttackScript>();

        if (character == null)
            character = status.character;

        target = GameManager.Instance.playerStatus.col.transform;
        currentTarget = target.position;
        currentState = State.Idle;
    }

    protected void Start()
    {
        Initialize();
    }
    protected override void Initialize()
    {
        base.Initialize();
        movement = GetComponent<Movement>();
        GameManager.Instance.advanceGameState += ExecuteFrame;

        status.hitstunEvent += ResetAttack;
        status.healthEvent += Detect;
        status.hitRecoveryEvent += RecoveryCooldown;
        status.deathEvent += DeathEvent;

        elapsed = pathUpdateTime;
        AIManager.Instance.allEnemies.Add(this);
    }

    protected void OnDisable()
    {
        status.hitRecoveryEvent -= RecoveryCooldown;
        status.deathEvent -= DeathEvent;

        GameManager.Instance.advanceGameState -= ExecuteFrame;

        if (AIManager.Instance.allEnemies.Contains(this))
            AIManager.Instance.allEnemies.Remove(this);
    }

    void ResetAttack()
    {
        startDistance = 0;
        endDistance = 0;
        attackQueue.Clear();
    }

    void DeathEvent()
    {
        GameManager.Instance.SpawnGold(status.currentStats.gold, transform.position);
        //GameManager.Instance.Gold += status.currentStats.gold;
        switch (enemyType)
        {
            case EnemyType.Normal:
                SaveManager.Instance.CurrentData.enemiesKilled++;
                break;
            case EnemyType.Elite:
                SaveManager.Instance.CurrentData.enemiesKilled++;
                break;
            case EnemyType.Boss:
                SaveManager.Instance.CurrentData.bossesKilled++;
                break;
            default:
                break;
        }
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

    protected override void ExecuteFrame()
    {
        DetectionEvent();
        CalculatePath();

        //FindPath();
        if (status.NonAttackState())
        {
            if (!InCooldown() && detected || ReachedNewDistance())
            {
                RollAction();
            }
        }

        ResolveCooldown();
        StateMachine();

        if (debug)
        {
            distance = Distance();
            horizontalDistance = HorizontalDistance();
            verticalDistance = HeightDistance();
        }

        clearLine = ClearLine();
        withinAngle = WithinAngle();
        inRange = TargetInRange();
    }

    protected bool InCooldown()
    {
        return cooldown > 0;
    }

    void ResolveCooldown()
    {
        if (cooldown > 0 && status.NonAttackState()) cooldown--;
    }

    public override void StateMachine()
    {
        if (movement.ground && !status.inHitStun || flying && !status.inHitStun)
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

    protected virtual void RollAction()
    {
        startDistance = 0;
        endDistance = 0;

        List<AIAction> temp = new List<AIAction>();
        if (status.character == null) return;

        foreach (var item in status.character.actions)
        {
            if (Distance() < item.distance && Distance() > item.minDistance && HeightDistance() < item.maxHeight)
            {
                if (item.actionType == AIAction.ActionType.Attack && !ClearLine()) continue;
                temp.Add(item);
            }
        }

        int RNG = Random.Range(0, temp.Count);
        if (temp.Count > 0)
        {
            //Debug.Log(RNG);
            currentAction = temp[RNG];
            ExecuteAction(temp[RNG]);
        }
    }

    protected virtual void ExecuteAction(AIAction aiAction)
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

   protected void Attacking()
    {
        currentTarget = target.position;

        if (attackQueue.Count > 0)
        {
            if (attack.attackString || status.currentState == Status.State.Neutral)
            {
                movement.direction = DirectDirection();
                AttackQueue();
                return;
            }
        }

        if (!status.NonAttackState())
        {
            FindPath();
            movement.direction = DirectDirection();
        }
        else
        {
            FindPath();
            Idle();
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

   public void Detect()
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

                AIEnemy ai = item.GetComponent<AIEnemy>();
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


    protected virtual void Approach()
    {
        if (target != null)
        {
            currentTarget = target.position;

            if (Vector3.Distance(transform.position, target.transform.position) > stoppingDistance && !movement.CheckEdge())
            {
                movement.direction = MovementDirection();
            }
            else
            {
                Idle();
                //if (WithinAngle())
                //    Idle();
                //else
                //{
                //    movement.direction = MovementDirection();
                //}
            }
        }
        else
        {
            Idle();
        }
    }
    protected virtual void Flee()
    {
        if (target != null)
        {
            currentTarget = target.position;

            movement.isMoving = true;
            movement.direction = (-TargetDirectionIgnoreTilt() + movementOffset).normalized;
        }
        else
        {
            Idle();
        }
    }
    protected void Idle()
    {
        movement.isMoving = false;
        movement.direction = new Vector3(0, 0, 0);
    }



}

public enum EnemyType
{
    Normal, Elite, Boss
}