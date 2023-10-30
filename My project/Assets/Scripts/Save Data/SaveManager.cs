using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public int seed;
    public bool autoLoad;
    public SaveData saveData;
    public event Action saveEvent;
    public event Action awakeLoadEvent;
    public event Action startLoadEvent;

    private void Awake()
    {
        Instance = this;
        if (autoLoad)
            LoadData();

        SetupRNG();
    }

    public void SetupRNG()
    {
        if (HasSaveData())
        {
            seed = saveData.currrentCharacter.rngSeed;
        }
        else
        {
            seed = (int)DateTime.Now.Ticks;
        }

        UnityEngine.Random.InitState(seed);
    }

    private void Start()
    {
        if (autoLoad && HasSaveData())
            startLoadEvent?.Invoke();
    }

    public CharacterVisualData VisualData
    {
        get
        {
            return saveData.currrentCharacter.visualData;
        }
        set
        {
            saveData.currrentCharacter.visualData = value;
        }
    }
    public List<SkillSO> LearnedSkills
    {
        get
        {
            return saveData.currrentCharacter.learnedSkills;
        }
        set
        {
            saveData.currrentCharacter.learnedSkills = value;
        }
    }
    public Stats Stats
    {
        get
        {
            return saveData.currrentCharacter.stats;
        }
        set
        {
            saveData.currrentCharacter.stats = value;
        }
    }
    public int CurrentLevel
    {
        get
        {
            return saveData.currrentCharacter.currentRoomID;
        }
        set
        {
            saveData.currrentCharacter.currentRoomID = value;
        }
    }

    public bool HasSaveData()
    {
        return (PlayerPrefs.HasKey("Save"));
    }
    [Button]
    public void DeleteData()
    {
        PlayerPrefs.DeleteKey("Save");
        SetupRNG();
    }

    public void LoadData()
    {

        if (PlayerPrefs.HasKey("Save"))
        {
            saveData = new SaveData();
            saveData.currrentCharacter = new CharacterData();
            saveData.currrentCharacter.visitedRooms = new List<int>();
            saveData.currrentCharacter.learnedSkills = new List<SkillSO>();
            saveData.currrentCharacter.visualData = new CharacterVisualData();

            string saveJson = PlayerPrefs.GetString("Save");
            saveData = JsonUtility.FromJson<SaveData>(saveJson);
            awakeLoadEvent?.Invoke();
        }
        //if (File.Exists(Application.persistentDataPath + "/saveData.json"))
        //{
        //    saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.persistentDataPath + "/saveData.json"));

    }

    public void SaveData()
    {
        saveEvent?.Invoke();
        saveData.currrentCharacter.rngSeed = seed;
        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/saveData.json", jsonData);
        PlayerPrefs.SetString("Save", jsonData);

        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class SaveData
{
    public CharacterData currrentCharacter;

    public List<CharacterData> oldCharacters;
}
[System.Serializable]
public class CharacterData
{
    public int rngSeed;
    public Stats stats;
    public int currentRoomID = 0;
    public List<int> visitedRooms;
    public CharacterVisualData visualData;
    public List<SkillSO> learnedSkills;
}

[System.Serializable]
public class CharacterVisualData
{
    [Range(0, 10)] public int characterJob;
    [Range(0, 10)] public int colorPreset;
    [Range(0, 10)] public int hairID;
    [Range(0, 10)] public int topID;
    [Range(0, 10)] public int bottomID;
    [Range(0, 10)] public int shoesID;
}