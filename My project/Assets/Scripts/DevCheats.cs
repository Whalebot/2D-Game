using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevCheats : MonoBehaviour
{
    public Rank rank;
    public List<SkillSO> skills;
    public Stats stats;
    private void Start()
    {
        InputManager.Instance.selectInput += Restart;
    }
    void Restart()
    {
        TransitionManager.Instance.LoadScene(0);
    }
    [Button]
    public void GiveSkillSkill()
    {
        foreach (var item in skills)
        {
            item.skillRank = rank;
            SkillManager.Instance.GetSkill(item);
        }

    }

    void Update()
    {
        if (Keyboard.current.numpadPlusKey.wasPressedThisFrame) GameManager.Instance.AdvanceGameStateButton();
        if (Keyboard.current.numpadMinusKey.wasPressedThisFrame) GameManager.Instance.runNormally = true;
        if (Keyboard.current.rKey.wasPressedThisFrame) TransitionManager.Instance.ReloadScene();
        if (Keyboard.current.f1Key.wasPressedThisFrame) SaveManager.Instance.SaveData();
        if (Keyboard.current.f2Key.wasPressedThisFrame) SaveManager.Instance.LoadData();
        if (Keyboard.current.f3Key.wasPressedThisFrame) SaveManager.Instance.DeleteData();
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            for (int i = 0; i < SaveManager.Instance.saveData.unlockedCharacters.Length; i++)
            {
                SaveManager.Instance.saveData.unlockedCharacters[i] = true;
            }

            GameManager.Instance.PlayerStats.maxHealth = 2000;
            GameManager.Instance.PlayerStats.currentHealth = 2000;
            GameManager.Instance.PlayerStats.maxMeter = 2000;
            GameManager.Instance.PlayerStats.currentMeter = 2000;
            GameManager.Instance.PlayerStats.rerolls = 2000;
            GameManager.Instance.PlayerStats.gold = 5000;
        }
    }
}
