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
    int counter;
    public override void AttackingBehaviour(SkillHandler handler)
    {
        base.AttackingBehaviour(handler);

        if (!affectedMoves.moves.Contains(handler.attackScript.activeMove))
            return;
        counter++;

        if (counter >= trailSpawnInterval)
        {
            counter = 0;
            Collider[] col = Physics.OverlapBox(handler.transform.position, boxSize * 0.5F, handler.transform.rotation, interactMask);
            Hazard foundHazard = null;
            foreach (Collider item in col)
            {
                Debug.Log(item.gameObject);
                foundHazard = item.GetComponentInParent<Hazard>();

            }
            if (foundHazard == null)
                Instantiate(trailPrefab, handler.transform.position, handler.transform.rotation);
            else
            {
                foundHazard.lifeCounter = 0;
            }
        }
    }
}
