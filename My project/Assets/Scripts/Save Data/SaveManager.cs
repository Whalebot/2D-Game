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

        LoadRNG();
    }

    void LoadRNG()
    {
        if (HasCurrentCharacter())
        {
            seed = saveData.currrentCharacter.rngSeed;
            UnityEngine.Random.InitState(seed);
        }
        else SetupRNG();
    }
    public void SetupRNG()
    {
        seed = (int)DateTime.Now.Ticks;
        UnityEngine.Random.InitState(seed);
    }

    private void Start()
    {
        if (autoLoad && HasSaveData())
            startLoadEvent?.Invoke();
    }
    public CharacterData CurrentData
    {
        get
        {
            return saveData.currrentCharacter;
        }
        set
        {
            saveData.currrentCharacter = value;
        }

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
    public List<SkillSO> FoundSkills
    {
        get
        {
            return saveData.currrentCharacter.foundSkills;
        }
        set
        {
            saveData.currrentCharacter.foundSkills = value;
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
    public bool HasCurrentCharacter()
    {
        return saveData.currrentCharacter.rngSeed != 0;
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
    public void SetupCharacter()
    {

        saveData.currrentCharacter = new CharacterData();
        saveData.currrentCharacter.visitedRooms = new List<int>();
        saveData.currrentCharacter.learnedSkills = new List<SkillSO>();
        saveData.currrentCharacter.visualData = new CharacterVisualData();
        saveData.currrentCharacter.stats = new Stats();

        SetupRNG();
    }
    [Button]
    public void KillCharacter()
    {
        saveData.oldCharacters.Add(saveData.currrentCharacter);
        saveData.currrentCharacter = new CharacterData();

        saveData.currrentCharacter.rngSeed = 0;
        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/saveData.json", jsonData);
        PlayerPrefs.SetString("Save", jsonData);

        PlayerPrefs.Save();
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
    public string characterName;
    public int vouchers;
    public int rngSeed;
    public Stats stats;
    public int currentRoomID = 0;
    public int currentArea = 1;
    public int enemiesKilled = 0;
    public int bossesKilled = 0;
    public List<int> visitedRooms;
    public CharacterVisualData visualData;
    public List<SkillSO> learnedSkills;
    public List<SkillSO> foundSkills;
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