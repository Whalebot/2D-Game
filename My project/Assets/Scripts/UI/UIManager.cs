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
    [TabGroup("Components")] public GameObject treasurePanel;
    [TabGroup("Components")] public GameObject blessingPanel, itemPanel, skillPanel;
    [TabGroup("Components")] public GameObject shopPanel;
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
    [TabGroup("Tooltip")] public RectTransform tooltip;
    [TabGroup("Tooltip")] public Vector3 tooltipOffset;
    [TabGroup("Tooltip")] public bool tooltipEnabled;
    [TabGroup("Tooltip")] public TextMeshProUGUI tooltipText;
    [TabGroup("Tooltip")] public GameObject skillDescriptionWindow;
    [TabGroup("Tooltip")] public Vector3 skillDescriptionOffset;
    [TabGroup("Tooltip")] public TextMeshProUGUI skillDescriptionText;
    [TabGroup("Tooltip")] public TextMeshProUGUI skillDescriptionTitle;



    private void Awake()
    {
        Instance = this;

        playerHPBar.status = GameManager.Instance.playerStatus;
    }

    private void Start()
    {
        //GameManager.Instance.advanceGameState += ExecuteFrame;
        GameManager.Instance.openRewardWindowEvent += OpenPowerupPanel;

        GameManager.Instance.pauseEvent += OpenPauseMenu;
        GameManager.Instance.resumeEvent += ClosePauseMenu;

        GameManager.Instance.openShopEvent += OpenShopPanel;
        SkillManager.Instance.pickedSkillEvent += SetupSkillPanel;

        rerollButton.onClick.AddListener(() => RerollButton());
        SetupSkillPanel(null);
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

        UpdateSkillIcon();

        //Vector3 mousePosition = Input.mousePosition; 
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(rect.parent as RectTransform, mousePosition, null, out Vector2 anchoredPosition); anchoredPosition += offset; rect.anchoredPosition = anchoredPosition;
        tooltip.position = Input.mousePosition + tooltipOffset;
        tooltip.gameObject.SetActive(tooltipEnabled);
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
    public void DisableTooltip() { tooltipEnabled = false; }

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
    public void SetupSkillPanel(SkillSO so)
    {
        foreach (Transform child in skillPanelOrganizer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in SaveManager.Instance.LearnedSkills)
        {
            GameObject temp = Instantiate(skillUIPrefab, skillPanelOrganizer.transform, false);
            SkillIcon icon = temp.GetComponent<SkillIcon>();
            icon.SetupIcon(item);
        }
        StartCoroutine(DelaySetupPosition());
    }
    IEnumerator DelaySetupPosition() {
        yield return new WaitForEndOfFrame(); skillPanelOrganizer.SetupPosition();
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
        treasurePanel.SetActive(true);
    }
    void OpenShopPanel()
    {
        shopPanel.SetActive(true);
    }
    public void CloseRewardPanels()
    {
        treasurePanel.SetActive(false);
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
