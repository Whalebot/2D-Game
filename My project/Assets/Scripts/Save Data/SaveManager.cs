using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public bool autoLoad;
    public SaveData saveData;
    public event Action saveEvent;
    public event Action loadEvent;

    private void Awake()
    {
        Instance = this;

    }

 
    private void Start()
    {
        if (autoLoad)
            LoadData();
    }

    public bool HasSaveData()
    {
        return (PlayerPrefs.HasKey("Save"));
    }


    [Button]
    public void DeleteData()
    {
        PlayerPrefs.DeleteKey("Save");
    }

    public void LoadData()
    {
        saveData = new SaveData();
        saveData.learnedSkills = new List<SkillSO>();
        saveData.visualData = new CharacterVisualData();

        if (PlayerPrefs.HasKey("Save"))
        {
            string saveJson = PlayerPrefs.GetString("Save");
            saveData = JsonUtility.FromJson<SaveData>(saveJson);
            loadEvent?.Invoke();
        }

        StartCoroutine(DelaySetup());
        //if (File.Exists(Application.persistentDataPath + "/saveData.json"))
        //{
        //    saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.persistentDataPath + "/saveData.json"));

    }
    IEnumerator DelaySetup()
    {
        yield return new WaitForFixedUpdate();

    }

    public void SaveData()
    {
        saveEvent?.Invoke();

        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/saveData.json", jsonData);
        PlayerPrefs.SetString("Save", jsonData);

        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class SaveData
{
    public int gold = 100;
    public int health = 50;
    public int meter = 0;
    public int currentLevel = 1;
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
}