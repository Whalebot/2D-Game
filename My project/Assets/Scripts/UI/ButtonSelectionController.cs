using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ScrollRect))]
public class ButtonSelectionController : MonoBehaviour
{
    [Range(0, 1)]
    public float scrollValue;
    public Scrollbar bar;
    public Transform contentContainer;
    ScrollRect scroll;

    public void Awake()
    {
   

    }

    private void Start()
    {
        bar.numberOfSteps = GetChildrenInContainer() - 3;
        ResetScroll();
    }

    int GetChildrenInContainer()
    {
        int count = 0;
        foreach (Transform child in contentContainer)
        {
            if (child.gameObject.activeSelf)
                count++;
        }
        return count;
    }

    private void OnEnable()
    {
        scroll = GetComponent<ScrollRect>();
        ResetScroll();
    }

    private void OnDisable()
    {
        ResetScroll();
    }

    public void ResetScroll() {
        scrollValue = 1;
        scroll.verticalNormalizedPosition = 1;
    }
    public void UpdateScroll(bool down)
    {
        if (down) scrollValue = Mathf.Clamp(scrollValue - (1F / (bar.numberOfSteps-1)), 0, 1);
        else
            scrollValue = Mathf.Clamp(scrollValue + (1F / (bar.numberOfSteps-1)), 0, 1);
        scroll.verticalNormalizedPosition = scrollValue;
    }
}