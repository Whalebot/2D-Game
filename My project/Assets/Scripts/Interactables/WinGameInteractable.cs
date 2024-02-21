using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameInteractable : Interactable
{
    public override void South()
    {
        base.South();

        GameManager.Instance.WinGame();
    }
}
