using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterVisuals : MonoBehaviour
{
    public CharacterVisualData visuals;
    public List<ColorPresetSO> allPresets;
    public List<GameObject> topOutifts;
    public List<GameObject> bottomOutfits;

    private void Start()
    {
        CharacterCreator.Instance.visualsUpdateEvent += UpdateVisuals;    }

    private void OnDisable()
    {
        CharacterCreator.Instance.visualsUpdateEvent -= UpdateVisuals;
    }

    void UpdateVisuals()
    {
        visuals = SaveManager.Instance.saveData.visuals;
        RemoveAllClothing();
        SetupOutfit();
    }

    [Button]
    void RemoveAllClothing()
    {
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
    void SetupOutfit()
    {
        if (topOutifts.Count > visuals.topID)
            topOutifts[visuals.topID].SetActive(true);
        else
            topOutifts[topOutifts.Count - 1].SetActive(true);

        if (bottomOutfits.Count > visuals.bottomID)
            bottomOutfits[visuals.bottomID].SetActive(true);
        else
            bottomOutfits[bottomOutfits.Count - 1].SetActive(true);

        //ApplyMaterial();
    }

    [Button]
    public void ApplyMaterial()
    {
        foreach (var item in allPresets[visuals.colorPreset].colorPresets)
        {
            item.material.SetColor("_MainColor", item.color);
        }
    }
}
