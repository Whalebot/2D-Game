
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Stats
{
    [Header("Ressources")]

    [TabGroup("Debug")] public int currentHealth = 0;
    [TabGroup("Debug")] public float currentStamina = 0;

    [TabGroup("Debug")] public int experience = 0;
    [TabGroup("Debug")] public int level = 1;

    [TabGroup("Stats")] public int attack;
    [TabGroup("Stats")] public int maxHealth = 0;
    [TabGroup("Stats")] public float movementSpeedModifier = 1;
    [TabGroup("Stats")] public int magic;
    [Header("Resulting Secondary stats")]
    [TabGroup("Stats")] public float attackSpeed = 1;
    [TabGroup("Stats")] public float staminaRegen = 1;
    [TabGroup("Stats")] public float poiseRegen = 0;

    [FoldoutGroup("Defense")] public float poise = 0;
    [FoldoutGroup("Defense")] public float defense = 0;
    [FoldoutGroup("Defense")] public int resistance = 0;

    [Header("Modifiers")]
    [FoldoutGroup("Modifiers")] public float guardBreakModifier = 1;

    [FoldoutGroup("Modifiers")] public float sizeModifier = 1;
    [FoldoutGroup("Modifiers")] public float damageModifierPercentage = 1;
    [FoldoutGroup("Modifiers")] public int damageModifierFlat = 1;
    [FoldoutGroup("Modifiers")] public float hitStunMultiplier = 1;
    [FoldoutGroup("Modifiers")] public float knockbackModifier = 1;
    [FoldoutGroup("Modifiers")] public float critChance = 0;
    [FoldoutGroup("Modifiers")] public float critMultiplier = 0;


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
