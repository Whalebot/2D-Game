using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class EventInteractable : Interactable
{
    public GameObject canvas;
    public List<EventButton> buttons;
    public override void Start()
    {
    }


    public override void South()
    {
        base.South();
        canvas.SetActive(true);
        GameManager.Instance.ToggleMenu();
        Reroll();
    }

    public void Reroll()
    {
        foreach (var item in buttons)
        {
            item.GetSkills();
        }
    }

    public void CloseMenu()
    {
        canvas.SetActive(false);
        GameManager.Instance.CloseMenu();
    }
}
