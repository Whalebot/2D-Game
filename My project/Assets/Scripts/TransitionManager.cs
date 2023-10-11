using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    public Animator fadeAnimator;
    public Animator deathAnimator;
    public Animator winAnimator;
    public GameObject endOfDayScreen;

    public static bool isLoading;

    public float sceneTransitionDelay;
    //  public GameObject resultScreen;
    bool dayEnded = false;
    private void Awake()
    {
        Instance = this;
        InputManager.Instance.southInput += ButtonPress;
    }

    void Start()
    {
        isLoading = false;
    }

    void ButtonPress()
    {
    }

    public void LoadHub()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator TransitionDelay(int sceneIndex)
    {
        AudioManager.Instance.FadeOutVolume();
        Fade();
        yield return new WaitForSeconds(sceneTransitionDelay);
        //DataManager.Instance.SaveData();
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator TransitionDelay(string sceneName)
    {
        AudioManager.Instance.FadeOutVolume();
        Fade();
        yield return new WaitForSeconds(sceneTransitionDelay);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadHouse()
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(SleepDelay(1));
    }

    IEnumerator SleepDelay(int sceneIndex)
    {
        AudioManager.Instance.FadeOutVolume();
        Fade();
        yield return new WaitForSeconds(sceneTransitionDelay);
        endOfDayScreen.SetActive(true);
        dayEnded = true;
    }

    public void EndDay()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadScene(int sceneIndex)
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(TransitionDelay(sceneIndex));
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading) return;
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
