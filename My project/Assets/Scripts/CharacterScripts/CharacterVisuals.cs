using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterVisuals : MonoBehaviour
{
    public CharacterVisualData visualData;
    public List<ColorPresetSO> allPresets;
    public List<GameObject> hairVariations;
    public List<GameObject> topOutifts;
    public List<GameObject> bottomOutfits;

    private void Start()
    {
        CharacterCreator.Instance.visualsUpdateEvent += UpdateVisuals;
    }

    private void OnDisable()
    {
        CharacterCreator.Instance.visualsUpdateEvent -= UpdateVisuals;
    }

    public void UpdateVisuals()
    {
        if (Application.isPlaying)
            visualData = SaveManager.Instance.saveData.visualData;
        RemoveAllClothing();
        SetupOutfit();
    }

    [Button]
    void RemoveAllClothing()
    {
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
    }
    [Button]
    public void SetupOutfit()
    {
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

        //ApplyMaterial();
    }

    [Button]
    public void ApplyMaterial()
    {
        foreach (var item in allPresets[visualData.colorPreset].colorPresets)
        {
            item.material.SetColor("_MainColor", item.color);
        }
    }
}
