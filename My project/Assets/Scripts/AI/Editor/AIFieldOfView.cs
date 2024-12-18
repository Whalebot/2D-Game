﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using UnityEditor;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(AIEnemy))]
public class AIFieldOfView : OdinEditor
{
    private void OnSceneGUI()
    {
        AIEnemy thisAI = (AIEnemy)target;
        Handles.color = Color.white;
        if (thisAI != null)
        {
            Vector3 offsetPosition = thisAI.transform.position + Vector3.up * thisAI.yOffset;

            Handles.DrawWireArc(offsetPosition, Vector3.forward, Vector3.right, 360, thisAI.range);
            Handles.color = Color.red;
            Handles.DrawWireArc(offsetPosition, Vector3.forward, Vector3.right, 360, thisAI.minDistance);
            Vector3 viewAngleA = thisAI.AngleToVector(-thisAI.viewAngle / 2);
            Vector3 viewAngleB = thisAI.AngleToVector(thisAI.viewAngle / 2);

            Handles.color = Color.blue;

            Handles.DrawLine(offsetPosition, offsetPosition + viewAngleA * thisAI.range);

            Handles.DrawLine(offsetPosition, offsetPosition + viewAngleB * thisAI.range);
        }

    }
}
