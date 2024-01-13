using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DPSCheck : MonoBehaviour
{
    public Status damageTarget;
    public bool started;
    public bool finished;
    public TextMeshProUGUI text;
    public int duration;
    public int dmgCounted;
    public GameObject chestD, chestC, chestB, chestA, chestS;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        damageTarget.hurtEvent += StartCounter;
        damageTarget.damageEvent += CountDamage;
    }

    // Update is called once per frame
    void StartCounter()
    {
        started = true;
        text.gameObject.SetActive(true);
    }

    void CountDamage(int dmg)
    {
        if (started && !finished)
            dmgCounted += dmg;
    }

    void ExecuteFrame()
    {
        if (started && !finished)
        {
            duration--;
            text.text = "" + ((duration / 60) + 1);
            if (duration <= 0)
            {
                text.text = dmgCounted + " DAMAGE!";
                finished = true;
                damageTarget.gameObject.SetActive(false);
                GiveReward();
            }
        }
    }

    void GiveReward()
    {
        if (dmgCounted < 50) { chestD.SetActive(true); }
        else if (dmgCounted < 100) { chestC.SetActive(true); }
        else if (dmgCounted < 200) { chestB.SetActive(true); }
        else if (dmgCounted < 500) { chestA.SetActive(true); }
        else if (dmgCounted < 1000) { chestS.SetActive(true); }
    }
}
