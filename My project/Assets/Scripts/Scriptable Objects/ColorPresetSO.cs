using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Color Preset", menuName = "Color Preset")]
public class ColorPresetSO : ScriptableObject
{
   public List<ColorPreset> colorPresets;
}
[System.Serializable]
public class ColorPreset {
    public Material material;
    public Color color;
}