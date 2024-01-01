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

    public static float time;

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
    [TabGroup("Settings")] public float realFrameCount;
    [TabGroup("Settings")] public bool hideMouse;

    public event Action pauseEvent;
    public event Action resumeEvent;
    public event Action advanceGameState;
    public event Action resetEvent;
    public event Action playerDeath;
    public event Action<RewardType> openRewardWindowEvent;
    public event Action openShopEvent;
    public event Action goldChangeEvent;

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

    [TabGroup("Feedback")] public GameObject enemyDamageText;
    [TabGroup("Feedback")] public GameObject enemyCritDamageText;

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
            goldChangeEvent?.Invoke();
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
        player = GameObject.FindGameObjectWithTag("Player").transform;

        InputManager.Instance.startInput += TogglePause;
        playerStatus = player.GetComponent<Status>();
        playerStatus.deathEvent += LoseGame;
        SaveManager.Instance.saveEvent += SaveData;
        SaveManager.Instance.startLoadEvent += LoadData;
        Physics.simulationMode = SimulationMode.Script;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;

        startTimeStep = Time.fixedDeltaTime;
        AIManager.Instance.roomClearEvent += RoomCleared;

    }

    void SaveData()
    {
        SaveManager.Instance.Stats.ReplaceStats(playerStatus.currentStats);
    }

    void LoadData()
    {
        //Load temp stats
        playerStatus.currentStats.ReplaceStats(SaveManager.Instance.Stats);
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

    public void OpenGetSkillWindow(RewardType rewardType)
    {
        openRewardWindowEvent?.Invoke(rewardType);
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

        if (runNormally) Physics.Simulate(Time.fixedDeltaTime);

        advanceGameState?.Invoke();

        if (offsetTime > 0)
        {
            offsetTime--;
            if (offsetTime <= 0) offsetCounter = 0;
        }

        realFrameCount += Time.timeScale;
        gameFrameCount++;
        time++;
        //frameCounterEvent?.Invoke(gameFrameCount);
    }

    [Button]
    public void AdvanceGameStateButton()
    {
        runNormally = false;
        AdvanceGameState();
        Physics.Simulate(Time.fixedDeltaTime);

    }

    public void KillPlayer()
    {
        playerDeath?.Invoke();
    }

    public void DamageNumbers(Transform other, int damageValue, bool crit, Alignment alignment)
    {
        if (showDamageText)
        {
            GameObject text = null;
            switch (alignment)
            {
                case Alignment.Player:
                    if (crit)
                        text = Instantiate(critDamageText, other.position + Vector3.up * offsetCounter * numberOffset, Quaternion.identity);
                    else
                        text = Instantiate(damageText, other.position + Vector3.up * offsetCounter * numberOffset, Quaternion.identity);
                    break;
                case Alignment.Enemy:
                    if (crit)
                        text = Instantiate(enemyCritDamageText, other.position + Vector3.up * offsetCounter * numberOffset, Quaternion.identity);
                    else
                        text = Instantiate(enemyDamageText, other.position + Vector3.up * offsetCounter * numberOffset, Quaternion.identity);
                    break;
                case Alignment.Neutral:
                    if (crit)
                        text = Instantiate(enemyCritDamageText, other.position + Vector3.up * offsetCounter * numberOffset, Quaternion.identity);
                    else
                        text = Instantiate(enemyDamageText, other.position + Vector3.up * offsetCounter * numberOffset, Quaternion.identity);
                    break;
                default:
                    break;
            }
         

            text.GetComponentInChildren<DamageText>().SetupNumber(damageValue);
            offsetCounter++;
            offsetCounter = Mathf.Clamp(offsetCounter, 0, 2);
            offsetTime = offsetResetSpeed;
        }
    }


    public void Slowmotion(float dur)
    {
        if (dur <= 0) return;
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

    public void TogglePause()
    {
        if (menuOpen)
            menuOpen = false;
        else
            menuOpen = true;

        if (menuOpen)
            pauseEvent?.Invoke();
        else
            resumeEvent?.Invoke();
        //Time.timeScale = 0;
    }

    public void ResumeGame()
    {
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
        if (menuOpen)
        {
            runNormally = false;
        }
        else CloseMenu();
    }
    public void CloseMenu()
    {
        menuOpen = false;
        runNormally = true;
        resumeEvent?.Invoke();
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
    }

    public void ReloadGame()
    {
        StartCoroutine(DeathScreenDelay());
        StartCoroutine(RestartDelay());
    }

    IEnumerator DeathScreenDelay()
    {
        yield return new WaitForSeconds(0.5F);
        TransitionManager.Instance.DeathScreen();
    }

    IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        SaveManager.Instance.KillCharacter();
        TransitionManager.Instance.RestartGame();
    }
}
