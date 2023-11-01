using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGate : MonoBehaviour
{
    bool isUsed;
    public GameObject gate;
    private void OnTriggerEnter(Collider other)
    {
        if (!isUsed)
            if (other.gameObject.CompareTag("Player"))
            {
                isUsed = true;
                gate.SetActive(true);
            }
    }
}
