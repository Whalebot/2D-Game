﻿using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }
    public bool flipPlayer;
    [HideInInspector] public InputManager input;
    [HideInInspector] public Camera cam;
    [HideInInspector] public Movement mov;
    //public event Action southInput;
    //public event Action westInput;
    //public event Action eastInput;
    //public event Action northInput;
    [TabGroup("Components")] public AttackScript attack;
    [TabGroup("Components")] public Status status;

    Ray ray;
    RaycastHit hit;
    [TabGroup("Interact")] public LayerMask interactableMask;
    [TabGroup("Interact")] public GameObject interactUI;
    [TabGroup("Interact")] public GameObject failInteractFX;
    public GameObject skillQuickslots;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        input = InputManager.Instance;
        status = GetComponent<Status>();
        mov = GetComponent<Movement>();

        cam = Camera.main;
    }

    private void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
    }


    void ExecuteFrame()
    {
        if (GameManager.menuOpen || GameManager.isPaused)
        {
            mov.direction = Vector3.zero;
            //            mov._rb.velocity = Vector3.zero;
            return;
        }

        if (input.inputDirection != Vector2.zero)
        {
            mov.isMoving = true;
            mov.direction = new Vector2(input.inputDirection.x, 0);
        }
        else mov.isMoving = false;

        if (input.inputDirection != Vector2.zero && flipPlayer)
            transform.localScale = new Vector3(Mathf.Sign(input.inputDirection.x), 1, 1);

        switch (status.currentState)
        {
            case Status.State.Neutral:
                NeutralInput();
                break;
            case Status.State.Startup:
                InAnimationInput();
                break;
            case Status.State.Active:
                InAnimationInput();
                break;
            case Status.State.Recovery:
                InAnimationInput();
                break;
            case Status.State.Hitstun:
                break;
            case Status.State.Blockstun:
                break;
            case Status.State.Knockdown:
                break;
            case Status.State.Wakeup:
                NeutralInput();
                break;
            case Status.State.InAnimation:
                break;
            default:
                break;
        }


    }
    bool WestButton()
    {
        if (mov.ground)
        {
            return attack.Attack(attack.moveset.lightCombo.moves[0]);
        }
        else
        {
            return attack.Attack(attack.moveset.airLightCombo.moves[0]);
        }
    }

    bool NorthButton()
    {
        if (mov.ground)
        {
            return attack.Attack(attack.moveset.heavyCombo.moves[0]);
        }
        else
        {
            return attack.Attack(attack.moveset.airHeavyCombo.moves[0]);
        }
    }

    void SouthButton()
    {
    }

    bool EastButton()
    {
        Debug.Log("Test");
        if (mov.ground)
        {
            if (!attack.attacking || attack.attacking && attack.canTargetCombo)
                mov.Jump();
            else return false;
        }
        else
        {
            //if (!attack.attacking || attack.attacking && attack.canTargetCombo)
            //    mov.DoubleJump();
            //else 
            return false;

        }
        return true;
    }

    bool R1Button()
    {
        if (mov.ground)
        {

            return attack.Attack(attack.moveset.dodge);
        }
        else
        {
            return false;
        }
    }

    void NeutralInput()
    {
        for (int i = 0; i < input.bufferedInputs.Count; i++)
        {
            switch (input.bufferedInputs[i].id)
            {
                //Interact button
                case 1:
                    if (WestButton())
                    {
                        DeleteInputs(i);
                    }
                    break;
                case 2:
                    if (NorthButton())
                    {
                        DeleteInputs(i);
                    }
                    break;

                case 3:
                    SouthButton();
                    DeleteInputs(i);
                    break;
                case 4:
                    if (EastButton())
                    {
                        DeleteInputs(i);
                    }
                    break;

                case 5:
                    if (R1Button())
                    {
                        DeleteInputs(i);
                    }
                    break;

                case 6:
                    if (mov.ground)
                    {
                        //  attack.ParryStart();
                        //Delete();
                    }
                    break;
                case 7:
                    if (mov.ground)
                    {
                        attack.Attack(attack.moveset.combos[0].moves[0]);
                        DeleteInputs(i);
                    }
                    break;
                case 8:
                    if (mov.ground)
                    {
                        attack.Attack(attack.moveset.combos[1].moves[0]);
                        DeleteInputs(i);
                    }
                    break;
                case 9:
                    if (mov.ground)
                    {
                        attack.Attack(attack.moveset.combos[2].moves[0]);
                        DeleteInputs(i);
                    }
                    break;
                case 10:
                    if (mov.ground)
                    {
                        attack.Attack(attack.moveset.combos[3].moves[0]);
                        DeleteInputs(i);
                    }
                    break;
                default: break;
            }
        }
    }
    void InAnimationInput()
    {
        mov.sprinting = false;

        if (InputAvailable())
        {
            if (attack.attackString) { NeutralInput(); }
        }
    }

    bool InputAvailable()
    {
        return input.bufferedInputs.Count > 0;
    }
    public void DeleteInputs(int bufferIndex)
    {
        for (int i = 0; i < bufferIndex + 1; i++)
        {
            input.bufferedInputs.RemoveAt(0);
        }
    }
}
