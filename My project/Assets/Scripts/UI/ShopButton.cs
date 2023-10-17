using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class ShopButton : MonoBehaviour
{
    public int price;
    public SkillManager skillManager;
    public SkillSO skillSO;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image buttonBackground;
    public Image iconBackground;
    public Image icon;
    Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => BuySkill());
    }

    private void OnEnable()
    {
        SetupSkill();
    }

    [Button]
    public void SetupSkill()
    {

  
        switch (skillSO.skillRank)
        {
            case Rank.D:
                price = 100;
                buttonBackground.sprite = skillManager.NBackground;
                iconBackground.sprite = skillManager.NBackground;
                break;
            case Rank.C:
                price = 125;
                buttonBackground.sprite = skillManager.RBackground;
                iconBackground.sprite = skillManager.RBackground;
                break;
            case Rank.B:
                price = 150;
                buttonBackground.sprite = skillManager.SRBackground;
                iconBackground.sprite = skillManager.SRBackground;
                break;
            case Rank.A:
                price = 200;
                buttonBackground.sprite = skillManager.SSRBackground;
                iconBackground.sprite = skillManager.SSRBackground;
                break;
            case Rank.S:
                price = 500;
                buttonBackground.sprite = skillManager.URBackground;
                iconBackground.sprite = skillManager.URBackground;
                break;
            default:
                break;
        }

        priceText.text = "" + price;
        if (price > GameManager.Instance.gold)
            priceText.color = Color.red;

        titleText.text = skillSO.title;
        descriptionText.text = skillSO.description;

        icon.sprite = skillSO.sprite;

    }

    public void BuySkill()
    {
        if (GameManager.Instance.gold > price)
        {
            GameManager.Instance.gold -= price;
            skillManager.GetSkill(skillSO);
            gameObject.SetActive(false);
        }
    }
}
