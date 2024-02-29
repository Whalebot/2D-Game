using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [TabGroup("Components")] public EventSystem eventSystem;
    [TabGroup("Components")] public ResultScreen resultScreen;
    [TabGroup("Components")] public GameObject treasurePanel;
    [TabGroup("Components")] public GameObject blessingPanel, itemPanel, skillPanel;
    [TabGroup("Components")] public OrganizeChildren skillPanelOrganizer;
    [TabGroup("Components")] public GameObject pauseMenu;
    [TabGroup("Components")] public GameObject skillUIPrefab;
    [TabGroup("Components")] public Image neutralSkillIcon, upSkillIcon, downSkillIcon, sideSkillIcon;
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
    [TabGroup("Components")] public TextMeshProUGUI goldChangeText;
    [TabGroup("Components")] public Animator goldUIAnimator;
    [TabGroup("Tooltip")] public RectTransform tooltip;
    [TabGroup("Tooltip")] public Vector3 tooltipOffset;
    [TabGroup("Tooltip")] public bool tooltipEnabled;
    [TabGroup("Tooltip")] public TextMeshProUGUI tooltipText;
    [TabGroup("Tooltip")] public GameObject skillDescriptionWindow;
    [TabGroup("Tooltip")] public Vector3 skillDescriptionOffset;
    [TabGroup("Tooltip")] public TextMeshProUGUI skillDescriptionText;
    [TabGroup("Tooltip")] public TextMeshProUGUI skillDescriptionTitle;
    [TabGroup("Settings")] public int goldTimer;
    int savedGoldCounter;
    int goldCounter;
    public static float buttonDelayCounter;
    public static bool buttonDelay;

    private void Awake()
    {
        Instance = this;

        if (GameManager.Instance != null)
            playerHPBar.status = GameManager.Instance.playerStatus;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.advanceGameState += ExecuteFrame;
            GameManager.Instance.openRewardWindowEvent += OpenPowerupPanel;

            GameManager.Instance.pauseEvent += OpenPauseMenu;
            GameManager.Instance.resumeEvent += ClosePauseMenu;
            GameManager.Instance.playerDeath += OpenResultScreen;
            GameManager.Instance.goldChangeEvent += GoldUIUpdate;

            ExecuteFrame();
        }
        rerollButton.onClick.AddListener(() => RerollButton());

    }

    public void GoldUIUpdate(int value)
    {
        savedGoldCounter += value;
        goldCounter = goldTimer;
        if (savedGoldCounter > 0)
        {
            goldChangeText.color = Color.green;
            goldChangeText.text = "+" + savedGoldCounter;
        }
        else
        {
            goldChangeText.color = Color.red;
            goldChangeText.text = "" + savedGoldCounter;
        }
        //goldChangeText.gameObject.SetActive(true);
        goldUIAnimator.SetTrigger("GainMoney");
    }

    void ResetGoldCounter()
    {
        savedGoldCounter = 0;

        //goldChangeText.gameObject.SetActive(false);
    }

    public void SetActiveEventSystem(GameObject go)
    {
        eventSystem.SetSelectedGameObject(null);
        if (InputManager.controlScheme != ControlScheme.Mouse)
        {
            eventSystem.SetSelectedGameObject(go);
        }
    }
    void OpenResultScreen()
    {
        resultScreen.gameObject.SetActive(true);
    }
    void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
    }
    void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (buttonDelayCounter > 0)
        {
            buttonDelayCounter--;
            if (buttonDelayCounter <= 0)
                buttonDelay = false;
        }
    }

    private void Update()
    {
        goldText.text = "" + GameManager.Instance.Gold;
        rerollButton.interactable = GameManager.Instance.playerStatus.currentStats.rerolls > 0;
        rerollText.text = "x" + GameManager.Instance.playerStatus.currentStats.rerolls;
        floorText.text = "Floor " + LevelManager.Instance.currentMapNode.y;
        timeText.text = "" + TimeFormatter(GameManager.time, true);

    }

    void ExecuteFrame()
    {
        if (savedGoldCounter > 0)
        {
            goldCounter--;
            if (goldCounter <= 0)
            {
                ResetGoldCounter();

            }
        }

        UpdateSkillIcon();

        tooltip.position = Input.mousePosition + tooltipOffset;
        tooltip.gameObject.SetActive(tooltipEnabled);
    }

    public void ButtonPressed()
    {
        buttonDelayCounter = 10;
        buttonDelay = true;
    }

    void UpdateSkillIcon()
    {
        AttackScript attack = GameManager.Instance.playerStatus.GetComponent<AttackScript>();
        if (attack.moveset.skillCombo.moves.Count > 0)
        {
            neutralSkillIcon.gameObject.SetActive(true);
            neutralSkillIcon.sprite = attack.moveset.skillCombo.moves[0].icon;

            if (GameManager.Instance.playerStatus.Meter < 50)
                neutralSkillIcon.color = Color.gray;
            else neutralSkillIcon.color = Color.white;
        }
        else neutralSkillIcon.gameObject.SetActive(false);

        if (attack.moveset.upSkillCombo.moves.Count > 0)
        {
            upSkillIcon.gameObject.SetActive(true);
            upSkillIcon.sprite = attack.moveset.upSkillCombo.moves[0].icon;

            if (GameManager.Instance.playerStatus.Meter < 50)
                upSkillIcon.color = Color.gray;
            else upSkillIcon.color = Color.white;
        }
        else upSkillIcon.gameObject.SetActive(false);

        if (attack.moveset.downSkillCombo.moves.Count > 0)
        {
            downSkillIcon.gameObject.SetActive(true);
            downSkillIcon.sprite = attack.moveset.downSkillCombo.moves[0].icon;

            if (GameManager.Instance.playerStatus.Meter < 50)
                downSkillIcon.color = Color.gray;
            else downSkillIcon.color = Color.white;
        }
        else downSkillIcon.gameObject.SetActive(false);

        if (attack.moveset.sideSkillCombo.moves.Count > 0)
        {
            sideSkillIcon.gameObject.SetActive(true);
            sideSkillIcon.sprite = attack.moveset.sideSkillCombo.moves[0].icon;

            if (GameManager.Instance.playerStatus.Meter < 50)
                sideSkillIcon.color = Color.gray;
            else sideSkillIcon.color = Color.white;
        }
        else sideSkillIcon.gameObject.SetActive(false);
    }

    public void DisplaySkillDescription(SkillIcon icon)
    {
        skillDescriptionWindow.SetActive(true);
        skillDescriptionTitle.text = icon.skillSO.title;
        skillDescriptionText.text = SkillManager.Instance.SkillDescription(icon.skillSO);
        skillDescriptionWindow.transform.position = icon.transform.position + skillDescriptionOffset;
    }
    public void HideSkillDescription()
    {
        skillDescriptionWindow.SetActive(false);
    }

    public void EnableTooltip(string key)
    {
        tooltipEnabled = true;
        tooltipText.text = key;
    }
    public void DisableTooltip() { 
        tooltipEnabled = false;
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

    public void RerollButton()
    {
        GameManager.Instance.playerStatus.currentStats.rerolls--;
        SkillManager.Instance.Reroll();
    }
    void OpenPowerupPanel(RewardType rewardType)
    {
        blessingPanel.SetActive(false);
        itemPanel.SetActive(false);
        skillPanel.SetActive(false);
        switch (rewardType)
        {
            case RewardType.Blessing:
                blessingPanel.SetActive(true);
                break;
            case RewardType.Item:
                itemPanel.SetActive(true);
                break;
            case RewardType.Skill:
                skillPanel.SetActive(true);
                break;
            case RewardType.Gold:
                break;
            default:
                break;
        }
        Debug.Log("PP panel");
        treasurePanel.SetActive(true);
    }

    public void CloseRewardPanels()
    {
        treasurePanel.SetActive(false);
        SetActiveEventSystem(null);
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
