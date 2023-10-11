using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool isPaused;
    public static bool cutscene;
    public static bool inventoryMenuOpen;
    public static bool shopOpen;
    public static bool menuOpen;
    public static bool gameOver;

    public static Vector3 startPosition;
    public static Quaternion startRotation;

    [TabGroup("Settings")] public bool runNormally;

    [TabGroup("Settings")] public bool showHitboxes;
    public static float inGameTime;
    [TabGroup("Settings")] public float timeDisplay;

    [TabGroup("Settings")] public bool showNames = true;
    [TabGroup("Settings")] public bool showHPBar = true;

    static bool saveOnce = false;

    [TabGroup("Settings")] public float restartDelay = 5;

    [TabGroup("Settings")] public int gameFrameCount;
    [TabGroup("Settings")] public bool hideMouse;

    public event Action advanceGameState;
    public event Action resetEvent;
    public event Action playerDeath;
    [TabGroup("Components")] public GameObject playerUI;
    [TabGroup("Components")] public Transform player;

    [TabGroup("Feedback")] public bool showDamageText;
    [TabGroup("Feedback")] public float numberOffset;
    [TabGroup("Feedback")] public float offsetResetSpeed;
    [TabGroup("Feedback")] float offsetCounter;
    [TabGroup("Feedback")] public GameObject damageText;
    [TabGroup("Feedback")] public GameObject healingText;
    [TabGroup("Feedback")] public float slowMotionValue;
    [TabGroup("Feedback")] public float slowMotionDuration;
    [TabGroup("Feedback")] public float slowMotionSmoothing;

    bool reloading;
    float startTimeStep;

    private void Awake()
    {
        Instance = this;
        if (startPosition != Vector3.zero)
        {
            SetPlayerPosition(startPosition, startRotation);
            startPosition = Vector3.zero;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;

        if (!saveOnce)
        {
            saveOnce = true;
        }

        //AIManager.Instance.allEnemiesKilled += FinalHit;
        startTimeStep = Time.fixedDeltaTime;
    }

    public void SetPlayerPosition(Vector3 pos, Quaternion rot)
    {
        player.transform.position = pos;
        player.transform.rotation = rot;
    }


    public void AdvanceGameState()
    {

        Physics.autoSimulation = runNormally;
        advanceGameState?.Invoke();

        gameFrameCount++;
        //frameCounterEvent?.Invoke(gameFrameCount);
    }

    [Button]
    public void AdvanceGameStateButton()
    {
        runNormally = false;
        Physics.autoSimulation = false;
        AdvanceGameState();
        Physics.Simulate(Time.fixedDeltaTime);

    }

    public void SaveData()
    {
        //  SaveManager.Instance.SaveData();
    }

    public void KillPlayer()
    {
        playerDeath?.Invoke();
    }

    public void SetTrigger(string n)
    {
        //GameTriggers triggers = DataManager.Instance.currentSaveData.triggers;

        //FieldInfo[] defInfo = triggers.GetType().GetFields();

        //for (int i = 0; i < defInfo.Length; i++)
        //{

        //    object obj = triggers;
        //    if (defInfo[i].GetValue(obj) is bool)
        //    {

        //        if (defInfo[i].Name.Contains(n))
        //        {
        //            defInfo[i].SetValue(obj, true);
        //        }
        //    }
        //}
    }


    public void DamageNumbers(Transform other, int damageValue)
    {
        if (showDamageText)
        {
            GameObject text = Instantiate(damageText, other.position + Vector3.up * offsetCounter, Quaternion.identity);
            text.GetComponentInChildren<TextMeshProUGUI>().text = "" + damageValue;
            offsetCounter += numberOffset;
        }
    }


    public void Slowmotion(float dur)
    {
        StartCoroutine(SetSlowmotion(dur));
    }

    public void FinalHit()
    {

        CameraManager.Instance.SetGroupTarget();
    }

    IEnumerator DelayResume()
    {
        yield return new WaitForFixedUpdate();
        isPaused = false;
    }

    public void PauseGame()
    {
        isPaused = true;

        //Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        // print("Resume");

        //  Time.timeScale = 1;
        StartCoroutine(DelayResume());
    }

    IEnumerator SetSlowmotion(float dur)
    {
        Time.timeScale = slowMotionValue;
        Time.fixedDeltaTime = startTimeStep * Time.timeScale;
        yield return new WaitForSecondsRealtime(dur * (1F / 60F));
        StartCoroutine(RevertSlowmotion());
    }
    IEnumerator RevertSlowmotion()
    {
        //gameOver = true;
        //CameraManager.Instance.RevertCamera();
        while (Time.timeScale < 1 && !isPaused)
        {
            Time.timeScale = Time.timeScale + slowMotionSmoothing;

            Time.fixedDeltaTime = startTimeStep * Time.timeScale;
            yield return new WaitForEndOfFrame();
        }


        Time.timeScale = 1;
        Time.fixedDeltaTime = startTimeStep;

    }

    void FixedUpdate()
    {
        if (isPaused)
        {
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            //return;
        }
        if (hideMouse)
        {
            //Cursor.visible = menuOpen;
            //Cursor.lockState = CursorLockMode.Confined;
        }

        if (runNormally)
        {
            AdvanceGameState();
        }
        inGameTime += Time.fixedDeltaTime;
        timeDisplay = inGameTime;

        if (offsetCounter > 0)
        {
            offsetCounter -= Time.fixedDeltaTime * offsetResetSpeed;
            if (offsetCounter <= 0) offsetCounter = 0;
        }

        if (gameOver && !reloading)
        {
            LoseGame();
        }
    }


    public void LoseGame()
    {
        playerDeath?.Invoke();
        reloading = true;
        StartCoroutine(DeathScreenDelay());
        StartCoroutine(RestartDelay());
    }

    public void Sleep()
    {
        reloading = true;
        StartCoroutine("DeathScreenDelay");
        TransitionManager.Instance.LoadHouse();
    }

    void RestartGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        inGameTime = 0;
    }

    IEnumerator DeathScreenDelay()
    {
        yield return new WaitForSeconds(0.5F);
        TransitionManager.Instance.DeathScreen();
    }

    IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        TransitionManager.Instance.LoadHouse();
    }

    public void ReturnToHub()
    {
        TransitionManager.Instance.LoadHub();
    }

}
