
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Stats
{
    [TabGroup("Stats")] public int currentHealth = 0;
    [TabGroup("Stats")] public int maxHealth = 0;
    [TabGroup("Stats")] public int currentMeter = 0;
    [TabGroup("Stats")] public int maxMeter = 0;
    [TabGroup("Stats")] public int level = 1;
    [TabGroup("Stats")] public int strength = 0;
    [TabGroup("Stats")] public int intelligence = 0;
    [TabGroup("Stats")] public int agility = 0;
    [TabGroup("Stats")] public int faith = 0;
    [TabGroup("Stats")] public int luck = 0;
    [TabGroup("Stats")] public int gold = 0;

    [TabGroup("Stats")] public int attack;
    [TabGroup("Stats")] public int magic;
    [TabGroup("Stats")] public float faithModifier = 0;
    [TabGroup("Stats")] public float attackSpeed = 1;
    [TabGroup("Stats")] public int jumps = 1;
    [TabGroup("Stats")] public int airActions = 1;

    [TabGroup("Stats")] public int rerolls = 0;

    [Header("Modifiers")]
    [TabGroup("Modifiers")] public float fireModifier = 0;
    [TabGroup("Modifiers")] public float iceModifier = 0;
    [TabGroup("Modifiers")] public float lightningModifier = 0;
    [TabGroup("Modifiers")] public float earthModifier = 0;
    [TabGroup("Modifiers")] public float windModifier = 0;
    [TabGroup("Modifiers")] public float poisonModifier = 0;

    [TabGroup("Modifiers")] public float movementSpeedModifier = 1;
    [TabGroup("Modifiers")] public float meterGainModifier = 1;
    [TabGroup("Modifiers")] public float poiseRegen = 0;
    [TabGroup("Modifiers")] public float poise = 0;
    [TabGroup("Modifiers")] public float defense = 0;
    [TabGroup("Modifiers")] public int resistance = 0;

    [TabGroup("Modifiers")] public float guardBreakModifier = 1;
    [TabGroup("Modifiers")] public float sizeModifier = 1;
    [TabGroup("Modifiers")] public float damageModifierPercentage = 1;
    [TabGroup("Modifiers")] public float backstabModifier = 0;
    [TabGroup("Modifiers")] public int damageModifierFlat = 0;
    [TabGroup("Modifiers")] public float hitStunMultiplier = 1;
    [TabGroup("Modifiers")] public float knockbackModifier = 1;
    [TabGroup("Modifiers")] public float critChance = 0;
    [TabGroup("Modifiers")] public float critMultiplier = 1.5f;
    [TabGroup("Modifiers")] public float lifesteal = 0;

    [TabGroup("Modifiers")] public float spellSpeed = 0;

    public void ReplaceStats(Stats newStats)
    {

        //Get stat definition and replace 1 with 2
        Stats def1 = this;
        Stats def2 = newStats;

        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;
            defInfo1[i].SetValue(obj, defInfo2[i].GetValue(obj2));
        }
    }
    [Button]
    public void CalculateValues()
    {
        attack = strength;
        magic = intelligence;
        faithModifier = faith * 0.2F;
    }
    [Button]
    public void ResetValues()
    {
        FieldInfo[] defInfo1 = this.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = this;

            object var1 = defInfo1[i].GetValue(obj);


            if (var1 is int)
            {
                defInfo1[i].SetValue(obj, (int)0);
            }
            else if (var1 is float)
            {
                defInfo1[i].SetValue(obj, 0);
            }
            //else if (var1 is bool)
            //{
            //    //SET VALUES
            //    if ((bool)var2)
            //        defInfo1[i].SetValue(obj, defInfo2[i].GetValue(obj2));
            //}
        }
    }
}
