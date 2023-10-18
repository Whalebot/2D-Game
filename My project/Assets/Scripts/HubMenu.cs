using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class HubMenu : MonoBehaviour
{
    public Animator animator;
    public List<RuntimeAnimatorController> animatorControllers;
    public List<Character> characters;
    [TabGroup("Components")] public Image classImage;
    [TabGroup("Components")] public TextMeshProUGUI classText;
    [TabGroup("Components")] public TextMeshProUGUI colorNumber;
    [TabGroup("Components")] public TextMeshProUGUI hairNumber;
    [TabGroup("Components")] public TextMeshProUGUI topNumber;
    [TabGroup("Components")] public TextMeshProUGUI bottomNumber;
    [TabGroup("Components")] public Button colorPlusButton;
    [TabGroup("Components")] public Button colorMinusButton;
    [TabGroup("Components")] public Button hairPlusButton;
    [TabGroup("Components")] public Button hairMinusButton;
    [TabGroup("Components")] public Button TopPlusButton;
    [TabGroup("Components")] public Button TopRemoveButton;
    [TabGroup("Components")] public Button BottomPlusButton;
    [TabGroup("Components")] public Button BottomRemoveButton;
    [TabGroup("Components")] public Button classPlusButton;
    [TabGroup("Components")] public Button classRemoveButton;

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
        classPlusButton.onClick.AddListener(() => ChangeClass(true));
        classRemoveButton.onClick.AddListener(() => ChangeClass(false));
    }

    private void FixedUpdate()
    {
        classImage.sprite = characters[CharacterCreator.Instance.visualData.characterJob].image;
        classText.text = "" + characters[CharacterCreator.Instance.visualData.characterJob].characterName;
        colorNumber.text = "" + CharacterCreator.Instance.visualData.colorPreset;
        hairNumber.text = "" + CharacterCreator.Instance.visualData.hairID;
        topNumber.text = "" + CharacterCreator.Instance.visualData.topID;
        bottomNumber.text = "" + CharacterCreator.Instance.visualData.bottomID;
    }
    public void StartGame()
    {
        SaveManager.Instance.DeleteData();
        SaveManager.Instance.saveData.health = 50;
        
        TransitionManager.Instance.LoadScene(1);
    }
    public void ChangeClass(bool add)
    {
        if (add)
            CharacterCreator.Instance.Class++;
        else
            CharacterCreator.Instance.Class--;

        animator.runtimeAnimatorController = animatorControllers[CharacterCreator.Instance.Class];
    }
    public void ChangeColor(bool add)
    {
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
