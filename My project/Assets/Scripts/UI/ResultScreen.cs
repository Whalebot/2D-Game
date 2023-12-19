using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    public Button continueButton;

    public TextMeshProUGUI floorText;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI boosesKilledText;

    public OrganizeChildren skillPanel;

    // Start is called before the first frame update
    void Start()
    {
        continueButton.onClick.AddListener(() => ContinueButton());
    }

    public void SetupResultScreen()
    {
        floorText.text = "" + SaveManager.Instance.CurrentLevel;
    }

    public void ContinueButton()
    {
        GameManager.Instance.ReloadGame();
    }
}
