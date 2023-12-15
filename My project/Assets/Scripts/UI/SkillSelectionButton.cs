using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class SkillSelectionButton : MonoBehaviour
{
    public SkillManager skillManager;
    public SkillSO skillSO;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public GameObject replacementContainer;
    public TextMeshProUGUI replacementText;
    public Image iconImage;
    public Image buttonBackground;
    Button button;
    bool tooltip = false;
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
        if (skillSO.sprite != null)
            iconImage.sprite = skillSO.sprite;

        SkillSO replacement = skillManager.CheckReplacementBlessing(skillSO);
        SkillSO upgrade = skillManager.CheckUpgrade(skillSO);

        if (replacement != null)
        {
            replacementContainer.SetActive(true);
            replacementText.text = "- Replaces " + replacement.name;
        }
        else
        {
            replacementContainer.SetActive(false);
        }

        switch (skillSO.skillRank)
        {
            case Rank.D:
                buttonBackground.sprite = skillManager.NBackground;
                break;
            case Rank.C:
                buttonBackground.sprite = skillManager.RBackground;
                break;
            case Rank.B:
                buttonBackground.sprite = skillManager.SRBackground;
                break;
            case Rank.A:
                buttonBackground.sprite = skillManager.SSRBackground;
                break;
            case Rank.S:
                buttonBackground.sprite = skillManager.URBackground;
                break;
            default:
                break;
        }
    }

    public void GetSkill()
    {
        skillManager.GetSkill(skillSO);

        UIManager.Instance.CloseRewardPanels();
    }
}
