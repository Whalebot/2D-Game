using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventButton : MonoBehaviour
{
    Button button;

    public SkillSO newSkill;
    public SkillSO oldSkill;
    public TextMeshProUGUI newSkillName;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI oldSkillName;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => ExchangeSkills());
        GetSkills();
    }

    public void GetSkills()
    {
        newSkill = SkillManager.Instance.RollSkill();
        newSkillName.text = newSkill.name;
        descriptionText.text = newSkill.description;

        int RNG = Random.Range(0, SkillManager.Instance.learnedSkills.Count);
        if (SkillManager.Instance.learnedSkills.Count > 0)
            oldSkill = SkillManager.Instance.learnedSkills[0];

        if (oldSkill != null)
            oldSkillName.text = oldSkill.name;
        else
        {
            oldSkillName.text = "No skill to replace";
        }
    }
    public void ExchangeSkills()
    {
        SkillManager.Instance.RemoveSkill(oldSkill);
        SkillManager.Instance.GetSkill(newSkill);
        EventInteractable eventInteractable = GetComponentInParent<EventInteractable>();
        eventInteractable.CloseMenu();

    }
}
