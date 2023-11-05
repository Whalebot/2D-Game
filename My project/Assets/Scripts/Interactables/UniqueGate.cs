using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueGate : Interactable
{
    public SceneSO sceneSO;
    public override void South()
    {
        base.South();
        EnterGate();
    }
    public void EnterGate()
    {
        TransitionManager.Instance.LoadScene(sceneSO.sceneName);
    }
}
