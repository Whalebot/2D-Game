using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillGroupSO", menuName = "ScriptableObjects/SkillGroupSO", order = 1)]
public class SkillGroupSO : ScriptableObject
{
    public List<SkillSO> skills;
}
