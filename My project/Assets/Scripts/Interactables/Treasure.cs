using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Treasure : Interactable
{

    public Rank rank;
    public RewardType reward;
    public bool autoSetup = true;
    public TextMeshProUGUI treasureText;
    public GameObject treasure;

    public override void Start()
    {
        base.Start();



        SetupText();
        AIManager.Instance.roomClearEvent += SpawnTreasure;
        if (LevelManager.Instance.currentRoomType != RoomTypes.Treasure && LevelManager.Instance.currentRoomType != RoomTypes.Event)
            treasure.SetActive(false);
    }

    void SetupText()
    {
        switch (reward)
        {
            case RewardType.Skill:
                treasureText.text = rank + " Rank Treasure";
                break;
            case RewardType.Item:
                treasureText.text = rank + " Rank Treasure";
                break;
            case RewardType.Potential:
                treasureText.text = rank + " Rank Treasure";
                break;
            case RewardType.Gold:
                treasureText.text = rank + " Gold";
                break;
            default:
                break;
        }

    }

    void AutoSetupTreasure()
    {
        if (LevelManager.Instance != null && autoSetup)
        {
            switch (LevelManager.Instance.currentMapNode.roomType)
            {
                case RoomTypes.Normal:
                    rank = Rank.C;
                    break;
                case RoomTypes.Elite:
                    rank = Rank.A;
                    break;
                case RoomTypes.Boss:
                    rank = Rank.S;
                    break;
                case RoomTypes.Treasure:
                    break;
                case RoomTypes.Shop:
                    break;
                case RoomTypes.Event:
                    break;
                case RoomTypes.Rest:
                    break;
                case RoomTypes.Disabled:
                    break;
                default:
                    break;
            }
        }
    }

    void SpawnTreasure()
    {
        if (treasure != null)
            treasure.SetActive(true);
    }

    public override void South()
    {
        base.South();
        GivePowerup();
    }
    public void GivePowerup()
    {
        switch (reward)
        {
            case RewardType.Skill:
                SkillManager.Instance.RollSkills(rank);
                GameManager.Instance.OpenGetSkillWindow();
                break;
            case RewardType.Item:
                SkillManager.Instance.RollSkills(rank);
                GameManager.Instance.OpenGetSkillWindow();
                break;
            case RewardType.Potential:
                SkillManager.Instance.RollSkills(rank);
                GameManager.Instance.OpenGetSkillWindow();
                break;
            case RewardType.Gold:
                GameManager.Instance.Gold += 150;
                break;
            default:
                break;
        }

        Destroy(gameObject);
    }
}
