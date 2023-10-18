using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class HubMenu : MonoBehaviour
{
    public TextMeshProUGUI colorNumber;
    public TextMeshProUGUI hairNumber;
    public TextMeshProUGUI topNumber;
    public TextMeshProUGUI bottomNumber;
    public Button colorPlusButton;
    public Button colorMinusButton;
    public Button hairPlusButton;
    public Button hairMinusButton;
    public Button TopPlusButton;
    public Button TopRemoveButton;
    public Button BottomPlusButton;
    public Button BottomRemoveButton;

    private void Start()
    {
        colorPlusButton.onClick.AddListener(() => ChangeColor(true));
        colorMinusButton.onClick.AddListener(() => ChangeColor(false));
        hairPlusButton.onClick.AddListener(() => ChangeHair(true));
        hairMinusButton.onClick.AddListener(() => ChangeHair(false));
        TopPlusButton.onClick.AddListener(() => ChangeTop(true));
        TopRemoveButton.onClick.AddListener(() => ChangeTop(false));
        BottomPlusButton.onClick.AddListener(() => ChangeBottom(true));
        BottomRemoveButton.onClick.AddListener(() => ChangeBottom(false));
    }

    private void FixedUpdate()
    {
        colorNumber.text = "" + CharacterCreator.Instance.visualData.colorPreset;
        hairNumber.text = "" + CharacterCreator.Instance.visualData.hairID;
        topNumber.text = "" + CharacterCreator.Instance.visualData.topID;
        bottomNumber.text = "" + CharacterCreator.Instance.visualData.bottomID;


    }

    public void ChangeColor(bool add) {
        if (add)
            CharacterCreator.Instance.ColorPreset++;
        else
            CharacterCreator.Instance.ColorPreset--;
    }
    public void ChangeHair(bool add)
    {
        if (add)
            CharacterCreator.Instance.HairID++;
        else
            CharacterCreator.Instance.HairID--;
    }
    public void ChangeTop(bool add)
    {
        if (add)
            CharacterCreator.Instance.TopID++;
        else
            CharacterCreator.Instance.TopID--;
    }
    public void ChangeBottom(bool add)
    {
        if (add)
            CharacterCreator.Instance.BottomID++;
        else
            CharacterCreator.Instance.BottomID--;
    }
}
