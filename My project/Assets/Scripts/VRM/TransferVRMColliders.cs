using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using VRM;
using UnityEditor;
using System.Reflection;
public class TransferVRMColliders : MonoBehaviour
{
    public VRMSpringBoneColliderGroup[] colliderGroups;
    public Transform[] allChildren;
    public Transform target;

    [Button]
    void DeleteColliders()
    {
        VRMSpringBoneColliderGroup[] ownColliders = GetComponentsInChildren<VRMSpringBoneColliderGroup>(true);
        for (int i = ownColliders.Length; i > 0; i--)
        {
            Debug.Log(ownColliders.Length);
            DestroyImmediate(ownColliders[i - 1]);
        }
    }
    [Button]
    void GetAllColliders()
    {
        colliderGroups = target.GetComponentsInChildren<VRMSpringBoneColliderGroup>();
        allChildren = GetComponentsInChildren<Transform>(true);
    }
    [Button]
    void TransferColliders()
    {
        foreach (VRMSpringBoneColliderGroup item in colliderGroups)
        {
            string transformName = item.gameObject.name;
            if (ReturnChildOfName(transformName) != null)
                CopyComponent(item, ReturnChildOfName(transformName).gameObject);
        }
    }
    [Button]
    void CopyPosition()
    {
        foreach (VRMSpringBoneColliderGroup item in colliderGroups)
        {
            string transformName = item.gameObject.name;
            if (ReturnChildOfName(transformName) != null)
            {
                VRMSpringBoneColliderGroup temp = ReturnChildOfName(transformName).GetComponent<VRMSpringBoneColliderGroup>();
                for (int i = 0; i < temp.Colliders.Length; i++)
                {

                    Vector3 vec = Quaternion.Inverse(temp.transform.rotation) * item.Colliders[i].Offset;
                    Debug.Log(item.Colliders[i].Offset + " " + vec);
                    temp.Colliders[i].Offset = vec;
                }

            }
        }
    }

    Transform ReturnChildOfName(string s)
    {

        foreach (Transform item in allChildren)
        {
            //Debug.Log(item.gameObject.name + " " + s);
            if (item.gameObject.name == (s))
            {
                //Debug.Log(item.gameObject.name + " " + s);
                return item;
            }


        }

        if (ReturnStringName(s) != "")
        {
            return ReturnChildOfNameNoLoop(ReturnStringName(s));
        }
        return null;
    }

    Transform ReturnChildOfNameNoLoop(string s)
    {
        foreach (Transform item in allChildren)
        {
            if (item.name == s)
                return item;
        }
        return null;
    }
    string ReturnStringName(string s)
    {
        //Debug.Log("Trying to find: " + s);
        if (s.Contains("L_Bust1")) return "Breast_L";
        if (s.Contains("R_Bust1")) return "Breast_R";
        if (s.Contains("R_Hand")) return "Right wrist";
        if (s.Contains("L_Hand")) return "Left wrist";
        if (s.Contains("Spine")) return "Spine";
        if (s.Contains("Neck")) return "Neck";
        if (s.Contains("UpperChest")) return "Upper Chest";
        if (s.Contains("Head")) return "Head";
        if (s.Contains("L_UpperLeg")) return "Left leg";
        if (s.Contains("R_UpperLeg")) return "Right leg";
        if (s.Contains("L_UpperArm")) return "Left arm";
        if (s.Contains("R_UpperArm")) return "Right arm";
        if (s.Contains("L_LowerArm")) return "Left elbow";
        if (s.Contains("R_LowerArm")) return "Right elbow";

        return "";
    }

    [Button]

    VRMSpringBoneColliderGroup CopyComponent(VRMSpringBoneColliderGroup original, GameObject destination)
    {
        System.Type type = original.GetType();
        VRMSpringBoneColliderGroup copy = (VRMSpringBoneColliderGroup)destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
}
