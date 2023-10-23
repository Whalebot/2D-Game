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
    public event Action awakeLoadEvent;
    public event Action startLoadEvent;

    private void Awake()
    {
        Instance = this;
        if (autoLoad)
            LoadData();
    }

 
    private void Start()
    {
        if (autoLoad && HasSaveData())
            startLoadEvent?.Invoke();
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

        if (PlayerPrefs.HasKey("Save"))
        {
            saveData = new SaveData();
            saveData.learnedSkills = new List<SkillSO>();
            saveData.visualData = new CharacterVisualData();

            string saveJson = PlayerPrefs.GetString("Save");
            saveData = JsonUtility.FromJson<SaveData>(saveJson);
            awakeLoadEvent?.Invoke();
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
    public Stats stats;
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
    [Range(0, 10)] public int shoesID;
}