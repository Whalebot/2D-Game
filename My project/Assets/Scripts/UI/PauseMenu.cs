using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public StatDisplay statWindow;
    public Button continueButton;
    public Button restartButton;
    public Button quitButton;

    private void Awake()
    {

        continueButton.onClick.AddListener(() => Continue());
        restartButton.onClick.AddListener(() => Restart());
        quitButton.onClick.AddListener(() => Quit());
    }

    private void OnEnable()
    {
        mapGenerator.SetupNodes();
        MarkVisitedRooms();
        UIManager.Instance.SetActiveEventSystem(continueButton.gameObject);
    }

    void Continue()
    {
        GameManager.Instance.CloseMenu();
    }
    void Restart()
    {
        GameManager.Instance.LoseGame();
    }
    void Quit()
    {

    }

    void MarkVisitedRooms()
    {

        for (int i = 0; i < mapGenerator.mapNodes.Count; i++)
        {
            foreach (var item in SaveManager.Instance.saveData.currrentCharacter.visitedRooms)
            {
                if (i == item)
                    mapGenerator.mapNodes[i].Visited();
            }

        }

    }
}
