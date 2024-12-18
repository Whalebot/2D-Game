using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(fileName = "SceneSO", menuName = "ScriptableObjects/SceneSO")]

public class SceneSO : ScriptableObject
{
    public Vector3 scenePosition;
    public Quaternion sceneRotation;
    public string sceneName;
#if UNITY_EDITOR    
    public SceneAsset sceneAsset;
    private void OnValidate()
    {
        
        sceneName = sceneAsset.name;

        //if (sceneAsset != null) {
        //    //this.name = sceneAsset.name;
        //    string assetPath = AssetDatabase.GetAssetPath(this);
        //    AssetDatabase.RenameAsset(assetPath, sceneAsset.name);
        //    //AssetDatabase.Refresh();
        //}
    }
#endif
}
