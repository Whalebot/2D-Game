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

    [Button]
    public void DeleteData()
    {
        PlayerPrefs.DeleteKey("Save");
    }

    public void LoadData()
    {
        loadEvent?.Invoke();

        SkillHandler skillHandler = GameManager.Instance.player.GetComponent<SkillHandler>();
        skillHandler.RemoveAllSkills();
        saveData = new SaveData();
        saveData.learnedSkills = new List<SkillSO>();

        if (PlayerPrefs.HasKey("Save"))
        {
            string saveJson = PlayerPrefs.GetString("Save");
            saveData = JsonUtility.FromJson<SaveData>(saveJson);
 
            foreach (var item in saveData.learnedSkills)
            {
                skillHandler.LearnSkill(item);
            }

            LevelManager.Instance.currentLevel = saveData.currentLevel;
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
        saveData.currentLevel = LevelManager.Instance.currentLevel;

        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/saveData.json", jsonData);
        PlayerPrefs.SetString("Save", jsonData);

        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class SaveData
{
    public int currentLevel = 1;
    public List<SkillSO> learnedSkills;
}