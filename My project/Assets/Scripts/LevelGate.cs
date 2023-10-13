using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGate : MonoBehaviour
{
    public int levelScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            EnterGate();
    }

    public void EnterGate() {
        TransitionManager.Instance.LoadScene(levelScene);
    }
}
