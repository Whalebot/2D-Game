using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo", menuName = "ScriptableObjects/Combo")]
public class Combo : ScriptableObject
{
    public string comboName;
    public List<Move> moves;
}