using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    public static int levelIndex = 1;
    public Animator fadeAnimator;
    public Animator deathAnimator;
    public Animator winAnimator;
    public GameObject resultScreen;

    public static bool isLoading;

    public float sceneTransitionDelay;
    public float fadeDelay = 0;

    private void Awake()
    {
        Instance = this;
        levelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Start()
    {
        isLoading = false;
    }

    IEnumerator TransitionDelay(int sceneIndex)
    {
        if(AudioManager.Instance != null)
        AudioManager.Instance.FadeOutVolume();
        yield return new WaitForSeconds(fadeDelay);
        Fade();
        yield return new WaitForSeconds(sceneTransitionDelay);
        //DataManager.Instance.SaveData();
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator TransitionDelay(string sceneName)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutVolume();
        Fade();
        yield return new WaitForSeconds(sceneTransitionDelay);
        SceneManager.LoadScene(sceneName);
    }


    public void ReloadScene() {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartGame()
    {
        if (isLoading) return;

        isLoading = true;
        StartCoroutine(TransitionDelay(0));
    }
    public void LoadScene(int sceneIndex)
    {
        if (isLoading) return;
        SaveManager.Instance.SaveData();
        isLoading = true;
        StartCoroutine(TransitionDelay(sceneIndex));
    }
    public void LoadScene(string sceneName)
    {
        if (isLoading) return;
        SaveManager.Instance.SaveData();
        isLoading = true;
        StartCoroutine(TransitionDelay(sceneName));
    }
    public void ResultScreen()
    {
        //  resultScreen.SetActive(true);
    }

    public void DeathScreen()
    {
        deathAnimator.SetTrigger("Trigger");
    }


    public void WinScreen()
    {
        winAnimator.SetTrigger("Trigger");
    }
    public void Transition()
    {
        fadeAnimator.SetTrigger("Fade");
    }

    public void Fade()
    {
        fadeAnimator.SetTrigger("Trigger");
    }
}
