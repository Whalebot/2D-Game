using Sirenix.OdinInspector;
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
    public event Action southInput;
    public event Action westInput;
    public event Action eastInput;
    public event Action northInput;
    [TabGroup("Components")] public AttackScript attack;
    [TabGroup("Components")] public Status status;
    [TabGroup("Components")] public InteractScript interact;

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
        if (GameManager.menuOpen || GameManager.isPaused || status.isDead)
        {
            mov.direction = Vector3.zero;
            //            mov._rb.velocity = Vector3.zero;
            return;
        }

        if (input.inputDirection.x != 0)
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
            return attack.ComboAttack(attack.moveset.lightCombo);
        }
        else
        {
            return attack.ComboAttack(attack.moveset.airLightCombo);
        }
    }

    bool NorthButton()
    {
        if (mov.ground)
        {
            return attack.ComboAttack(attack.moveset.heavyCombo);
        }
        else
        {
            return attack.ComboAttack(attack.moveset.airHeavyCombo);
        }
    }

    bool SouthButton()
    {
        if (interact.CanSouth())
        {
            interact.South();
            return true;
        }
        if (mov.ground)
        {
            if (status.NonAttackState() || attack.attacking && attack.canTargetCombo)
                mov.Jump();
            else return false;
        }
        else
        {
            if (status.NonAttackState() || attack.attacking && attack.canTargetCombo)
                mov.DoubleJump();
            else
                return false;

        }
        return true;
    }

    bool EastButton()
    {
        if (mov.ground)
        {

            return attack.ComboAttack(attack.moveset.skillCombo);
        }
        else
        {
            return attack.ComboAttack(attack.moveset.airSkillCombo);
        }
    }
    bool UpEastButton()
    {
        return attack.ComboAttack(attack.moveset.upSkillCombo);
    }
    bool DownEastButton()
    {
        return attack.ComboAttack(attack.moveset.downSkillCombo);
    }
    bool R1Button()
    {
        if (mov.ground)
        {

            return attack.Attack(attack.moveset.dodge);
        }
        else
        {
            return attack.Attack(attack.moveset.airDodge);
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
                    if (SouthButton())
                    {
                        DeleteInputs(i);
                    }
                    break;
                case 4:
                    if (input.bufferedInputs[i].dir == 8)
                    {
                        if (UpEastButton())
                        {
                            DeleteInputs(i);
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 2)
                    {
                        if (DownEastButton())
                        {
                            DeleteInputs(i);
                            break;
                        }
                    }

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
                //case 81:
                //    if (mov.ground)
                //    {

                //        DeleteInputs(i);
                //    }
                //    break;
                //case 82:
                //    if (mov.ground)
                //    {

                //        DeleteInputs(i);
                //    }
                //    break;
                //case 83:
                //    if (mov.ground)
                //    {

                //        DeleteInputs(i);
                //    }
                //    break;
                //case 84:
                //    if (UpEastButton())
                //    {
                //        DeleteInputs(i);
                //    }
                //    break;
                default: break;
            }
        }
    }
    void InAnimationInput()
    {
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
