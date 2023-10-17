using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterCreator : MonoBehaviour
{
    public static CharacterCreator Instance;

    [TabGroup("Debug")] public CharacterVisualData visuals;

    [TabGroup("Components")] public List<ColorPresetSO> allPresets;
    [TabGroup("Character Creation")] [InlineEditor] public ColorPresetSO preset;

    [TabGroup("Character Creation")] public GameObject target;
    [TabGroup("Character Creation")] public List<GameObject> outfit1Objects;
    [TabGroup("Character Creation")] public List<GameObject> outfit2Objects;

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

    void SaveVisuals() { SaveManager.Instance.saveData.visuals = visuals; }

    [Button]
    public void RandomizeVisuals()
    {
        visuals.colorPreset = UnityEngine.Random.Range(0, allPresets.Count);
        visuals.topID = UnityEngine.Random.Range(0, 2);
        visuals.bottomID = UnityEngine.Random.Range(0, 2);

        SaveVisuals();
        ApplyMaterial();
        visualsUpdateEvent?.Invoke();
    }

    [Button]
    void SwitchPreset()
    {
        visuals.colorPreset++;
        if (visuals.colorPreset >= allPresets.Count)
            visuals.colorPreset = 0;
        preset = allPresets[visuals.colorPreset];
        ApplyMaterial();
    }
    [Button]
    void Outfit1()
    {
        foreach (var item in outfit1Objects)
        {
            item.SetActive(true);
        }
        foreach (var item in outfit2Objects)
        {
            item.SetActive(false);
        }
    }
    [Button]
    void Outfit2()
    {
        foreach (var item in outfit1Objects)
        {
            item.SetActive(false);
        }
        foreach (var item in outfit2Objects)
        {
            item.SetActive(true);
        }
    }
    [Button]
    public void GetMaterials()
    {
        SkinnedMeshRenderer[] mr = target.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var item in mr)
        {
            foreach (var mat in item.sharedMaterials)
            {
                bool isDuplicate = false;
                foreach (var pres in preset.colorPresets)
                {
                    if (pres.material == mat)
                        isDuplicate = true;
                }
                if (!isDuplicate)
                {
                    ColorPreset temp = new ColorPreset();
                    temp.material = mat;
                    temp.color = Color.white;
                    preset.colorPresets.Add(temp);
                }
            }
        }
    }

    [Button]
    public void ApplyMaterial()
    {
        foreach (var item in allPresets[visuals.colorPreset].colorPresets)
        {
            item.material.SetColor("_MainColor", item.color);
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
}
