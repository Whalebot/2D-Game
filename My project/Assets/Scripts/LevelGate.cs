using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelGate : Interactable
{
    public int gateID;
    public RoomTypes roomType;
    public int levelScene;
    public GameObject levelGates;
    public TextMeshProUGUI levelName;

    public override void Start()
    {
        LevelManager.Instance.spawnLevelGates += SpawnLevelGates;
        roomType = LevelManager.Instance.NextRoomType(gateID);
        levelScene = LevelManager.Instance.NextLevelIndex(roomType);
        SetupGate();

        if (LevelManager.Instance.currentRoomType != RoomTypes.Shop || roomType == RoomTypes.Disabled)
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
        TransitionManager.Instance.LoadScene(levelScene);
    }
}
