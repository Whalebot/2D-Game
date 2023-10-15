using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class ButtonPrompt : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject defaultObject;
    public GameObject activeObject;
    public GameObject kbActiveObject;
    public ControlScheme scheme;
    public GameObject PS4;
    public GameObject XBOX;
    public GameObject keyboard;

    void Start()
    {
        ChangeInput();
    }

    [Button]
    public void SetupPrompt(Interactable.Type interactType)
    {
        if (Application.isPlaying) if (GameManager.menuOpen) return;
        UpdateUI();
       
        if (interactType == Interactable.Type.None)
        {
            ResetPrompt();
        }
        else
        {
            text.gameObject.SetActive(true);
            text.text = interactType.ToString();
            activeObject.SetActive(true);
            kbActiveObject.SetActive(true);
        }
        defaultObject.SetActive(true);

    }
    [Button]
    public void ResetPrompt()
    {
        defaultObject.SetActive(true);
        text.gameObject.SetActive(false);
        activeObject.SetActive(false);
        kbActiveObject.SetActive(false);
    }
    public void DisablePrompt()
    {
        defaultObject.SetActive(false);
        text.gameObject.SetActive(false);
        activeObject.SetActive(false);
        kbActiveObject.SetActive(false);
    }
    private void OnEnable()
    {
        InputManager.Instance.controlSchemeChange += ChangeInput;
        ChangeInput();
    }

    private void OnDisable()
    {
        InputManager.Instance.controlSchemeChange -= ChangeInput;
    }

    private void OnValidate()
    {
        UpdateUI();
    }

    void ChangeInput()
    {
        scheme = InputManager.controlScheme;
        UpdateUI();
    }

    void UpdateUI()
    {
        switch (scheme)
        {
            case ControlScheme.PS4:
                keyboard.SetActive(false);
                if (!PS4.activeSelf) PS4.SetActive(true);
                break;
            case ControlScheme.XBOX:
                keyboard.SetActive(false);
                if (!XBOX.activeSelf)
                    XBOX.SetActive(true); break;
            case ControlScheme.MouseAndKeyboard:
                PS4.SetActive(false);
                XBOX.SetActive(false);
                if (!keyboard.activeSelf) keyboard.SetActive(true);
                break;
            default: PS4.SetActive(true); break;
        }
    }
}
