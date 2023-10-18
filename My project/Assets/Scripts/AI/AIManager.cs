using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    public int enemyCount;
    public bool encounterFinished;

    public static float aimOffset = 1F;

    public event Action allEnemiesKilled;
    public GameObject clearSFX;
    public event Action noEnemiesRoom;
    public event Action<Character> killEvent;

    public List<AI> respawningEnemies;
    public List<AI> allEnemies;
    public List<AI> activeEnemies;


    public bool combatEncounter;
    private void Awake()
    {

        Instance = this;
        allEnemies = new List<AI>();


    }
    // Start is called before the first frame update
    void Start()
    {

    }

    [Button]
    public void GetAllAI()
    {
        AI[] temp = FindObjectsOfType<AI>();
        respawningEnemies = new List<AI>(temp);
        for (int i = 0; i < respawningEnemies.Count; i++)
        {
            respawningEnemies[i].id = respawningEnemies.Count - i - 1;
        }
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

    public void EnemyKilled(AI temp) {
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
        //if (combatEncounter)
        //    Instantiate(clearSFX);
        encounterFinished = true;
        allEnemiesKilled?.Invoke();
    }
}
