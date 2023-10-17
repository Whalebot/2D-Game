using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelGate : Interactable
{
    public int levelScene;
    public GameObject levelGates;
    public TextMeshProUGUI levelName;

    public override void Start()
    {
        SkillManager.Instance.pickedSkillEvent += SpawnLevelGates;
        levelGates.SetActive(false);
        levelScene = LevelManager.Instance.NextLevelIndex();
    }
    void SpawnLevelGates(SkillSO skillSO)
    {
        levelGates.SetActive(true);
    }

    public override void South()
    {
        base.South();
        EnterGate();
    }

    public void EnterGate() {
        LevelManager.Instance.GoToNextLevel();
        TransitionManager.Instance.LoadScene(levelScene);
    }
}
