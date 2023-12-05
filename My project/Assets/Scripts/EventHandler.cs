using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public List<GameObject> disableObjects;
    public List<GameObject> enableObjects;
    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.spawnLevelGates += DisableObjects;
    }

    void DisableObjects() {
        foreach (var item in disableObjects)
        {
            item.SetActive(false);
        }
        foreach (var item in enableObjects)
        {
            item.SetActive(true);
        }
    }
}
