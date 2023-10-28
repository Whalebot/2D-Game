using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySummoner : MonoBehaviour
{
    public int summonTime = 60;
    int summonCounter;
    public bool isSummoning;
    public GameObject enemyPrefab;
    public GameObject vfx;

    private void Awake()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
    }

    private void Start()
    {
        AddToWave();
    }

    void ExecuteFrame()
    {
        if (isSummoning)
        {
            summonCounter++;
            if (summonCounter >= summonTime)
            {

                SummonEnemy();
            }
        }
    }

    void AddToWave()
    {
        AIManager.Instance.wave1.Add(this);
        AIManager.Instance.allEnemiesKilledEvent += StartSummon;
    }

    void StartSummon()
    {
        isSummoning = true;
        vfx.SetActive(true);
        AIManager.Instance.allEnemiesKilledEvent -= StartSummon;
    }

    void SummonEnemy()
    {
        isSummoning = false;
        Instantiate(enemyPrefab, transform.position, transform.rotation);
        AIManager.Instance.wave1.Remove(this);

        GameManager.Instance.advanceGameState -= ExecuteFrame;
        //Destroy(this);
    }
}
