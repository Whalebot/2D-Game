using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : Interactable
{

    public Rank rank;

    public GameObject treasure;

    public override void Start()
    {
        base.Start();
        AIManager.Instance.roomClearEvent += SpawnTreasure;
        if(LevelManager.Instance.currentRoomType != RoomTypes.Treasure)
        treasure.SetActive(false);
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
