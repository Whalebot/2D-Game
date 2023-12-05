using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public RoomTypes currentRoomType = RoomTypes.Normal;

    public List<SceneSO> normalRoomLvl1;
    public List<SceneSO> eliteRoomLvl1;
    public List<SceneSO> bossRoomLvl1;
    public List<SceneSO> shopRoomLvl1;
    public List<SceneSO> treasureRoomLvl1;
    public List<SceneSO> eventRoomLvl1;
    public List<SceneSO> restRoomLvl1;
    public List<SceneSO> stairwayRoom;

    public List<SceneSO> normalRoomLvl2;
    public List<SceneSO> eliteRoomLvl2;
    public List<SceneSO> bossRoomLvl2;
    public List<SceneSO> treasureRoomLvl2;
    public List<SceneSO> eventRoomLvl2;

    public int area = 1;
    public int currentLevel = 1;
    public int treasureChance = 10;
    public int shopChance = 20;
    public int eliteChance = 20;
    public MapNode room1;
    public MapNode room2;
    public MapNode room3;

    public event Action spawnLevelGates;
    public List<MapNode> allNodes;
    public MapGenerator mapGenerator;
    public MapGenerator mapGenerator2;
    public MapGenerator mapGenerator3;
    public MapNode currentMapNode;

    private void Awake()
    {
        Instance = this;


        SaveManager.Instance.saveEvent += SaveData;
        SaveManager.Instance.startLoadEvent += LoadData;
        area = SaveManager.Instance.saveData.currrentCharacter.currentArea;

        mapGenerator.SetupNodes();
        mapGenerator2.SetupNodes();
        mapGenerator3.SetupNodes();

        foreach (var item in mapGenerator.mapNodes)
        {
            allNodes.Add(item);
        }
        foreach (var item in mapGenerator2.mapNodes)
        {
            allNodes.Add(item);
        }
        foreach (var item in mapGenerator3.mapNodes)
        {
            allNodes.Add(item);
        }
        if (SaveManager.Instance.HasSaveData())
        {
            currentMapNode = allNodes[SaveManager.Instance.CurrentLevel];
            currentRoomType = currentMapNode.roomType;
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

        for (int i = 0; i < allNodes.Count; i++)
        {
            foreach (var item in SaveManager.Instance.saveData.currrentCharacter.visitedRooms)
            {
                if (i == item)
                    allNodes[i].Visited();
            }
        }
    }

    public void GoToNextLevel(MapNode node)
    {
        int index = -1;
        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i] == node)
                index = i;
        }
        SaveManager.Instance.saveData.currrentCharacter.visitedRooms.Add(index);
        SaveManager.Instance.saveData.currrentCharacter.currentArea = area;
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

                if (area == 2)
                {
                    RNG = UnityEngine.Random.Range(0, normalRoomLvl2.Count);

                    return normalRoomLvl2[RNG].sceneName;
                }
                else
                {
                    RNG = UnityEngine.Random.Range(0, normalRoomLvl1.Count);
 

                    return normalRoomLvl1[RNG].sceneName;
                }
            case RoomTypes.Elite:
                if (area == 2)
                {
                    RNG = UnityEngine.Random.Range(0, eliteRoomLvl2.Count);
                    return eliteRoomLvl2[RNG].sceneName;
                }
                else
                {
                    RNG = UnityEngine.Random.Range(0, eliteRoomLvl1.Count);
                    return eliteRoomLvl1[RNG].sceneName;
                }
            case RoomTypes.Boss:
                if (area == 2)
                {
                    RNG = UnityEngine.Random.Range(0, bossRoomLvl2.Count);
                    if (currentMapNode.roomType == RoomTypes.Boss)
                    {
                        return stairwayRoom[0].sceneName;
                    }

                    return bossRoomLvl2[RNG].sceneName;
                }
                else
                {
                    RNG = UnityEngine.Random.Range(0, bossRoomLvl1.Count);
                    if (currentMapNode.roomType == RoomTypes.Boss)
                    {
                        return stairwayRoom[0].sceneName;
                    }

                    return bossRoomLvl1[RNG].sceneName;
                }
              
            case RoomTypes.Treasure:
                if (area == 2)
                {
                    RNG = UnityEngine.Random.Range(0, treasureRoomLvl2.Count);
                    return treasureRoomLvl2[RNG].sceneName;
                }
                else
                {
                    RNG = UnityEngine.Random.Range(0, treasureRoomLvl1.Count);
                    return treasureRoomLvl1[RNG].sceneName;
                }
            case RoomTypes.Shop:
                RNG = UnityEngine.Random.Range(0, shopRoomLvl1.Count);
                return shopRoomLvl1[RNG].sceneName;
            case RoomTypes.Event:
                if (area == 2)
                {
                    RNG = UnityEngine.Random.Range(0, eventRoomLvl2.Count);
                    return eventRoomLvl2[RNG].sceneName;
                }
                else
                {
                    RNG = UnityEngine.Random.Range(0, eventRoomLvl1.Count);
                    Debug.Log(eventRoomLvl1[RNG].sceneName);
                    return eventRoomLvl1[RNG].sceneName;
                }
            case RoomTypes.Rest:
                RNG = UnityEngine.Random.Range(0, restRoomLvl1.Count);
                return restRoomLvl1[RNG].sceneName;
            case RoomTypes.Staircase:
                return stairwayRoom[0].sceneName;
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
    Normal, Elite, Boss, Treasure, Shop, Event, Rest, Staircase, Disabled
}