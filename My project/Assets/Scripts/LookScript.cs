using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LookScript : MonoBehaviour
{
    public Transform target;
    public Transform leftEye;
    public Transform rightEye;
    public Vector3 offsetLeft;
    public Vector3 offsetRight;

    //[Button]
    private void OnValidate()
    {

    }

    private void FixedUpdate()
    {
        leftEye.transform.LookAt(target);
        // leftEye.transform.up  = (target.position- leftEye.position).normalized;
        rightEye.transform.up = (target.position - leftEye.position).normalized;

        leftEye.rotation *= Quaternion.Euler(offsetLeft);
        rightEye.rotation *= Quaternion.Euler(offsetRight);
    }
}
