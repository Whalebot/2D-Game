using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SkillSO", menuName = "ScriptableObjects/Skill", order = 1)]
[System.Serializable]
public class SkillSO : ScriptableObject
{
    public Rank skillRank;
    public SkillType type;
    public string title;
 [PreviewField(50)]   public Sprite sprite;
    [ShowIf("@type == SkillType.Active")] public Move move;
    [ShowIf("@type == SkillType.Active")] public Move move2;
    [ShowIf("@type == SkillType.Active")] public Move move3;
    [TextArea(15, 20)]
    public string description;
    [ShowIf("@type == SkillType.Passive")]
    public Stats stats;
}

public enum Rank { D, C, B , A , S}

public enum SkillType { Active, Passive }