using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class SkillSelectionButton : MonoBehaviour
{
    public SkillSO skillSO;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public GameObject replacementContainer;
    public TextMeshProUGUI replacementText;
    public Image iconImage;
    public Image buttonBackground;
    public List<GameObject> stars;
    Button button;
    bool tooltip = false;
    public List<ToolTipDescription> toolTips;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GetSkill());
    }

    private void FixedUpdate()
    {

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(descriptionText, Input.mousePosition, null);  // If you are not in a Canvas using Screen Overlay, put your camera instead of null
        if (linkIndex != -1)
        {
            tooltip = true;
            TMP_LinkInfo linkInfo = descriptionText.textInfo.linkInfo[linkIndex];
            //Debug.Log(linkInfo.GetLinkText());
            UIManager.Instance.EnableTooltip(linkInfo.GetLinkText());
        }
        else if (tooltip)
        {
            tooltip = false;
            UIManager.Instance.DisableTooltip();
        }
    }

    private void OnEnable()
    {
        SetupSkill();
    }

    [Button]
    public void SetupSkill()
    {
        titleText.text = skillSO.title;
        descriptionText.text = SkillManager.Instance.SkillDescription(skillSO);

        List<Tag> foundTags = SkillManager.Instance.GetTags(skillSO);

        foreach (var item in toolTips)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < foundTags.Count; i++)
        {
            if (toolTips.Count > i)
            {
                toolTips[i].gameObject.SetActive(true);
                toolTips[i].SetupToolTip(foundTags[i].title, SkillManager.Instance.CheckStringTags(foundTags[i].description));
            }
        }

        if (skillSO.sprite != null)
            iconImage.sprite = skillSO.sprite;

        SkillSO replacement = SkillManager.Instance.CheckReplacementBlessing(skillSO);
        SkillSO upgrade = SkillManager.Instance.CheckUpgrade(skillSO);

        if (replacement != null)
        {
            replacementContainer.SetActive(true);
            replacementText.text = "- Replaces " + replacement.name;
        }
        else
        {
            replacementContainer.SetActive(false);
        }


        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].SetActive(i <= (int)skillSO.skillRank);
        }

        switch (skillSO.skillRank)
        {
            case Rank.D:
                buttonBackground.sprite = SkillManager.Instance.NBackground;
                break;
            case Rank.C:
                buttonBackground.sprite = SkillManager.Instance.RBackground;
                break;
            case Rank.B:
                buttonBackground.sprite = SkillManager.Instance.SRBackground;
                break;
            case Rank.A:
                buttonBackground.sprite = SkillManager.Instance.SSRBackground;
                break;
            case Rank.S:
                buttonBackground.sprite = SkillManager.Instance.URBackground;
                break;
            default:
                break;
        }
    }

    public void GetSkill()
    {
        Debug.Log(UIManager.buttonDelay);
        if (UIManager.buttonDelay) return;
        SkillManager.Instance.GetSkill(skillSO);
        UIManager.Instance.DisableTooltip();
        UIManager.Instance.CloseRewardPanels();
    }
}
