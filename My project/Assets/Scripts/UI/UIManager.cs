using System;
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
    public OrganizeChildren skillPanelOrganizer;
    [TabGroup("Components")] public GameObject pauseMenu;
    [TabGroup("Components")] public GameObject skillUIPrefab;
    [TabGroup("Components")] public HPBar playerHPBar;
    [TabGroup("Components")] public ButtonPrompt southPrompt;
    [TabGroup("Components")] public ButtonPrompt westPrompt;
    [TabGroup("Components")] public ButtonPrompt northPrompt;
    [TabGroup("Components")] public ButtonPrompt eastPrompt;
    [TabGroup("Components")] public Button rerollButton;
    [TabGroup("Components")] public TextMeshProUGUI rerollText;
    [TabGroup("Components")] public TextMeshProUGUI goldText;
    [TabGroup("Components")] public TextMeshProUGUI floorText;
    [TabGroup("Components")] public TextMeshProUGUI timeText;


    private void Awake()
    {
        Instance = this;

        playerHPBar.status = GameManager.Instance.playerStatus;
    }

    private void Start()
    {
        //GameManager.Instance.advanceGameState += ExecuteFrame;
        GameManager.Instance.getSkillEvent += OpenPowerupPanel;

        GameManager.Instance.pauseEvent += OpenPauseMenu;
        GameManager.Instance.resumeEvent += ClosePauseMenu;

        GameManager.Instance.openShopEvent += OpenShopPanel;
        rerollButton.onClick.AddListener(() => RerollButton());
        SetupSkillPanel();
    }
    void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
    }
    void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
    }
    void FixedUpdate()
    {
        goldText.text = "" + GameManager.Instance.Gold;
        rerollButton.interactable = GameManager.Instance.playerStatus.currentStats.rerolls > 0;
        rerollText.text = "x" + GameManager.Instance.playerStatus.currentStats.rerolls;
        floorText.text = "Floor " + LevelManager.Instance.currentMapNode.y;
        timeText.text = "" + TimeFormatter(GameManager.time, true);
    }
    public static string TimeFormatter(float seconds, bool forceHHMMSS = false)
    {
        float secondsRemainder = Mathf.Floor((seconds % 60) * 100) / 100.0f;
        int minutes = ((int)(seconds / 60)) % 60;
        int hours = (int)(seconds / 3600);

        if (!forceHHMMSS)
        {
            if (hours == 0)
            {
                return System.String.Format("{0:00}:{1:00.00}", minutes, secondsRemainder);
            }
        }
        return System.String.Format("{0}:{1:00}:{2:00}", hours, minutes, secondsRemainder);
    }
    [Button]
    public void SetupSkillPanel()
    {
        foreach (Transform child in skillPanelOrganizer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in SaveManager.Instance.LearnedSkills)
        {
            GameObject temp = Instantiate(skillUIPrefab, skillPanelOrganizer.transform, false);
            Image img = temp.transform.GetChild(0).gameObject.GetComponent<Image>();
            if (img != null) img.sprite = item.sprite;
        }
        skillPanelOrganizer.SetupPosition();
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
