using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterCreator : MonoBehaviour
{
    public static CharacterCreator Instance;

    [TabGroup("Character Creation")] public CharacterVisualData visualData;
    [TabGroup("Character Creation")] [InlineEditor] public ColorPresetSO preset;
    [TabGroup("Character Creation")] public GameObject target;
    [TabGroup("Components")] public CharacterVisuals visuals;

    [TabGroup("Components")] public List<ColorPresetSO> allPresets;
    public event Action visualsUpdateEvent;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SaveManager.Instance.saveEvent += SaveVisuals;

        if (!SaveManager.Instance.HasSaveData())
            RandomizeVisuals();
    }
    private void OnValidate()
    {
        ApplyVisuals();
    }
    void SaveVisuals()
    {
        if (Application.isPlaying)
            SaveManager.Instance.saveData.visualData = visualData;
    }
    [Button]
    void ApplyVisuals()
    {
        SaveVisuals();
        ApplyMaterial();
        visualsUpdateEvent?.Invoke();
        if (Application.isEditor)
        {
            visuals.visualData = visualData;
            visuals.UpdateVisuals();
            if (visualData.colorPreset > allPresets.Count)
                preset = allPresets[allPresets.Count - 1];
            else
                preset = allPresets[visualData.colorPreset];
        }
    }

    [Button]
    public void RandomizeVisuals()
    {
        visualData.colorPreset = UnityEngine.Random.Range(0, allPresets.Count);
        visualData.hairID = UnityEngine.Random.Range(0, 2);
        visualData.topID = UnityEngine.Random.Range(0, 3);
        visualData.bottomID = UnityEngine.Random.Range(0, 2);

        ApplyVisuals();
    }

    [Button]
    void SwitchPreset()
    {
        visualData.colorPreset++;
        if (visualData.colorPreset >= allPresets.Count)
            visualData.colorPreset = 0;

        if (visualData.colorPreset > allPresets.Count)
            preset = allPresets[allPresets.Count - 1];
        else
            ApplyVisuals();
    }

    public void ApplyMaterial()
    {
        foreach (var item in allPresets[visualData.colorPreset].colorPresets)
        {
            Debug.Log(item.color);
            item.material.SetColor("_MainColor", item.color * item.color);
            Debug.Log(item.material.GetColor("_MainColor"));
        }
    }

    [Button]
    public void SaveMaterial()
    {
        foreach (var item in preset.colorPresets)
        {
            Debug.Log(item.material.GetColor("_MainColor"));
            item.color = item.material.GetColor("_MainColor");
        }
    }
    [Button]
    public void GetMaterials()
    {
        SkinnedMeshRenderer[] mr = target.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var thing in allPresets)
        {
            foreach (var item in mr)
            {
                foreach (var mat in item.sharedMaterials)
                {
                    bool isDuplicate = false;
                    foreach (var pres in thing.colorPresets)
                    {
                        if (pres.material == mat)
                            isDuplicate = true;
                    }
                    if (!isDuplicate)
                    {
                        ColorPreset temp = new ColorPreset();
                        temp.material = mat;
                        temp.color = Color.white;
                        thing.colorPresets.Add(temp);
                    }
                }
            }
        }
    }

}
