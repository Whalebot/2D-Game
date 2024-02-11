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
    public GameObject replacementContainer;
    public TextMeshProUGUI replacementText;
    public Image buttonBackground;
    public Image iconBackground;
    public Image icon;
    public SFX buySFX;
    Button button;
    bool tooltip = false;


    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => BuySkill());
        GameManager.Instance.goldChangeEvent += GoldChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.goldChangeEvent -= GoldChanged;
        UIManager.Instance.DisableTooltip();
    }

    private void OnEnable()
    {
        skillManager = SkillManager.Instance;
        SetupSkill();
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

        SetupSkill();
    }
    void GoldChanged(int changedMoney) {
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
        if (price > GameManager.Instance.Gold)
            priceText.color = Color.red;
        else priceText.color = Color.green;

        titleText.text = skillSO.title;
        descriptionText.text = SkillManager.Instance.SkillDescription(skillSO);
        if (skillSO.sprite != null)
            icon.sprite = skillSO.sprite;

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

    }

    public void BuySkill()
    {
        if (UIManager.buttonDelay) return;
        if (GameManager.Instance.Gold >= price)
        {
            GameManager.Instance.Gold -= price;
            AudioManager.Instance.PlaySFX(buySFX);
            skillManager.GetSkill(skillSO);
            gameObject.SetActive(false);
        }
    }
}
