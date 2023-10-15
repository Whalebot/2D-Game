using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGate : Interactable
{
    public int levelScene;


    public override void South()
    {
        base.South();
        EnterGate();
    }

    public void EnterGate() {
        TransitionManager.Instance.LoadScene(levelScene);
    }
}
