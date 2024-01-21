using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class OrganizeChildren : MonoBehaviour
{
    public Vector2 offset;

    private void Start()
    {
        SkillManager.Instance.pickedSkillEvent += SetupSkillPanel;
    }

    private void OnEnable()
    {
     
        SetupSkillPanel(null);
    }

    private void OnDisable()
    {
        //SkillManager.Instance.pickedSkillEvent -= SetupSkillPanel;
    }

    [Button]
    public void SetupSkillPanel(SkillSO so)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in SaveManager.Instance.LearnedSkills)
        {
            GameObject temp = Instantiate(UIManager.Instance.skillUIPrefab, transform, false);
            SkillIcon icon = temp.GetComponent<SkillIcon>();
            icon.SetupIcon(item);
        }
        //SetupPosition();
        StartCoroutine(DelaySetupPosition());
    }
    IEnumerator DelaySetupPosition()
    {
        yield return new WaitForEndOfFrame(); SetupPosition();
    }

    [Button]
    public void SetupPosition()
    {
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
