using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class HubMenu : MonoBehaviour
{
    public Animator animator;
    [TabGroup("Components")] public Image classImage;
    [TabGroup("Components")] public TextMeshProUGUI classText;
    [TabGroup("Components")] public TextMeshProUGUI colorNumber;
    [TabGroup("Components")] public TextMeshProUGUI hairNumber;
    [TabGroup("Components")] public TextMeshProUGUI topNumber;
    [TabGroup("Components")] public TextMeshProUGUI bottomNumber;
    [TabGroup("Components")] public TextMeshProUGUI shoesNumber;
    [TabGroup("Components")] public TextMeshProUGUI strengthStat;
    [TabGroup("Components")] public TextMeshProUGUI agilityStat;
    [TabGroup("Components")] public TextMeshProUGUI intStat;
    [TabGroup("Components")] public TextMeshProUGUI luckStat;
    [TabGroup("Components")] public TextMeshProUGUI gold;
    [TabGroup("Components")] public Button colorPlusButton;
    [TabGroup("Components")] public Button colorMinusButton;
    [TabGroup("Components")] public Button hairPlusButton;
    [TabGroup("Components")] public Button hairMinusButton;
    [TabGroup("Components")] public Button topPlusButton;
    [TabGroup("Components")] public Button topMinusButton;
    [TabGroup("Components")] public Button bottomPlusButton;
    [TabGroup("Components")] public Button bottomMinusButton;
    [TabGroup("Components")] public Button shoesPlusButton;
    [TabGroup("Components")] public Button shoesMinusButton;
    [TabGroup("Components")] public Button classPlusButton;
    [TabGroup("Components")] public Button classMinusButton;

    private void Start()
    {
        colorPlusButton.onClick.AddListener(() => ChangeColor(true));
        colorMinusButton.onClick.AddListener(() => ChangeColor(false));
        hairPlusButton.onClick.AddListener(() => ChangeHair(true));
        hairMinusButton.onClick.AddListener(() => ChangeHair(false));
        topPlusButton.onClick.AddListener(() => ChangeTop(true));
        topMinusButton.onClick.AddListener(() => ChangeTop(false));
        bottomPlusButton.onClick.AddListener(() => ChangeBottom(true));
        bottomMinusButton.onClick.AddListener(() => ChangeBottom(false));
        bottomPlusButton.onClick.AddListener(() => ChangeBottom(true));
        bottomMinusButton.onClick.AddListener(() => ChangeBottom(false));
        classPlusButton.onClick.AddListener(() => ChangeClass(true));
        classMinusButton.onClick.AddListener(() => ChangeClass(false));

        animator.runtimeAnimatorController = CharacterCreator.Instance.characters[CharacterCreator.Instance.Class].controller;
    }

    private void FixedUpdate()
    {
        classImage.sprite = CharacterCreator.Instance.characters[CharacterCreator.Instance.visualData.characterJob].image;
        strengthStat.text = "" + CharacterCreator.Instance.characters[CharacterCreator.Instance.visualData.characterJob].stats.strength;
        agilityStat.text = "" + CharacterCreator.Instance.characters[CharacterCreator.Instance.visualData.characterJob].stats.agility;
        intStat.text = "" + CharacterCreator.Instance.characters[CharacterCreator.Instance.visualData.characterJob].stats.intelligence;
        luckStat.text = "" + CharacterCreator.Instance.characters[CharacterCreator.Instance.visualData.characterJob].stats.luck;
        gold.text = "" + CharacterCreator.Instance.characters[CharacterCreator.Instance.visualData.characterJob].stats.gold;

        classText.text = "" + CharacterCreator.Instance.characters[CharacterCreator.Instance.visualData.characterJob].characterName;
        colorNumber.text = "" + CharacterCreator.Instance.visualData.colorPreset;
        hairNumber.text = "" + CharacterCreator.Instance.visualData.hairID;
        topNumber.text = "" + CharacterCreator.Instance.visualData.topID;
        bottomNumber.text = "" + CharacterCreator.Instance.visualData.bottomID;
        shoesNumber.text = "" + CharacterCreator.Instance.visualData.shoesID;
    }
    public void StartGame()
    {
        SaveManager.Instance.SetupCharacter();
        SaveManager.Instance.Stats.ReplaceStats(CharacterCreator.Instance.characters[CharacterCreator.Instance.Class].stats);

        TransitionManager.Instance.LoadScene(1);
    }
    public void ChangeClass(bool add)
    {
        if (add)
            CharacterCreator.Instance.Class++;
        else
            CharacterCreator.Instance.Class--;

        animator.runtimeAnimatorController = CharacterCreator.Instance.characters[CharacterCreator.Instance.Class].controller;
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
    public void ChangeShoes(bool add)
    {
        if (add)
            CharacterCreator.Instance.BottomID++;
        else
            CharacterCreator.Instance.BottomID--;
    }
}
