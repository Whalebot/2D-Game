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
        if (p1.material == null && p2.material == null) { return 0; }
        if (p2.material == null) { return -1; }
        if (p1.material == null) { return 1; }
        return p1.material.name.CompareTo(p2.material.name);
    }
}
[System.Serializable]
public class ColorPreset
{
    public Material material;
    public Color color;
}