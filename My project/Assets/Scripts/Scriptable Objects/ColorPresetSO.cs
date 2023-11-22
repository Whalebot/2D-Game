using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Color Preset", menuName = "Color Preset")]
public class ColorPresetSO : ScriptableObject
{
    public List<ColorPreset> colorPresets;


    [Button]
    public void SortByName()
    {
        colorPresets.Sort(SortByName);
    }
    static int SortByName(ColorPreset p1, ColorPreset p2)
    {
        if (p1.materialGroup == null && p2.materialGroup == null) { return 0; }
        if (p2.materialGroup == null) { return -1; }
        if (p1.materialGroup == null) { return 1; }
        return p1.materialGroup.name.CompareTo(p2.materialGroup.name);
    }
}
[System.Serializable]
public class ColorPreset
{
    public MaterialGroupSO materialGroup;
    public Color color;
}