﻿
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
    [TabGroup("Stats")] public int attack;
    [TabGroup("Stats")] public int magic;
    [TabGroup("Stats")] public float attackSpeed = 1;
    [TabGroup("Stats")] public int airActions = 1;
    [TabGroup("Stats")] public int luck = 0;

    [Header("Modifiers")]
    [FoldoutGroup("Modifiers")] public float movementSpeedModifier = 1;
    [FoldoutGroup("Modifiers")] public float meterGainModifier = 1;
    [FoldoutGroup("Modifiers")] public float poiseRegen = 0;
    [FoldoutGroup("Modifiers")] public float poise = 0;
    [FoldoutGroup("Modifiers")] public float defense = 0;
    [FoldoutGroup("Modifiers")] public int resistance = 0;

    [FoldoutGroup("Modifiers")] public float guardBreakModifier = 1;
    [FoldoutGroup("Modifiers")] public float sizeModifier = 1;
    [FoldoutGroup("Modifiers")] public float damageModifierPercentage = 1;
    [FoldoutGroup("Modifiers")] public int damageModifierFlat = 0;
    [FoldoutGroup("Modifiers")] public float hitStunMultiplier = 1;
    [FoldoutGroup("Modifiers")] public float knockbackModifier = 1;
    [FoldoutGroup("Modifiers")] public float critChance = 0;
    [FoldoutGroup("Modifiers")] public float critMultiplier = 1.5f;


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
