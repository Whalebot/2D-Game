using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int currentLevel = 1;
    public int nextLevelIndex;
    private void Awake()
    {
        Instance = this;
        nextLevelIndex = NextLevelIndex();
    }

    public void GoToNextLevel()
    {
        currentLevel++;
    }

    [Button]
    public int NextLevelIndex()
    {
        if (currentLevel % 4 == 0)
            return 1;
        else
            return 0;
    }
}
