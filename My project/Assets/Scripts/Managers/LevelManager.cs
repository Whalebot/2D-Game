using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public RoomTypes currentRoomType = RoomTypes.Normal;
    public List<SceneSO> sceneObjects;
    public List<SceneSO> normalRoomLvl1;
    public List<SceneSO> eliteRoomLvl1;
    public List<SceneSO> bossRoomLvl1;
    public List<SceneSO> shopRoomLvl1;
    public List<SceneSO> treasureRoomLvl1;
    public List<SceneSO> eventRoomLvl1;
    public int currentLevel = 1;
    public int treasureChance = 10;
    public int shopChance = 20;
    public int eliteChance = 20;
    public MapNode room1;
    public MapNode room2;
    public MapNode room3;

    public event Action spawnLevelGates;
    public MapGenerator mapGenerator;
    public MapNode currentMapNode;

    private void Awake()
    {
        Instance = this;


        SaveManager.Instance.saveEvent += SaveData;
        SaveManager.Instance.startLoadEvent += LoadData;

        mapGenerator.SetupNodes();

        if (SaveManager.Instance.HasSaveData())
        {
            currentMapNode = mapGenerator.mapNodes[SaveManager.Instance.CurrentLevel];
        }
        else
        {
            currentMapNode = mapGenerator.startNode;
        }
        NextRoomType();

    }
    private void Start()
    {
        MarkVisitedRooms();
    }

    void MarkVisitedRooms()
    {

        for (int i = 0; i < mapGenerator.mapNodes.Count; i++)
        {
            foreach (var item in SaveManager.Instance.saveData.currrentCharacter.visitedRooms)
            {
                if (i == item)
                    mapGenerator.mapNodes[i].Visited();
            }

        }

    }

    public void GoToNextLevel(MapNode node)
    {
        int index = -1;
        for (int i = 0; i < mapGenerator.mapNodes.Count; i++)
        {
            if (mapGenerator.mapNodes[i] == node)
                index = i;
        }
        SaveManager.Instance.saveData.currrentCharacter.visitedRooms.Add(index);
        SaveManager.Instance.CurrentLevel = index;
    }

    void SaveData()
    {
        //SaveManager.Instance.CurrentLevel = currentLevel;
    }
    void LoadData()
    {
        currentLevel = SaveManager.Instance.CurrentLevel;
        NextRoomType();
    }

    public void SpawnLevelGates()
    {
        spawnLevelGates?.Invoke();
    }

    public string NextLevelName(RoomTypes roomType)
    {
        int RNG = 0;
        switch (roomType)
        {
            case RoomTypes.Normal:
                RNG = UnityEngine.Random.Range(0, normalRoomLvl1.Count);
                return normalRoomLvl1[RNG].sceneName;
            case RoomTypes.Elite:
                RNG = UnityEngine.Random.Range(0, eliteRoomLvl1.Count);
                return eliteRoomLvl1[RNG].sceneName;
            case RoomTypes.Boss:
                RNG = UnityEngine.Random.Range(0, bossRoomLvl1.Count);
                return bossRoomLvl1[RNG].sceneName;
            case RoomTypes.Treasure:
                RNG = UnityEngine.Random.Range(0, treasureRoomLvl1.Count);
                return treasureRoomLvl1[RNG].sceneName;
            case RoomTypes.Shop:
                RNG = UnityEngine.Random.Range(0, shopRoomLvl1.Count);
                return shopRoomLvl1[RNG].sceneName;
            case RoomTypes.Event:
                RNG = UnityEngine.Random.Range(0, eventRoomLvl1.Count);
                return eventRoomLvl1[RNG].sceneName;

            case RoomTypes.Disabled:
                return "";
            default:
                RNG = UnityEngine.Random.Range(0, normalRoomLvl1.Count);
                return normalRoomLvl1[RNG].sceneName;
        }
    }

    [Button]
    public void NextRoomType()
    {
        room1 = currentMapNode.GetExitNode(1);
        room2 = currentMapNode.GetExitNode(2);
        room3 = currentMapNode.GetExitNode(3);
    }
}

public enum RoomTypes
{
    Normal, Elite, Boss, Treasure, Shop, Event, Disabled
}