using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelGate : Interactable
{
    public int gateID;
    public MapNode destinationNode;
    public string nextSceneName;
    public GameObject levelGates;
    public TextMeshProUGUI levelName;
    public Image rewardPreviewImage;
    public Sprite skillSprite, itemSprite, potentialSprite, goldSprite;

    public override void Start()
    {
        rewardPreviewImage.gameObject.SetActive(false);
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

        if (LevelManager.Instance.currentMapNode.roomType == RoomTypes.Boss)
        {
            destinationNode = LevelManager.Instance.mapGenerator2.startNode;
        }

        if (destinationNode != null)
            nextSceneName = LevelManager.Instance.NextLevelName(destinationNode.roomType);


        if (destinationNode == null)
        {
            levelGates.SetActive(false);
            return;
        }
        if (LevelManager.Instance.currentRoomType == RoomTypes.Normal || LevelManager.Instance.currentRoomType == RoomTypes.Elite || LevelManager.Instance.currentRoomType == RoomTypes.Boss)
            levelGates.SetActive(false);

        SetupGate();
    }

    void SetupGate()
    {
        if (destinationNode != null)
            levelName.text = "" + destinationNode.roomType;

        if (destinationNode.roomType == RoomTypes.Normal || destinationNode.roomType == RoomTypes.Elite)
        {
            rewardPreviewImage.gameObject.SetActive(true);
            switch (destinationNode.rewardType)
            {
                case RewardType.Blessing:
                    rewardPreviewImage.sprite = skillSprite;
                    break;
                case RewardType.Item:
                    rewardPreviewImage.sprite = itemSprite;
                    break;
                case RewardType.Skill:
                    rewardPreviewImage.sprite = potentialSprite;
                    break;
                case RewardType.Gold:
                    rewardPreviewImage.sprite = goldSprite;
                    break;
                default:
                    break;
            }
        }
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
        if (destinationNode.roomType == RoomTypes.Staircase)
        {
            LevelManager.Instance.area++;
        }
        LevelManager.Instance.GoToNextLevel(destinationNode);
        TransitionManager.Instance.LoadScene(nextSceneName);
    }
}
