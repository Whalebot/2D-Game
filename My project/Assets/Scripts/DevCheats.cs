using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevCheats : MonoBehaviour
{
    public Rank rank;
    public SkillSO skill;
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
    public void GiveSkillSkill() {
        skill.skillRank = rank;
        SkillManager.Instance.GetSkill(skill); 
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
               

            GameManager.Instance.playerStatus.currentStats.maxHealth = 2000;
            GameManager.Instance.playerStatus.currentStats.currentHealth = 2000;
            GameManager.Instance.playerStatus.currentStats.maxMeter = 2000;
            GameManager.Instance.playerStatus.currentStats.currentMeter = 2000;
            GameManager.Instance.playerStatus.currentStats.rerolls = 2000;
        }
    }
}
