using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrailSkillProperty", menuName = "ScriptableObjects/SkillProperty/TrailSkillProperty")]
public class TrailSkillProperty : UniqueSkillProperty
{
    public MoveGroup affectedMoves;
    public GameObject trailPrefab;
    public int trailSpawnInterval;
    public Vector3 boxSize;
    public LayerMask interactMask;
    int trailCounter;
    public override void AttackingBehaviour(SkillHandler handler, Rank rank)
    {
        base.AttackingBehaviour(handler, rank);

        if (!affectedMoves.moves.Contains(handler.attackScript.activeMove))
            return;
        trailCounter++;

        if (trailCounter >= trailSpawnInterval)
        {
            trailCounter = 0;
            Collider[] col = Physics.OverlapBox(handler.transform.position, boxSize * 0.5F, handler.transform.rotation, interactMask);
            Hazard foundHazard = null;
            foreach (Collider item in col)
            {
                foundHazard = item.GetComponentInParent<Hazard>();

            }
            if (foundHazard == null)
            {
                Status status = handler.GetComponent<Status>();
                GameObject go = Instantiate(trailPrefab, handler.transform.position, handler.transform.rotation);
                Hazard haz = go.GetComponent<Hazard>();
                haz.damageModifier = status.currentStats.faithModifier * status.currentStats.fireModifier;
                haz.rank = rank;
            }
            else
            {
                foundHazard.lifeCounter = 0;
            }
        }
    }
}
