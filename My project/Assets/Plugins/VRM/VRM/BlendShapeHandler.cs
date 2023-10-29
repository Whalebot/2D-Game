using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using VRM;
public class BlendShapeHandler : MonoBehaviour
{
    public BlendShapeAvatar blendShapeAvatar;
    [FoldoutGroup("Clips")]
    [InlineEditor] public BlendShapeClip blendShapeClip;

    public BlendShapeClip defaultBlendshape;
    public BlendShapeClip smileBlendshape;
    public BlendShapeClip angryBlendshape;
    public BlendShapeClip smugBlendshape;
    public BlendShapeClip blushBlendshape;
    public BlendShapeClip worriedBlendshape;
    //[Space(100)]
    public SkinnedMeshRenderer mr;
    [Range(0, 1)] public float blinkValue = 0;
    public float smoothSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnDisable()
    {

    }

    private void FixedUpdate()
    {
        if (mr != null)
            SetBlendShape(blinkValue);
    }
    void OnValidate()
    {
        if (mr != null)
            SetBlendShape(blinkValue);
    }
    [Button]
    void ResetBlendShapes()
    {
        for (int i = 0; i < mr.sharedMesh.blendShapeCount; i++)
        {
            if (mr != null)
                mr.SetBlendShapeWeight(i, 0);
        }
    }
    void SetBlendShape(float val)
    {
        foreach (var item in blendShapeClip.Values)
        {
            //    Debug.Log(item.ToString());
            if (mr != null)
                mr.SetBlendShapeWeight(item.Index, (item.Weight * val));
        }
    }
    [Button]
    void SetSmoothBlendShape(BlendShapeClip clip)
    {
        StartCoroutine(SmoothBlendShape(clip));
    }

    [Button("Thing", 50)]
    void SetBlendShape(BlendShapeClip clip, float val)
    {
        ResetBlendShapes();
        foreach (var item in clip.Values)
        {
            //    Debug.Log(item.ToString());
            mr.SetBlendShapeWeight(item.Index, (item.Weight * val));
        }
    }



    IEnumerator SmoothBlendShape(BlendShapeClip clip)
    {
        float val = 0;
        while (val < 1)
        {
            val += smoothSpeed;
            SetBlendShape(clip, val);
            yield return new WaitForFixedUpdate();
        }
        SetBlendShape(clip, 1);
    }


    void SetBlendShape()
    {
        foreach (var item in blendShapeClip.Values)
        {
            Debug.Log(item.ToString());
            mr.SetBlendShapeWeight(item.Index, item.Weight);
        }

    }


}
