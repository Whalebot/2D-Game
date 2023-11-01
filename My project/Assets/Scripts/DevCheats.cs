using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevCheats : MonoBehaviour
{
    public SkillSO skill;
    private void Start()
    {
        InputManager.Instance.selectInput += Restart;
    }
    void Restart()
    {
        TransitionManager.Instance.LoadScene(0);
    }
    [Button]
    public void GiveSkillSkill() { SkillManager.Instance.GetSkill(skill); }

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
            GameManager.Instance.playerStatus.currentStats.maxHealth = 1000000;
            GameManager.Instance.playerStatus.currentStats.currentHealth = 1000000;
            GameManager.Instance.playerStatus.currentStats.maxMeter = 1000000;
            GameManager.Instance.playerStatus.currentStats.currentMeter = 1000000;
        }
    }
}
