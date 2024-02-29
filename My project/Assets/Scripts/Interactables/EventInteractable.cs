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
    public Button rerollButton;
    public TextMeshProUGUI rerollText;
    public override void Start()
    {
        rerollButton.onClick.AddListener(() => Reroll());
    }

    private void FixedUpdate()
    {
        rerollButton.interactable = GameManager.Instance.PlayerStats.rerolls > 0;
        rerollText.text = "" + GameManager.Instance.PlayerStats.rerolls;
    }

    public override void South()
    {
        base.South();
        canvas.SetActive(true);
        GameManager.Instance.ToggleMenu();
        RollSkills();
    }

    public void RollSkills()
    {
        foreach (var item in buttons)
        {
            item.GetSkills();
        }
    }

    public void Reroll()
    {
        GameManager.Instance.PlayerStats.rerolls--;
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
