using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Treasure : Interactable
{

    public Rank rank;
    public bool autoSetup = true;
    public TextMeshProUGUI treasureText;
    public GameObject treasure;

    public override void Start()
    {
        base.Start();
        treasureText.text = rank + " Rank Treasure";
        AIManager.Instance.roomClearEvent += SpawnTreasure;
        if (LevelManager.Instance.currentRoomType != RoomTypes.Treasure && LevelManager.Instance.currentRoomType != RoomTypes.Event)
            treasure.SetActive(false);
    }

    void AutoSetupTreasure() {
        if (LevelManager.Instance != null && autoSetup) {
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //        GivePowerup();
    //}
    public override void South()
    {
        base.South();
        GivePowerup();
    }
    public void GivePowerup()
    {
        SkillManager.Instance.RollSkills(rank);
        GameManager.Instance.OpenGetSkillWindow();
        Destroy(gameObject);
    }
}
