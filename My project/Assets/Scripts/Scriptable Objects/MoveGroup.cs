using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move Group", menuName = "Move Group")]
public class MoveGroup : ScriptableObject
{
    public List<Move> moves;
}
