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
    public static bool menuOpen;
    public static bool gameOver;

    public static Vector3 startPosition;
    public static Quaternion startRotation;

    [TabGroup("Settings")] public bool runNormally;

    [TabGroup("Settings")] public bool flipGraphics;
    [TabGroup("Settings")] public bool showHitboxes;
    [TabGroup("Settings")] public bool showHurtboxes;
    public static float inGameTime;
    [TabGroup("Settings")] public float timeDisplay;

    [TabGroup("Settings")] public bool showNames = true;
    [TabGroup("Settings")] public bool showHPBar = true;

    [TabGroup("Settings")] public float restartDelay = 2;

    [TabGroup("Settings")] public int gameFrameCount;
    [TabGroup("Settings")] public bool hideMouse;

    public event Action advanceGameState;
    public event Action resetEvent;
    public event Action playerDeath;
    public event Action getSkillEvent;
    public event Action openShopEvent;

    [TabGroup("Components")] public GameObject playerUI;
    [TabGroup("Components")] public Transform player;
    [TabGroup("Components")] public Status playerStatus;

    [TabGroup("Feedback")] public bool showDamageText;
    [TabGroup("Feedback")] public float numberOffset;
    [TabGroup("Feedback")] public int offsetResetSpeed;
    [TabGroup("Feedback")] [SerializeField] int offsetCounter;
    [TabGroup("Feedback")] [SerializeField] int offsetTime;
    [TabGroup("Feedback")] public GameObject damageText;
    [TabGroup("Feedback")] public GameObject critDamageText;
    [TabGroup("Feedback")] public GameObject healingText;
    [TabGroup("Feedback")] public float slowMotionValue;
    [TabGroup("Feedback")] public float slowMotionDuration;
    [TabGroup("Feedback")] public float slowMotionSmoothing;

    bool reloading;
    float startTimeStep;

    public int Gold
    {
        get { return playerStatus.currentStats.gold; }
        set
        {
            playerStatus.currentStats.gold = value;
        }
    }

    private void Awake()
    {
        Instance = this;
        if (startPosition != Vector3.zero)
        {
            SetPlayerPosition(startPosition, startRotation);
            startPosition = Vector3.zero;
        }

        isPaused = false;
        menuOpen = false;

        playerStatus = player.GetComponent<Status>();
        playerStatus.deathEvent += LoseGame;
        SaveManager.Instance.saveEvent += SaveData;
        SaveManager.Instance.startLoadEvent += LoadData;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;

        startTimeStep = Time.fixedDeltaTime;
        AIManager.Instance.allEnemiesKilled += RoomCleared;

    }

    void SaveData()
    {
        SaveManager.Instance.saveData.stats.ReplaceStats(playerStatus.currentStats);
    }

    void LoadData()
    {
        //Load temp stats
        playerStatus.currentStats.ReplaceStats(SaveManager.Instance.saveData.stats);
        Debug.Log($"{playerStatus.currentStats.gold} {SaveManager.Instance.saveData.stats.gold}");
    }

    void RoomCleared()
    {
        switch (LevelManager.Instance.currentRoomType)
        {
            case RoomTypes.Normal:
                Gold += 20;
                break;
            case RoomTypes.Boss:
                Gold += 100;
                break;
            case RoomTypes.Treasure:
                Gold += 35;
                break;
            case RoomTypes.Shop:
                break;
            case RoomTypes.Disabled:
                break;
            default:
                break;
        }

    }

    public void OpenGetSkillWindow()
    {
        getSkillEvent?.Invoke();
        ToggleMenu();
    }

    public void OpenShopWindow()
    {
        openShopEvent?.Invoke();
        ToggleMenu();
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

        if (offsetTime > 0)
        {
            offsetTime--;
            if (offsetTime <= 0) offsetCounter = 0;
        }

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

    public void KillPlayer()
    {
        playerDeath?.Invoke();
    }

    public void DamageNumbers(Transform other, int damageValue, bool crit)
    {
        if (showDamageText)
        {
            GameObject text = null;
            if (crit)
                text = Instantiate(critDamageText, other.position + Vector3.up * offsetCounter * numberOffset, Quaternion.identity);
            else
                text = Instantiate(damageText, other.position + Vector3.up * offsetCounter * numberOffset, Quaternion.identity);

            text.GetComponentInChildren<DamageText>().SetupNumber(damageValue);
            offsetCounter++;
            offsetTime = offsetResetSpeed;
        }
    }


    public void Slowmotion(float dur)
    {
        StartCoroutine(SetSlowmotion(dur));
    }

    public void FinalHit()
    {

        // CameraManager.Instance.SetGroupTarget();
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

    public void ToggleMenu()
    {
        menuOpen = !menuOpen;
        Physics.autoSimulation = !Physics.autoSimulation;
    }
    public void CloseMenu()
    {
        menuOpen = false;
        Physics.autoSimulation = true;
    }
    void FixedUpdate()
    {
        isPaused = menuOpen;
        if (isPaused)
        {

            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            return;
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
        SaveManager.Instance.DeleteData();
        TransitionManager.Instance.RestartGame();
    }
}
