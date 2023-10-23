using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject powerupPanel;
    public GameObject shopPanel;
    [TabGroup("Components")] public HPBar playerHPBar;
    [TabGroup("Components")] public ButtonPrompt southPrompt;
    [TabGroup("Components")] public ButtonPrompt westPrompt;
    [TabGroup("Components")] public ButtonPrompt northPrompt;
    [TabGroup("Components")] public ButtonPrompt eastPrompt;
    [TabGroup("Components")] public Button rerollButton;
    [TabGroup("Components")] public TextMeshProUGUI rerollText;
    [TabGroup("Components")] public TextMeshProUGUI goldText;


    private void Awake()
    {
        Instance = this;

        playerHPBar.status = GameManager.Instance.playerStatus;
    }

    private void Start()
    {
        //GameManager.Instance.advanceGameState += ExecuteFrame;
        GameManager.Instance.getSkillEvent += OpenPowerupPanel;
        GameManager.Instance.openShopEvent += OpenShopPanel;
        rerollButton.onClick.AddListener(() => RerollButton());
    }

    void FixedUpdate()
    {
        goldText.text = "" + GameManager.Instance.Gold;
        rerollButton.interactable = GameManager.Instance.playerStatus.currentStats.rerolls > 0;
        rerollText.text = "x" + GameManager.Instance.playerStatus.currentStats.rerolls;
    }

    public void RerollButton()
    {
        GameManager.Instance.playerStatus.currentStats.rerolls--;
        SkillManager.Instance.RollSkills(Rank.B);
    }
    void OpenPowerupPanel()
    {
        powerupPanel.SetActive(true);
    }
    void OpenShopPanel()
    {
        shopPanel.SetActive(true);
    }
    public void CloseRewardPanels()
    {
        powerupPanel.SetActive(false);
        shopPanel.SetActive(false);
        GameManager.Instance.CloseMenu();
        LevelManager.Instance.SpawnLevelGates();
    }
    public void SetupButtonPrompt(Interactable interactable)
    {
        if (interactable.CanWest())
            westPrompt.SetupPrompt(interactable.westType);
        else westPrompt.ResetPrompt();

        if (interactable.CanNorth())
            northPrompt.SetupPrompt(interactable.northType);
        else northPrompt.ResetPrompt();

        if (interactable.CanSouth())
            southPrompt.SetupPrompt(interactable.southType);
        else southPrompt.ResetPrompt();

        if (interactable.CanEast())
            eastPrompt.SetupPrompt(interactable.eastType);
        else eastPrompt.ResetPrompt();
    }
    public void DisableButtonPrompts()
    {
        westPrompt.DisablePrompt();
        northPrompt.DisablePrompt();
        southPrompt.DisablePrompt();
        eastPrompt.DisablePrompt();
    }

}
