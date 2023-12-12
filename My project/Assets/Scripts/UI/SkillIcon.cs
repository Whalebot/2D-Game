using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    public Image background;
    public Image skillIcon;
    public SkillSO skillSO;

    public void SetupIcon(SkillSO skill)
    {
        skillSO = skill;
        skillIcon.sprite = skill.sprite;
    }

    public void ShowDescription()
    {
        UIManager.Instance.DisplaySkillDescription(this);
    }

    public void HideDescription() {
        UIManager.Instance.HideSkillDescription();
    }
}
