using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
public class DevCheats : MonoBehaviour
{
    private void Start()
    {
        InputManager.Instance.selectInput += Restart;
    }
    void Restart() {
        TransitionManager.Instance.LoadScene(0);
    }

    void Update()
    {
        if (Keyboard.current.numpadPlusKey.wasPressedThisFrame) GameManager.Instance.AdvanceGameStateButton();
        if (Keyboard.current.numpadMinusKey.wasPressedThisFrame) GameManager.Instance.runNormally = true;
        if (Keyboard.current.rKey.wasPressedThisFrame) TransitionManager.Instance.LoadScene(0);
        if (Keyboard.current.f1Key.wasPressedThisFrame) SaveManager.Instance.SaveData();
        if (Keyboard.current.f2Key.wasPressedThisFrame) SaveManager.Instance.LoadData();
        if (Keyboard.current.f3Key.wasPressedThisFrame) SaveManager.Instance.DeleteData();
    }
}
