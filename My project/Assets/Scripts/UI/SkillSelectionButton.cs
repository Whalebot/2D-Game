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
        buttonBackground.sprite = skillSO.sprite;
    }

    public void GetSkill()
    {
        GameManager.Instance.player.GetComponent<SkillHandler>().SkillEXP(skillSO);
        SaveManager.Instance.saveData.learnedSkills.Add(skillSO);
        UIManager.Instance.ClosePowerupPanel();
    }
}
