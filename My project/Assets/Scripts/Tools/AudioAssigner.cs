using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class AudioAssigner : MonoBehaviour
{
    public List<Move> allMoves;
    // Start is called before the first frame update
    void Start()
    {

    }
#if UNITY_EDITOR
    [Button]
    void LoadItemSO()
    {
        //    allMoves.Clear();


        //    string[] skillNames = AssetDatabase.FindAssets("t:Move", new[] { "Assets/Movelist" });
        //    foreach (var SOName in skillNames)
        //    {
        //        var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
        //        var item = AssetDatabase.LoadAssetAtPath<Move>(SOpath);
        //        allMoves.Add(item);

        //    }
        //    foreach (var item in allMoves)
        //    {
        //        if (item.hitSFX.audioClips.Count == 0)
        //            if (item.hitSFX.audioClip != null)
        //                item.hitSFX.audioClips.Add(item.hitSFX.audioClip);

        //        foreach (var temp in item.sfx)
        //        {
        //            if (temp.audioClips.Count == 0)
        //                if (temp.audioClip != null)
        //                    temp.audioClips.Add(temp.audioClip);
        //        }

        //        item.SetDirty();
        //    }

    }

#endif
}
