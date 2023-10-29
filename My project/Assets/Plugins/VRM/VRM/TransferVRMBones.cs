using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using Sirenix.OdinInspector;

public class TransferVRMBones : MonoBehaviour
{
    public string hairTranslation;
    public VRMSpringBone[] springBones;
    public Transform root;

    // Start is called before the first frame update
    void Start()
    {

    }

    [Button]
    void GetAllSpringBones()
    {
        springBones = GetComponentsInChildren<VRMSpringBone>();
    }
    [Button]
    void ChangeCenter()
    {

        foreach (VRMSpringBone item in springBones)
        {
            item.m_center = transform;
        }
    }

    [Button]
    void TryToChangeRootBones()
    {

        foreach (VRMSpringBone item in springBones)
        {
            //Set Root
            item.m_center = root;
            //List<Transform> bonesToReplace = new List<Transform>();
            //foreach (Transform bone in item.RootBones)
            //{
            //    if (!bone.IsChildOf(transform))
            //    {
            //        bonesToReplace.Add(bone);
            //    }
            //}
            for (int i = 0; i < item.RootBones.Count; i++)
            {
                string boneName = item.RootBones[i].name;
                if (!item.RootBones[i].IsChildOf(transform))
                {


                    if (ReturnChildOfName(boneName) != null)
                        item.RootBones[i] = ReturnChildOfName(boneName);
                }
            }
            for (int i = 0; i < item.ColliderGroups.Length; i++)
            {
                string colliderName = item.ColliderGroups[i].name;
                if (!item.ColliderGroups[i].transform.IsChildOf(transform))
                {


                    if (TranslateName(colliderName) != null)
                    {
                        item.ColliderGroups[i] = ReturnChildContains(TranslateName(colliderName));
                    }

                }
            }
            ChangeCenter();
        }
    }


    string TranslateName(string s)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        string temp = s;
        if (s.Contains("L_LowerArm")) return "Left elbow";
        if (s.Contains("R_LowerArm")) return "Right elbow";
        if (s.Contains("L_UpperArm")) return "Left arm";
        if (s.Contains("R_UpperArm")) return "Right arm";
        if (s.Contains("L_UpperLeg")) return "Left leg";
        if (s.Contains("R_UpperLeg")) return "Right leg";
        if (s.Contains("L_Hand")) return "Left wrist";
        if (s.Contains("R_Hand")) return "Right wrist";
        if (s.Contains("UpperChest")) { Debug.Log("chest"); return "Upper Chest"; }


        if (s.Contains("J_Bip_C_"))
        {
            temp = temp.Replace("J_Bip_C_", "");
            return temp;
        }
        if (s.Contains("J_Bip_R_"))
        {
            temp = temp.Replace("J_Bip_R_", "");
            return temp;
        }
        if (s.Contains("J_Bip_L_"))
        {
            temp = temp.Replace("J_Bip_L_", "");
            return temp;
        }
        if (s.Contains("J_Sec_C_"))
        {
            temp = temp.Replace("J_Sec_C_", "");
            return temp;
        }
        if (s.Contains("J_Sec_R_"))
        {
            temp = temp.Replace("J_Sec_R_", "");
            return temp;
        }
        if (s.Contains("J_Sec_L_"))
        {
            temp = temp.Replace("J_Sec_L_", "");
            return temp;
        }

        return null;
    }
    VRMSpringBoneColliderGroup ReturnChildContains(string s)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform item in allChildren)
        {
            if (item.name.Contains(s))
            {
                VRMSpringBoneColliderGroup col = item.GetComponent<VRMSpringBoneColliderGroup>();
                if (col != null)
                    return col;
            }
        }
        return null;
    }

    Transform ReturnChildOfName(string s)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform item in allChildren)
        {
            if (item.name == s)
                return item;

        }
        if (ReturnStringName(s) != "")
        {
            return ReturnChildOfNameNoLoop(ReturnStringName(s));
        }
        return null;
    }

    Transform ReturnChildOfNameNoLoop(string s)
    {
        Debug.Log(s);
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform item in allChildren)
        {
            if (item.name == s)
                return item;
        }
        return null;
    }

    string ReturnStringName(string s)
    {
        string temp = s;
        if (s.Contains("J_Sec_Hair"))
        {
            temp = temp.Replace("J_Sec_Hair", hairTranslation);
            return temp;
        }
        if (s.Contains("L_Bust1")) return "Breast_L";
        if (s.Contains("R_Bust1")) return "Breast_R";

        return "";
    }
}
