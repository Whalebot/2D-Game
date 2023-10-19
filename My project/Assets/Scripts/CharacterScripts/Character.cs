using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Character", menuName = "ScriptableObjects/Character", order = 2)]
public class Character : ScriptableObject
{
    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    public string characterName;
    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    public string subTitle;

    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    public string description;

    [HorizontalGroup("Game Data", 75)]
    [PreviewField(75)] public Sprite image;
    [HorizontalGroup("Game Data", 75)]
    [PreviewField(75)]
    public GameObject prefab;
    public RuntimeAnimatorController controller;
    public Moveset moveset; 

    [InlineEditor] public List<AIAction> actions;
    [HideLabel] public Stats stats;
    public List<SkillSO> skills;
}
