using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterCreator : MonoBehaviour
{
    public static CharacterCreator Instance;
    public bool liveUpdate;

    [TabGroup("Character Creation")] public CharacterVisualData visualData;
    [TabGroup("Character Creation")] [InlineEditor] public ColorPresetSO preset;
    [TabGroup("Character Creation")] public GameObject target;
    [TabGroup("Components")] public List<Character> characters;
    [TabGroup("Components")] public CharacterVisuals visuals;

    [TabGroup("Components")] public List<ColorPresetSO> allPresets;
    public event Action visualsUpdateEvent;

    private void Awake()
    {
        Instance = this;
        SaveManager.Instance.saveEvent += SaveVisuals;
        //SaveManager.Instance.awakeLoadEvent += LoadVisuals;

        if (GameManager.Instance != null)
        {
            LoadVisuals();
            GameManager.Instance.playerStatus.character = characters[SaveManager.Instance.saveData.visualData.characterJob];
            GameManager.Instance.player.GetComponent<AttackScript>().moveset = characters[SaveManager.Instance.saveData.visualData.characterJob].moveset;
        }
    }
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.player.GetComponentInChildren<Animator>().runtimeAnimatorController = characters[SaveManager.Instance.saveData.visualData.characterJob].controller;
        }

        if (!SaveManager.Instance.HasSaveData() && visuals != null)
            RandomizeVisuals();
    }
    public int Class
    {
        get
        {
            return visualData.characterJob;
        }
        set
        {
            visualData.characterJob = value;

            if (visualData.characterJob >= 3)
                visualData.characterJob = 0;
            if (visualData.characterJob < 0)
                visualData.characterJob = 2;

            ApplyVisuals();
        }
    }
    public int ColorPreset
    {
        get
        {
            return visualData.colorPreset;
        }
        set
        {
            visualData.colorPreset = value;

            if (visualData.colorPreset >= allPresets.Count)
                visualData.colorPreset = 0;
            if (visualData.colorPreset < 0)
                visualData.colorPreset = allPresets.Count - 1;

            ApplyVisuals();
        }
    }
    public int HairID
    {
        get
        {
            return visualData.hairID;
        }
        set
        {
            visualData.hairID = value;

            if (visualData.hairID >= visuals.hairVariations.Count)
                visualData.hairID = 0;
            if (visualData.hairID < 0)
                visualData.hairID = visuals.hairVariations.Count - 1;

            ApplyVisuals();
        }
    }
    public int TopID
    {
        get
        {
            return visualData.topID;
        }
        set
        {
            visualData.topID = value;

            if (visualData.topID >= visuals.topOutifts.Count)
                visualData.topID = 0;
            if (visualData.topID < 0)
                visualData.topID = visuals.topOutifts.Count - 1;

            ApplyVisuals();
        }
    }
    public int BottomID
    {
        get
        {
            return visualData.bottomID;
        }
        set
        {
            visualData.bottomID = value;

            if (visualData.bottomID >= visuals.bottomOutfits.Count)
                visualData.bottomID = 0;
            if (visualData.bottomID < 0)
                visualData.bottomID = visuals.bottomOutfits.Count - 1;

            ApplyVisuals();
        }
    }
    public int ShoesID
    {
        get
        {
            return visualData.shoesID;
        }
        set
        {
            visualData.shoesID = value;

            if (visualData.shoesID >= visuals.shoes.Count)
                visualData.shoesID = 0;
            if (visualData.shoesID < 0)
                visualData.shoesID = visuals.shoes.Count - 1;

            ApplyVisuals();
        }
    }
    private void OnValidate()
    {
        if (liveUpdate && !Application.isPlaying)
            ApplyVisuals();
    }

    private void FixedUpdate()
    {
        if (liveUpdate && !Application.isPlaying)
            ApplyVisuals();
    }

    //    private void OnDrawGizmos()
    //    {
    //#if UNITY_EDITOR
    //        if (liveUpdate)
    //            ApplyVisuals();


    //        // Ensure continuous Update calls.
    //        if (!Application.isPlaying)
    //        {
    //            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
    //            UnityEditor.SceneView.RepaintAll();
    //        }
    //#endif
    //    }

    void SaveVisuals()
    {
        if (Application.isPlaying)
        {
            Debug.Log("Saving visuals");
            SaveManager.Instance.saveData.visualData = visualData;
        }
    }
    void LoadVisuals()
    {
        if (Application.isPlaying)
        {
            visualData = SaveManager.Instance.saveData.visualData;
            ApplyVisuals();
        }
    }
    [Button]
    void ApplyVisuals()
    {
        ApplyMaterial();
        visualsUpdateEvent?.Invoke();
        if (visuals != null)
        {
            visuals.visualData = visualData;
            visuals.UpdateVisuals();
        }

        if (liveUpdate)
        {
            if (visualData.colorPreset >= allPresets.Count)
                preset = allPresets[allPresets.Count - 1];
            else
                preset = allPresets[visualData.colorPreset];
        }
    }

    [Button]
    public void RandomizeVisuals()
    {
        visualData.colorPreset = UnityEngine.Random.Range(0, allPresets.Count);
        visualData.hairID = UnityEngine.Random.Range(0, visuals.hairVariations.Count);
        visualData.topID = UnityEngine.Random.Range(0, visuals.topOutifts.Count);
        visualData.bottomID = UnityEngine.Random.Range(0, visuals.bottomOutfits.Count);
        visualData.bottomID = UnityEngine.Random.Range(0, visuals.shoes.Count);

        SaveVisuals();
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

        int currentPreset = visualData.colorPreset;
        if (visualData.colorPreset >= allPresets.Count)
            currentPreset = allPresets.Count - 1;

        foreach (var item in allPresets[currentPreset].colorPresets)
        {
            item.material.SetColor("_MainColor", item.color * item.color);
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
