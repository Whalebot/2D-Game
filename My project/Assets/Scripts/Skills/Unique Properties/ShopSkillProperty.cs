using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopSkillProperty", menuName = "ScriptableObjects/SkillProperty/ShopSkillProperty")]
public class ShopSkillProperty : UniqueSkillProperty
{
    public float shopPriceMultiplier;

    public override void ActivateBehaviour(SkillHandler handler)
    {
        base.ActivateBehaviour(handler);
        GameManager.Instance.ShopMultiplier += shopPriceMultiplier;
    }
}
