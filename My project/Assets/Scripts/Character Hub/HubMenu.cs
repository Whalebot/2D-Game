using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class HubMenu : MonoBehaviour
{
    public Animator animator;
    public Animator faceAnimator;
    public Animator cameraPan;
    public Animator tabAnimator;
    [TabGroup("Components")] public Button tabToggleButton;
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
        tabToggleButton.onClick.AddListener(() => ToggleTab());

        colorPlusButton.onClick.AddListener(() => ChangeColor(true));
        colorMinusButton.onClick.AddListener(() => ChangeColor(false));

        hairPlusButton.onClick.AddListener(() => ChangeHair(true));
        hairMinusButton.onClick.AddListener(() => ChangeHair(false));

        topPlusButton.onClick.AddListener(() => ChangeTop(true));
        topMinusButton.onClick.AddListener(() => ChangeTop(false));
        bottomPlusButton.onClick.AddListener(() => ChangeBottom(true));
        bottomMinusButton.onClick.AddListener(() => ChangeBottom(false));

        shoesPlusButton.onClick.AddListener(() => ChangeShoes(true));
        shoesMinusButton.onClick.AddListener(() => ChangeShoes(false));

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
    void ToggleTab() {
        tabAnimator.SetBool("Open", !tabAnimator.GetBool("Open"));    }

    public void StartGame()
    {
        SaveManager.Instance.SetupCharacter();
        SaveManager.Instance.Stats.ReplaceStats(CharacterCreator.Instance.characters[CharacterCreator.Instance.Class].stats);
        cameraPan.SetInteger("TransitionID", CharacterCreator.Instance.Class);
        animator.SetInteger("ID", CharacterCreator.Instance.Class);
        cameraPan.gameObject.SetActive(true);
        animator.SetBool("Cutscene", true);
        faceAnimator.SetTrigger("Emote");
        StartCoroutine(FadeOutCanvas());
        TransitionManager.Instance.LoadScene(1);
    }
    IEnumerator FadeOutCanvas()
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        while (group.alpha > 0)
        {
            group.alpha -= 0.01F;
            yield return new WaitForEndOfFrame();
        }
        Canvas canvas = GetComponent<Canvas>();
        canvas.enabled = false;
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
            CharacterCreator.Instance.ShoesID++;
        else
            CharacterCreator.Instance.ShoesID--;
    }
}
