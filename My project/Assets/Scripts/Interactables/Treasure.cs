using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Treasure : Interactable
{

    public Rank rank;
    public RewardType reward;
    public bool autoSetup = true;
    public bool canInteract = true;
    public TextMeshProUGUI treasureText;
    public GameObject treasure;
    public GameObject woodChest, silverChest, goldChest;
    public SFX treasureSFX;
    Status status;

    public override void Start()
    {
        status = GetComponent<Status>();

        status.deathEvent += GivePowerup;
        base.Start();

        AutoSetupTreasure();

        SetupText();
        AIManager.Instance.roomClearEvent += SpawnTreasure;
        if (LevelManager.Instance.currentRoomType != RoomTypes.Treasure && LevelManager.Instance.currentRoomType != RoomTypes.Event || AIManager.Instance.HasEnemies())
            treasure.SetActive(false);
    }

    void SetupText()
    {
        switch (reward)
        {
            case RewardType.Blessing:
                treasureText.text = rank + " Rank " + reward + " Treasure";
                break;
            case RewardType.Item:
                treasureText.text = rank + " Rank " + reward + " Treasure";
                break;
            case RewardType.Skill:
                treasureText.text = rank + " Rank " + reward + " Treasure";
                break;
            case RewardType.Gold:
                treasureText.text = rank + " Rank Gold";
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
                    reward = LevelManager.Instance.currentMapNode.rewardType;
                    rank = Rank.C;
                    break;
                case RoomTypes.Elite:
                    reward = LevelManager.Instance.currentMapNode.rewardType;
                    rank = Rank.A;
                    break;
                case RoomTypes.Boss:
                    reward = LevelManager.Instance.currentMapNode.rewardType;
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

            SetupChest();
        }
    }

    void SetupChest()
    {
        if (autoSetup)
        {
            woodChest.SetActive(false);
            silverChest.SetActive(false);
            goldChest.SetActive(false);

            switch (rank)
            {
                case Rank.D:
                    woodChest.SetActive(true);
                    break;
                case Rank.C:
                    woodChest.SetActive(true);
                    break;
                case Rank.B:
                    silverChest.SetActive(true);
                    break;
                case Rank.A:
                    silverChest.SetActive(true);
                    break;
                case Rank.S:
                    goldChest.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnValidate()
    {
        SetupChest();
    }

    void SpawnTreasure()
    {
        if (treasure != null)
            treasure.SetActive(true);
    }
    public override bool CanInteract()
    {
        return canInteract;
    }
    public override void South()
    {
        base.South();
        GivePowerup();
    }
    public void GivePowerup()
    {
        AudioManager.Instance.PlaySFX(treasureSFX, transform.position);
        switch (reward)
        {
            case RewardType.Blessing:
                SkillManager.Instance.RollBlessing(rank);
                GameManager.Instance.OpenGetSkillWindow(RewardType.Blessing);
                break;
            case RewardType.Item:
                SkillManager.Instance.RollItem(rank);
                GameManager.Instance.OpenGetSkillWindow(RewardType.Item);
                break;
            case RewardType.Skill:
                SkillManager.Instance.RollActiveSkill(rank);
                GameManager.Instance.OpenGetSkillWindow(RewardType.Skill);
                break;
            case RewardType.Gold:
                GameManager.Instance.Gold += 150;
                LevelManager.Instance.SpawnLevelGates();
                break;
            default:
                break;
        }

        Destroy(gameObject);
    }
}
