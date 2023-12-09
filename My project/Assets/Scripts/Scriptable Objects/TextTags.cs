using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Text tags", menuName = "ScriptableObjects/TextTags", order = 1)]
public class TextTags : ScriptableObject
{
    public Color tagColor;
    public List<string> tags;
}
