using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : Interactable
{
    public SkillSO skill;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GivePowerup();
    }

    public void GivePowerup()
    {

        GameManager.Instance.OpenGetSkillWindow();
        Destroy(gameObject);
    }
}
