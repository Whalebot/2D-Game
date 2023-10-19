using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonProperty", menuName = "MoveUniqueProperty/SummonProperty")]
public class SummonProperty : MoveUniqueProperty
{
    public GameObject summon;
    public Vector3 offset;

    public override void OnStartupFrame(AttackScript atk, int frame)
    {
        base.OnStartupFrame(atk, frame);
        Instantiate(summon, atk.transform.position + atk.transform.forward * offset.z + atk.transform.right * offset.x + atk.transform.up* offset.y, atk.transform.rotation);
    }

}
