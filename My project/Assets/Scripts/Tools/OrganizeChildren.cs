using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class OrganizeChildren : MonoBehaviour
{
    public Vector2 offset;

    [Button]
    public void SetupPosition()
    {
        Debug.Log("pos");

        RectTransform[] children = transform.GetComponentsInChildren<RectTransform>();
        List<RectTransform> layer1 = new List<RectTransform>();
        layer1.Clear();

        foreach (var item in children)
        {
            if (item.parent == transform)
                layer1.Add(item);
        }

        for (int i = 0; i < layer1.Count; i++)
        {
            if (i % 2 == 0)
            {
                layer1[i].localPosition = new Vector3(offset.x * i, 0, 0);
            }
            else
            {
                layer1[i].localPosition = new Vector3(offset.x * i, offset.y, 0);
            }
        }
    }
}
