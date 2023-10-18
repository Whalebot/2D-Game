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
    public int currentLevel = 1;

    public event Action spawnLevelGates;

    private void Awake()
    {
        Instance = this;
        switch (TransitionManager.levelIndex - 1)
        {
            case 0: currentRoomType = RoomTypes.Normal; break;
            case 1: currentRoomType = RoomTypes.Treasure; break;
            case 2: currentRoomType = RoomTypes.Boss; break;
            case 3: currentRoomType = RoomTypes.Shop; break;
            default:
                break;
        }

        SaveManager.Instance.saveEvent += SaveData;
        SaveManager.Instance.loadEvent += LoadData;
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
        if ((int)roomType >= sceneObjects.Count) return "";
        return sceneObjects[(int)roomType].sceneName;
        //switch (roomType)
        //{
        //    case RoomTypes.Normal:
        //        return sceneObjects[(int)roomType].sceneName;
        //    case RoomTypes.Boss:
        //        return 2;
        //    case RoomTypes.Treasure:
        //        return 1;
        //    case RoomTypes.Shop:
        //        return 3;
        //    case RoomTypes.Disabled:
        //        return 0;
        //    default:
        //        return 0;
        //}
    }

    [Button]
    public RoomTypes NextRoomType(int gateID = 0)
    {
        switch (gateID)
        {
            case 0:
                if (currentLevel % 4 == 0)
                {
                    return RoomTypes.Boss;
                }
                else
                    return RoomTypes.Normal;
            case 1:
                if (currentLevel % 4 == 0)
                {
                    return RoomTypes.Disabled;
                }
                else
                    return RoomTypes.Shop;
            case 2:
                if (currentLevel % 4 == 0)
                {
                    return RoomTypes.Disabled;
                }
                else
                    return RoomTypes.Treasure;
            default:
                return RoomTypes.Normal;
        }
    }
}

public enum RoomTypes
{
    Normal, Boss, Treasure, Shop, Disabled
}