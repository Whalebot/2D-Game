using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelGate : Interactable
{
    public int gateID;
    public MapNode destinationNode;
    public string nextSceneName;
    public GameObject levelGates;
    public TextMeshProUGUI levelName;

    public override void Start()
    {
        LevelManager.Instance.spawnLevelGates += SpawnLevelGates;
        switch (gateID)
        {
            case 0:
                destinationNode = LevelManager.Instance.room1;
                break;
            case 1:
                destinationNode = LevelManager.Instance.room2;
                break;
            case 2:
                destinationNode = LevelManager.Instance.room3;
                break;
            default:
                destinationNode = LevelManager.Instance.room1;
                break;
        }

        if (destinationNode != null)
            nextSceneName = LevelManager.Instance.NextLevelName(destinationNode.roomType);

        SetupGate();
        if (destinationNode == null)
        {
            levelGates.SetActive(false);
            return;
        }
        if (LevelManager.Instance.currentRoomType != RoomTypes.Shop && LevelManager.Instance.currentRoomType != RoomTypes.Treasure)
            levelGates.SetActive(false);
    }

    void SetupGate()
    {
        if (destinationNode != null)
            levelName.text = "" + destinationNode.roomType;
    }

    void SpawnLevelGates()
    {
        if (destinationNode != null)
            if (destinationNode.roomType != RoomTypes.Disabled)
                levelGates.SetActive(true);
    }

    public override void South()
    {
        base.South();
        EnterGate();
    }

    public void EnterGate()
    {
        LevelManager.Instance.GoToNextLevel(destinationNode);
        TransitionManager.Instance.LoadScene(nextSceneName);
    }
}
