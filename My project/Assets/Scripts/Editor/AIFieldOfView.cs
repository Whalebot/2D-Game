using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(AI))]
public class AIFieldOfView : OdinEditor
{
    private void OnSceneGUI()
    {
        AI thisAI = (AI)target;
        Handles.color = Color.white;

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
