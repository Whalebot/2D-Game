using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class StatDisplay : MonoBehaviour
{
    public Character character;
    public Status status;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;

    public TextMeshProUGUI damageText;

    public TextMeshProUGUI attackText;
    public TextMeshProUGUI magicText;

    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI mDefenseText;

    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI movementSpeedText;

    public TextMeshProUGUI criticalChanceText;
    public TextMeshProUGUI criticalDamageText;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.playerStatus.statEvent += ShowPlayerStats;
    }

    private void OnEnable()
    {
        SetupStatText(GameManager.Instance.playerStatus.currentStats);
    }

    private void OnDisable()
    {
        GameManager.Instance.playerStatus.statEvent -= ShowPlayerStats;
    }

    public void ShowPlayerStats()
    {
        SetupStatText(GameManager.Instance.playerStatus.currentStats);
        SetupColors(GameManager.Instance.playerStatus);
    }

    [Button]
    public void ShowCharacterStats()
    {
        SetupStatText(character.stats);
    }

    public void SetupStatText(Stats s)
    {
        healthText.text = s.currentHealth + "/" + s.maxHealth;
        manaText.text = s.currentMeter + "/" + s.maxMeter;

        damageText.text = (s.damageModifierPercentage * 100).ToString("F0") + "%";

        attackText.text = "" + s.attack;
        magicText.text = "" + s.attack;

        defenseText.text = (s.defense * 100).ToString("F0") + "%";
        mDefenseText.text = (s.defense * 100).ToString("F0") + "%";


        attackSpeedText.text = (s.attackSpeed * 100).ToString("F0") + "%";
        movementSpeedText.text = (s.movementSpeedModifier * 100).ToString("F0") + "%";

        criticalChanceText.text = (s.critChance * 100).ToString("F0") + "%";
        criticalDamageText.text = (s.critMultiplier * 100).ToString("F0") + "%";
    }

    public void SetupColors(Status status)
    {
        Stats c = status.currentStats;
        Stats b = status.baseStats;

        if (c.maxHealth > b.maxHealth)
            healthText.color = Color.green;
        if (c.maxHealth < b.maxHealth)
            healthText.color = Color.red;

        if (c.maxMeter > b.maxMeter)
            manaText.color = Color.green;
        if (c.maxMeter < b.maxMeter)
            manaText.color = Color.red;


        if (c.damageModifierPercentage > b.damageModifierPercentage)
            damageText.color = Color.green;
        if (c.damageModifierPercentage < b.damageModifierPercentage)
            damageText.color = Color.red;


        if (c.attack > b.attack)
            attackText.color = Color.green;
        if (c.attack < b.attack)
            attackText.color = Color.red;
        if (c.attack > b.attack)
            magicText.color = Color.green;
        if (c.attack < b.attack)
            magicText.color = Color.red;

        if (c.defense > b.defense)
            defenseText.color = Color.green;
        if (c.defense < b.defense)
            defenseText.color = Color.red;
        if (c.defense > b.defense)
            mDefenseText.color = Color.green;
        if (c.defense < b.defense)
            mDefenseText.color = Color.red;

        if (c.attackSpeed > b.attackSpeed)
            attackSpeedText.color = Color.green;
        if (c.attackSpeed < b.attackSpeed)
            attackSpeedText.color = Color.green;

        if (c.attackSpeed > b.attackSpeed)
            movementSpeedText.color = Color.green;
        if (c.attackSpeed < b.attackSpeed)
            movementSpeedText.color = Color.red;

        if (c.critChance > b.critChance)
            criticalChanceText.color = Color.green;
        if (c.critChance < b.critChance)
            criticalChanceText.color = Color.red;
        if (c.critMultiplier > b.critMultiplier)
            criticalDamageText.color = Color.green;
        if (c.critMultiplier < b.critMultiplier)
            criticalDamageText.color = Color.red;
    }
}
