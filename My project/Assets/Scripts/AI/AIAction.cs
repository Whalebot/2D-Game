using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu()]
public class AIAction : ScriptableObject
{
    public enum ActionType
    {
        Attack,
        Approach,
        Flee
    }
    public ActionType actionType;
    //Conditions
    public float minDistance;
    public float maxHeight = 2;
    public float distance;
    [HideIf("@actionType == ActionType.Attack")] public float targetDistance;
    public int cooldown;
    [HideIf("@actionType != ActionType.Attack")] public Combo combo;
    //Actions
    //Attack
    //Approach
    //Flee
}