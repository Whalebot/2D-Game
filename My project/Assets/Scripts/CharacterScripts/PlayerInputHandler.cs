using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }
    public bool flipPlayer;
    public bool holdWest;
    public bool holdNorth;
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
        input.interactInput += InteractButton;
        input.northInput += HoldNorthButton;
        input.westInput += HoldWestButton;
        input.northRelease += ReleaseNorthButton;
        input.westRelease += ReleaseWestButton;


        status = GetComponent<Status>();
        mov = GetComponent<Movement>();

        cam = Camera.main;
    }

    private void OnDestroy()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
        input.interactInput -= InteractButton;
        input.northInput -= HoldNorthButton;
        input.westInput -= HoldWestButton;
        input.northRelease -= ReleaseNorthButton;
        input.westRelease -= ReleaseWestButton;

    }

    void ExecuteFrame()
    {
        if (GameManager.menuOpen || GameManager.isPaused || status.isDead)
        {
            mov.direction = Vector3.zero;
            mov.isMoving = false;
            return;
        }

        if (input.inputDirection.x == 0 && !mov.isFlying || input.inputDirection == Vector2.zero)
        {
            mov.isMoving = false;

        }
        else
        {
            mov.isMoving = true;
            mov.direction = new Vector2(input.inputDirection.x, input.inputDirection.y);
        }

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

    public void InteractButton()
    {
        if (interact.CanSouth())
        {
            interact.South();
        }
    }

    bool WestButton()
    {
        if (mov.ground)
        {
            if (attack.isDashing)
                if (attack.ComboAttack(attack.moveset.dashAttackCombo))
                {
                    return true;
                }
            return attack.ComboAttack(attack.moveset.lightCombo);
        }
        else
        {
            if (attack.isDashing)
                if (attack.ComboAttack(attack.moveset.airDashAttackCombo))
                {
                    return true;
                }
            return attack.ComboAttack(attack.moveset.airLightCombo);
        }
    }
    bool UpWestButton()
    {
        if (mov.ground)
        {
            return attack.ComboAttack(attack.moveset.upLightCombo);
        }
        else
        {
            return attack.ComboAttack(attack.moveset.airLightCombo);
        }
    }
    bool DownWestButton()
    {
        if (mov.ground)
        {
            return attack.ComboAttack(attack.moveset.downLightCombo);
        }
        else
        {
            return attack.ComboAttack(attack.moveset.downAirLightCombo);
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
    bool UpNorthButton()
    {
        if (mov.ground)
        {
            return attack.ComboAttack(attack.moveset.upHeavyCombo);
        }
        else
        {
            return attack.ComboAttack(attack.moveset.airHeavyCombo);
        }
    }
    bool DownNorthButton()
    {
        if (mov.ground)
        {
            return attack.ComboAttack(attack.moveset.downHeavyCombo);
        }
        else
        {
            return attack.ComboAttack(attack.moveset.downAirHeavyCombo);
        }
    }
    void HoldNorthButton()
    {
        holdNorth = true;
    }
    void HoldWestButton()
    {
        holdWest = true;
    }
    void ReleaseNorthButton()
    {
        holdNorth = false;
        if (attack.IsHoldAttack())
            attack.ReleaseButton();
    }
    void ReleaseWestButton()
    {
        holdWest = false;
        if (attack.IsHoldAttack())
            attack.ReleaseButton();
    }

    bool SouthButton()
    {

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
    bool SideEastButton()
    {
        if (mov.ground)
        {

            return attack.ComboAttack(attack.moveset.sideSkillCombo);
        }
        else
        {
            return attack.ComboAttack(attack.moveset.sideSkillCombo);
        }
    }
    bool UpEastButton()
    {
        return attack.ComboAttack(attack.moveset.upSkillCombo);
    }
    bool DownEastButton()
    {
        if (mov.ground)
        {

            return attack.ComboAttack(attack.moveset.downSkillCombo);
        }
        else
        {
            return attack.ComboAttack(attack.moveset.downAirSkillCombo);
        }
    }

    bool R1Button()
    {
        if (mov.ground)
        {
            if (attack.ComboAttack(attack.moveset.dodgeCombo))
            {
                return true;
            }
            else return false;
        }
        else
        {
            if (attack.ComboAttack(attack.moveset.airDodgeCombo))
            {
                return true;
            }
            else return false;
        }

    }

    bool DashAttack()
    {
        if (mov.ground)
        {
            if (attack.isDashing)
                if (attack.ComboAttack(attack.moveset.dashAttackCombo))
                {
                    return true;
                }
            return false;
        }
        else
        {
            if (attack.isDashing)
                if (attack.ComboAttack(attack.moveset.airDashAttackCombo))
                {
                    return true;
                }
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
                    if (input.bufferedInputs[i].dir == 8)
                    {
                        if (UpWestButton())
                        {
                            DeleteInputs(i);
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 2)
                    {
                        if (DownWestButton())
                        {
                            DeleteInputs(i);
                            break;
                        }
                    }
                    if (WestButton())
                    {
                        DeleteInputs(i);
                    }
                    break;
                case 2:

                    if (input.bufferedInputs[i].dir == 8)
                    {
                        if (UpNorthButton())
                        {
                            DeleteInputs(i);
                            break;
                        }
                    }
                    if (input.bufferedInputs[i].dir == 2)
                    {
                        if (DownNorthButton())
                        {
                            DeleteInputs(i);
                            break;
                        }
                    }
                    if (NorthButton())
                    {
                        DeleteInputs(i);
                    }
                    break;

                case 3:
                    if (input.bufferedInputs[i].dir == 2)
                    {

                        if (mov.ground)
                        {
                            mov.FallThroughPlatforms();
                            DeleteInputs(i);
                            break;
                        }
                    }
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
                    if (input.bufferedInputs[i].dir == 4 || input.bufferedInputs[i].dir == 6)
                    {
                        if (SideEastButton())
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
        if (attack.IsHoldAttack() && !holdNorth && !holdWest)
            attack.ReleaseButton();


        if (InputAvailable())
        {
            if (attack.attackString) { NeutralInput(); }
            else
            {
                for (int i = 0; i < input.bufferedInputs.Count; i++)
                {
                    switch (input.bufferedInputs[i].id)
                    {
                        //Interact button
                        case 1:
                            if (DashAttack())
                            {
                                DeleteInputs(i);
                            }
                            break;
                    }
                }
            }
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
