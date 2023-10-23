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
    public int currentLevel = 1;
    public int treasureChance = 10;
    public int shopChance = 20;
    public int eliteChance = 20;
    public RoomTypes room1;
    public RoomTypes room2;
    public RoomTypes room3;

    public event Action spawnLevelGates;

    private void Awake()
    {
        Instance = this;
        //switch (TransitionManager.levelIndex - 1)
        //{
        //    case 0: currentRoomType = RoomTypes.Normal; break;
        //    case 1: currentRoomType = RoomTypes.Treasure; break;
        //    case 2: currentRoomType = RoomTypes.Boss; break;
        //    case 3: currentRoomType = RoomTypes.Shop; break;
        //    default:
        //        break;
        //}

        SaveManager.Instance.saveEvent += SaveData;
        SaveManager.Instance.startLoadEvent += LoadData;
    }
    private void Start()
    {

    }

    void SaveData()
    {
        SaveManager.Instance.saveData.currentLevel = currentLevel;
    }
    void LoadData()
    {
        currentLevel = SaveManager.Instance.saveData.currentLevel;
    }
    public void SpawnLevelGates()
    {
        spawnLevelGates?.Invoke();
    }
    public void GoToNextLevel()
    {
        currentLevel++;
    }

    public string NextLevelName(RoomTypes roomType)
    {
        //if (roomType == RoomTypes.Boss)
        //{
        //    if (currentLevel > 6)
        //        return sceneObjects[sceneObjects.Count - 1].sceneName;
        //}
        //if ((int)roomType >= sceneObjects.Count) return "";

        //return sceneObjects[(int)roomType].sceneName;
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


            case RoomTypes.Disabled:
                return "";
            default:
                return "";
        }
    }

    [Button]
    public void NextRoomType()
    {
        //Next level is a boss
        if (currentLevel % 4 == 0)
        {
            room1 = RoomTypes.Boss;
            room2 = RoomTypes.Disabled;
            room3 = RoomTypes.Disabled;
            return;
        }

        room1 = RollRoomType();
        room2 = RollRoomType();
        room3 = RollRoomType();
    }

    RoomTypes RollRoomType()
    {
        int RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= treasureChance)
            return RoomTypes.Treasure;

        RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= eliteChance)

            return RoomTypes.Elite;
        RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= shopChance)
            return RoomTypes.Shop;

        return RoomTypes.Normal;
    }
}

public enum RoomTypes
{
    Normal, Elite, Boss, Treasure, Shop, Disabled
}