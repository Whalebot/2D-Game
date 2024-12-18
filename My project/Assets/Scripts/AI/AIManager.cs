﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    public int enemyCount;
    public int wave;
    public bool encounterFinished;
    public SFX roomClearSFX;


    public static float aimOffset = 1F;


    public GameObject clearSFX;
    public event Action noEnemiesRoom;
    public event Action<Character> killEvent;

    public event Action allEnemiesKilledEvent;
    public event Action<int> waveFinishEvent;
    public event Action roomClearEvent;

    public List<EnemySummoner> wave1;
    public List<AIEnemy> respawningEnemies;
    public List<AIEnemy> allEnemies;
    public List<AIEnemy> activeEnemies;


    public bool combatEncounter;
    private void Awake()
    {

        Instance = this;
        allEnemies = new List<AIEnemy>();


    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (activeEnemies.Count > 0 && !combatEncounter)
        {
            combatEncounter = true;
            AudioManager.Instance.GoToState(AudioManager.AudioState.Battle);
        }
        else if (activeEnemies.Count <= 0 && combatEncounter)
        {
            combatEncounter = false;
            AudioManager.Instance.GoToState(AudioManager.AudioState.Default);
        }

        enemyCount = allEnemies.Count;

    }

    public int EnemyCount()
    {
        return allEnemies.Count;
    }

    public bool HasEnemies()
    {
        return allEnemies.Count > 0;
    }

    public void EnemyKilled(AIEnemy temp)
    {
        if (allEnemies.Contains(temp))
            allEnemies.Remove(temp);

        if (activeEnemies.Contains(temp))
            activeEnemies.Remove(temp);

        killEvent?.Invoke(temp.character);

        if (allEnemies.Count == 0)
            AllEnemiesKilled();
    }

    public void KillAllEnemies()
    {

        int count = allEnemies.Count;
        for (int i = 0; i < count; i++)
        {
            //print(count + " " + AllEnemies[i]);
            allEnemies[0].GetComponent<Status>().Health = 0;
            //Destroy(AllEnemies[0].gameObject);
        }

    }

    void EmptyRoom()
    {
        encounterFinished = true;
        noEnemiesRoom?.Invoke();
    }

    public void AllEnemiesKilled()
    {
        if (wave1.Count > 0)
        {
            wave++;
            waveFinishEvent?.Invoke(wave);
            //Spawn Wave
            allEnemiesKilledEvent?.Invoke();
        }
        else
        {
            //if (combatEncounter)
            //    Instantiate(clearSFX);
            encounterFinished = true;
            AudioManager.Instance.PlaySFX(roomClearSFX);
            roomClearEvent?.Invoke();
        }
    }
}
