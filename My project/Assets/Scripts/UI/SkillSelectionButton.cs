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
    public Image buttonBackground;
    Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GetSkill());
    }

    private void OnEnable()
    {
        SetupSkill();
    }

    [Button]
    public void SetupSkill()
    {
        titleText.text = skillSO.title;
        descriptionText.text = skillSO.description;
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
        UIManager.Instance.ClosePowerupPanel();
    }
}
