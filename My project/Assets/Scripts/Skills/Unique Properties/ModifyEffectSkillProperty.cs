using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifyEffectSkillProperty", menuName = "ScriptableObjects/SkillProperty/ModifyEffectSkillProperty")]
public class ModifyEffectSkillProperty : UniqueSkillProperty
{
    public int modifiedStacks;
    public bool fireSpread;
    public bool poisonSpread;
    public void ModifyEffect(StatusEffect effect)
    {

        if (fireSpread)
        {
            if (effect.GetType() == typeof(BurningStatusEffect))
            {
                BurningStatusEffect temp = (BurningStatusEffect)effect;
                temp.spread = true;
            }
        }
        if (poisonSpread)
        {
            if (effect.GetType() == typeof(PoisonStatusEffect))
            {
                PoisonStatusEffect temp = (PoisonStatusEffect)effect;
                temp.spread = true;

                if (modifiedStacks > 0)
                    temp.maxStacks = modifiedStacks;
            }
        }
    }
}
