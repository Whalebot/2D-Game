using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class Shop : Interactable
{
    public Rank rank;
    public GameObject shopPanel;
    public GameObject shopContainer;
    public TextMeshProUGUI rerollCounter;
    [TabGroup("Components")] public Button rerollButton;
    [TabGroup("Components")] public Button leaveShopButton;
    [TabGroup("Components")] public List<ShopButton> shopItems;
    [TabGroup("Components")] public List<ShopButton> shopBlessings;
    [TabGroup("Components")] public List<ShopButton> shopSkills;

    public override void Start()
    {
        rerollButton.onClick.AddListener(() => RerollShop());
        leaveShopButton.onClick.AddListener(() => CloseShop());
        RollShop(rank);
        base.Start();
    }
    private void FixedUpdate()
    {
        rerollCounter.text = "" + GameManager.Instance.playerStatus.currentStats.rerolls;
        rerollButton.interactable = GameManager.Instance.playerStatus.currentStats.rerolls > 0;
    }

    [Button]
    public void RollShop(Rank r)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            SkillSO skill = null;
            skill = SkillManager.Instance.RollSkill(SkillManager.Instance.items);
            shopItems[i].skillSO = skill;
            if (skill != null)
                shopItems[i].SetupSkill();
        }

        for (int i = 0; i < shopBlessings.Count; i++)
        {
            SkillSO skill = null;
            skill = SkillManager.Instance.RollSkill(SkillManager.Instance.blessings);
            shopBlessings[i].skillSO = skill;
            if (skill != null)
                shopBlessings[i].SetupSkill();
        }

        for (int i = 0; i < shopSkills.Count; i++)
        {
            SkillSO skill = null;
            skill = SkillManager.Instance.RollSkill(SkillManager.Instance.activeSkills);
            shopSkills[i].skillSO = skill;
            if (skill != null)
                shopSkills[i].SetupSkill();
        }
    }


    public void RerollShop()
    {
        GameManager.Instance.playerStatus.currentStats.rerolls--;
        RollShop(rank);
        SkillManager.Instance.selectedSkills.Clear();
    }

    public override void South()
    {
        base.South();
        OpenShop();
    }
    public void OpenShop()
    {
        Debug.Log("Try to open shop");
        shopPanel.SetActive(true);
        UIManager.Instance.SetActiveEventSystem(rerollButton.gameObject);
        GameManager.Instance.OpenShopWindow();
        //shopContainer.SetActive(false);
    }
    public void CloseShop() {
        shopPanel.SetActive(false);
        GameManager.Instance.CloseMenu();
    }
}
