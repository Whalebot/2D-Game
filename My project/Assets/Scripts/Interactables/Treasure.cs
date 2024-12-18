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
    public bool open = false;
    public TextMeshProUGUI treasureText;
    public GameObject treasure;
    public Transform visualContainer;
    public GameObject woodChest, silverChest, goldChest, shrine;
    public SFX treasureSFX;
    public float treasureDelayOpen = 1f;
    protected Status status;

    public override void Start()
    {
        status = GetComponent<Status>();

        status.deathEvent += OpenChest;
        base.Start();

        AutoSetupTreasure();
        SetupVisuals();

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
                    reward = RewardType.Item;
                    rank = Rank.S;
                    break;
                case RoomTypes.Treasure:
                    reward = RewardType.Item;
                    rank = Rank.A;
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

    void SetupVisuals()
    {


        visualContainer.localScale = new Vector3(Mathf.Sign(transform.eulerAngles.y) * Mathf.Abs(visualContainer.localScale.x), visualContainer.localScale.y, visualContainer.localScale.z);
        float angle = visualContainer.localEulerAngles.y;
        angle = (angle > 180) ? angle - 360 : angle;
        visualContainer.localRotation = Quaternion.Euler(0, Mathf.Sign(transform.rotation.y) * Mathf.Abs(visualContainer.localEulerAngles.y), 0);


        woodChest.SetActive(false);
        silverChest.SetActive(false);
        goldChest.SetActive(false);
        shrine.SetActive(false);

        if (reward == RewardType.Blessing)
        {
            shrine.SetActive(true);
        }
        else
        {
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
        SetupVisuals();
    }

    void SpawnTreasure()
    {
        if (treasure != null)
            treasure.SetActive(true);
    }
    public override bool CanInteract()
    {
        return canInteract && !open;
    }
    public override void South()
    {
        base.South();
        OpenChest();
    }

    void OpenChest()
    {
        StartCoroutine(DelayOpenChest());
    }

    IEnumerator DelayOpenChest()
    {
        open = true;
        yield return new WaitForSeconds(treasureDelayOpen);
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
                GameManager.Instance.Gold += 50 * (int)rank;
                LevelManager.Instance.SpawnLevelGates();
                break;
            default:
                break;
        }

        Destroy(gameObject);
    }
}
