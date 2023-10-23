using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelGate : Interactable
{
    public int gateID;
    public RoomTypes roomType;
    public string nextSceneName;
    public GameObject levelGates;
    public TextMeshProUGUI levelName;

    public override void Start()
    {
        LevelManager.Instance.spawnLevelGates += SpawnLevelGates;
        switch (gateID)
        {
            case 0:
                roomType = LevelManager.Instance.room1;
                break;
            case 1:
                roomType = LevelManager.Instance.room2;
                break;
            case 2:
                roomType = LevelManager.Instance.room3;
                break;
            default:
                roomType = LevelManager.Instance.room1;
                break;
        }

      
        nextSceneName = LevelManager.Instance.NextLevelName(roomType);
        SetupGate();

        if (LevelManager.Instance.currentRoomType != RoomTypes.Shop && LevelManager.Instance.currentRoomType != RoomTypes.Treasure|| roomType == RoomTypes.Disabled)
            levelGates.SetActive(false);
    }

    void SetupGate()
    {
        levelName.text = "" + roomType;
    }

    void SpawnLevelGates()
    {
        if (roomType != RoomTypes.Disabled)
            levelGates.SetActive(true);
    }

    public override void South()
    {
        base.South();
        EnterGate();
    }

    public void EnterGate()
    {
        LevelManager.Instance.GoToNextLevel();
        TransitionManager.Instance.LoadScene(nextSceneName);
    }
}
