using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : Interactable
{
    public Rank rank;

    public SkillSO skill;

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

        GameManager.Instance.OpenGetSkillWindow();
        Destroy(gameObject);
    }
}
