using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterVisuals : MonoBehaviour
{
    public bool loadVisuals = false;
    public ColorPresetSO presetSO;
    public CharacterVisualData visualData;
    public List<GameObject> weapons;
    public List<GameObject> hairVariations;
    public List<GameObject> topOutifts;
    public List<GameObject> bottomOutfits;
    public List<GameObject> shoes;
    public List<Material> materials;
    public Animator anim;
    private void Start()
    {
        GetMaterials();
        CharacterCreator.Instance.visualsUpdateEvent += UpdateVisuals;
        UpdateVisuals();
    }

    private void OnDisable()
    {
        CharacterCreator.Instance.visualsUpdateEvent -= UpdateVisuals;
    }
    [Button]
    public void GetMaterials()
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);

        if (Application.isPlaying)
            foreach (var item in renderers)
            {
                for (int i = 0; i < item.materials.Length; i++)
                {
                    materials.Add(item.materials[i]);
                    item.materials[i].name.Replace(" (Instance)", "");
                    // Debug.Log(item.materials[i].name);

                }
            }
    }
    [Button]
    public void ApplyMaterial()
    {
        ColorPresetSO preset = null;
        if (presetSO != null)
        {
            preset = presetSO;
        }
        else { preset = CharacterCreator.Instance.allPresets[visualData.colorPreset]; }

        foreach (var item in preset.colorPresets)
        {
            foreach (var mat in materials)
            {
                if (mat.name.Contains(item.material.name))
                    mat.SetColor("_MainColor", item.color * item.color);
            }
        }
    }
    [Button]
    public void RandomizeMaterials()
    {

        foreach (var mat in materials)
        {
            int RNG = Random.Range(0, CharacterCreator.Instance.allPresets.Count);
            ColorPresetSO preset = CharacterCreator.Instance.allPresets[RNG];
            foreach (var item in preset.colorPresets)
            {
                if (mat.name.Contains(item.material.name))
                    mat.SetColor("_MainColor", item.color * item.color);
            }
        }

    }

    [Button]
    public void UpdateVisuals()
    {
        if (Application.isPlaying && SaveManager.Instance != null && loadVisuals)
            visualData = SaveManager.Instance.VisualData;
        RemoveAllClothing();
        SetupOutfit();
    }

    [Button]
    void RemoveAllClothing()
    {
        foreach (var item in weapons)
        {
            item.SetActive(false);
        }
        foreach (var item in hairVariations)
        {
            item.SetActive(false);
        }
        foreach (var item in topOutifts)
        {
            item.SetActive(false);
        }
        foreach (var item in bottomOutfits)
        {
            item.SetActive(false);
        }
        foreach (var item in shoes)
        {
            item.SetActive(false);
        }
    }
    [Button]
    public void SetupOutfit()
    {

        if (weapons.Count > visualData.characterJob)
            weapons[visualData.characterJob].SetActive(true);
        else
            weapons[weapons.Count - 1].SetActive(true);

        if (hairVariations.Count > visualData.hairID)
            hairVariations[visualData.hairID].SetActive(true);
        else
            hairVariations[hairVariations.Count - 1].SetActive(true);

        if (topOutifts.Count > visualData.topID)
            topOutifts[visualData.topID].SetActive(true);
        else
            topOutifts[topOutifts.Count - 1].SetActive(true);

        if (bottomOutfits.Count > visualData.bottomID)
            bottomOutfits[visualData.bottomID].SetActive(true);
        else
            bottomOutfits[bottomOutfits.Count - 1].SetActive(true);

        if (shoes.Count > visualData.shoesID)
            shoes[visualData.shoesID].SetActive(true);
        else
            shoes[shoes.Count - 1].SetActive(true);

        if (Application.isPlaying)
        {
            anim.runtimeAnimatorController = CharacterCreator.Instance.characters[visualData.characterJob].controller;
            ApplyMaterial();
        }

    }
}
