using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pathfinding;

public class SuccubusBoss : AIEnemy
{
    [TabGroup("Succubus")] public List<Transform> teleportLocationsSide;
    [TabGroup("Succubus")] public List<Transform> teleportLocationTop;

    public void TeleportSide()
    {
        if (teleportLocationsSide.Count <= 0)
            return;

        // int RNG = Random.Range(0, teleportLocationsSide.Count);
        int teleportID = 0;
        float maxDist = 0;
        for (int i = 0; i < teleportLocationsSide.Count; i++)
        {
            float enemyTeleportDistance = Vector3.Distance(target.position, teleportLocationsSide[i].position);
            if (enemyTeleportDistance > maxDist)
            {
                maxDist = enemyTeleportDistance;
                teleportID = i;
            }
        }

        transform.position = teleportLocationsSide[teleportID].position;
    }
    public void TeleportTop()
    {
        if (teleportLocationTop.Count <= 0)
            return;

        int RNG = Random.Range(0, teleportLocationTop.Count);

        transform.position = teleportLocationTop[RNG].position;
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
    protected override void RollAction()
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

    protected override void ExecuteAction(AIAction aiAction)
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

    protected override void Approach()
    {
        TeleportSide();

    }
    protected override void Flee()
    {
        TeleportSide();

    }
}
