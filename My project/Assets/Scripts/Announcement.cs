using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Announcement : MonoBehaviour
{
    public float pauseDuration = 2;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PauseGame());
    }

    IEnumerator PauseGame()
    {
        GameManager.cutscene = true;
        yield return new WaitForSecondsRealtime(pauseDuration);
        GameManager.cutscene = false;
        gameObject.SetActive(false);
    }
}
