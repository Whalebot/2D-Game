using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public static ControlScheme controlScheme = ControlScheme.PS4;
    public Joystick phoneJoystick;
    public delegate void InputEvent();
    public Controls controls = null;

    public bool upToJump;

    public List<BufferedInput> bufferedInputs;
    List<BufferedInput> deleteInputs;
    public int bufferWindow;

    public bool InputAvailable => bufferedInputs.Count > 0;

    #region Events
    public InputEvent controlSchemeChange;
    public InputEvent keyboardEvent;
    public InputEvent mouseEvent;
    public InputEvent gamepadEvent;

    public InputEvent interactInput;

    public InputEvent westInput;
    public InputEvent northInput;
    public InputEvent eastInput;
    public InputEvent southInput;

    public InputEvent westRelease;
    public InputEvent northRelease;
    public InputEvent southRelease;
    public InputEvent eastRelease;

    public InputEvent startInput;
    public InputEvent selectInput;
    public InputEvent R1input;
    public InputEvent R1release;
    public InputEvent R2input;
    public InputEvent R2release;
    public InputEvent L1input;
    public InputEvent L1release;
    public InputEvent L2input;
    public InputEvent L2release;
    public InputEvent L3input;
    public InputEvent R3input;
    public InputEvent R3Right;
    public InputEvent R3Left;
    public InputEvent touchPadInput;
    public InputEvent leftInput;
    public InputEvent rightInput;
    #endregion

    [HideInInspector] public bool R1Hold;
    [HideInInspector] public bool R2Hold;
    [HideInInspector] public bool L1Hold;
    [HideInInspector] public bool L2Hold;

    public Vector2 inputDirection;
    public Vector2 lookDirection;
    public Vector2 mousePosition;
    public Vector2 mouseScroll;

    public bool dodgeTap;
    public int tapFrames = 10;
    int tapCounter;

    public bool debug;
    float lastTapTime;
    bool canTap;

    private void OnEnable() => controls.Default.Enable();
    private void OnDisable() => controls.Default.Disable();

    void Awake()
    {
        Instance = this;

        controls = new Controls();
        // controls.MouseScheme.
        controls.Default.LAnalog.performed += context => InputDirection(context);

        controls.Default.West.performed += context => OnWest(context);
        controls.Default.West.canceled += context => OnWest(context);

        controls.Default.North.performed += context => OnNorth(context);
        controls.Default.North.canceled += context => OnNorth(context);

        controls.Default.South.performed += context => OnSouth(context);
        controls.Default.South.canceled += _ => OnSouthRelease();

        controls.Default.East.performed += context => OnEast(context);

        controls.Default.Up.performed += context => OnUp(context);
        controls.Default.Left.performed += context => OnLeft(context);
        controls.Default.Right.performed += context => OnRight(context);
        controls.Default.Down.performed += context => OnDown(context);

        controls.Default.R1.performed += context => OnR1(context);
        controls.Default.R1.canceled += _ => OnR1Release();

        controls.Default.Interact.performed += context => OnInteract(context);

        controls.Default.R2.performed += _ => OnR2Press();
        controls.Default.R2.canceled += _ => OnR2Release();

        controls.Default.L1.performed += context => OnL1(context);
        controls.Default.L1.canceled += _ => OnL1Release();

        controls.Default.L2.performed += _ => OnL2Press();
        controls.Default.L2.canceled += _ => OnL2Release();

        controls.Default.L3.performed += _ => OnL3();
        controls.Default.R3.performed += _ => OnR3();

        controls.Default.Start.performed += _ => OnStart();
        controls.Default.Select.performed += _ => OnSelect();

        controls.Default.Console.performed += _ => OnTouchPad();

        controls.Default._1.performed += _ => On1();
        controls.Default._2.performed += _ => On2();
        controls.Default._3.performed += _ => On3();
        controls.Default._4.performed += _ => On4();

    }
    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.advanceGameState += ExecuteFrame;

        bufferedInputs = new List<BufferedInput>();
        deleteInputs = new List<BufferedInput>();
    }

    void InputDirection(InputAction.CallbackContext context)
    {
        ChangeControlScheme(context);
        //    Vector2 v = context.ReadValue<Vector2>();
    }

    void ExecuteFrame()
    {
        if (R1Hold)
        {
            tapCounter++;
        }

        deleteInputs.Clear();
        foreach (var item in bufferedInputs)
        {
            item.frame--;
            if (item.frame <= 0)
                deleteInputs.Add(item);
        }
        foreach (var item in deleteInputs)
        {
            if (bufferedInputs.Contains(item))
                bufferedInputs.Remove(item);
        }
    }

    void LoadScheme()
    {
        //controlScheme = DataManager.Instance.currentSaveData.settings.controls;
    }

    void SaveScheme()
    {
        //  DataManager.Instance.currentSaveData.settings.controls = controlScheme;
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (debug) print("Interact");
        ChangeControlScheme(context);
        interactInput?.Invoke();
    }
    private void OnLeft(InputAction.CallbackContext context)
    {
        if (debug) print("Left");
        ChangeControlScheme(context);
        //InputBuffer(("InputBuffer", 7);
        leftInput?.Invoke();
    }
    private void OnRight(InputAction.CallbackContext context)
    {
        if (debug) print("Right");
        ChangeControlScheme(context);
        rightInput?.Invoke();
    }

    private void OnMouseScrollDown()
    {
        if (debug) print("Scroll Down");
        ChangeControlScheme(ControlScheme.Keyboard);
        leftInput?.Invoke();
    }
    private void OnMouseScrollUp()
    {
        if (debug) print("Scroll Up");
        ChangeControlScheme(ControlScheme.Keyboard);
        rightInput?.Invoke();
    }
    private void OnUp(InputAction.CallbackContext context)
    {
        if (debug) print("Up");
        ChangeControlScheme(context);
        if (upToJump)
            InputBuffer(3);
    }
    private void OnDown(InputAction.CallbackContext context)
    {
        if (debug) print("Down");
        ChangeControlScheme(context);
        //InputBuffer(("InputBuffer", 9);
    }


    private void Update()
    {
        if (GameManager.isPaused)
        {
            inputDirection = Vector2.zero;
            lookDirection = Vector2.zero;
            return;
        }
        inputDirection = controls.Default.LAnalog.ReadValue<Vector2>();
        //inputDirection = phoneJoystick.Direction;
        lookDirection = controls.Default.RAnalog.ReadValue<Vector2>();
        mouseScroll = controls.Default.ScrollWheel.ReadValue<Vector2>();

        if (mouseScroll.y > 0) { OnMouseScrollDown(); }
        if (mouseScroll.y < 0) { OnMouseScrollUp(); }

        if (canTap && lastTapTime + 0.1F < Time.time)
        {
            if (lookDirection.x > 0.5F) RAnalogTapRight();
            else if (lookDirection.x < -0.5F) RAnalogTapLeft();
        }
        else if (Mathf.Abs(lookDirection.x) < 0.1F) canTap = true;
    }
    void ChangeControlScheme(ControlScheme scheme)
    {
        controlScheme = scheme;
        switch (scheme)
        {
            case ControlScheme.PS4:
                gamepadEvent?.Invoke();
                controlScheme = ControlScheme.PS4;
                controlSchemeChange?.Invoke();
                break;
            case ControlScheme.XBOX:
                gamepadEvent?.Invoke();
                controlScheme = ControlScheme.PS4;
                controlSchemeChange?.Invoke();
                break;
            case ControlScheme.Keyboard:
                controlScheme = ControlScheme.Keyboard;
                controlSchemeChange?.Invoke();
                keyboardEvent?.Invoke();
                //Cursor.lockState = CursorLockMode.Confined;
                // Cursor.visible = true;
                break;
            case ControlScheme.Switch:
                break;
            default:
                break;
        }
    }
    void ChangeControlScheme(InputAction.CallbackContext context)
    {
        ControlScheme oldScheme = controlScheme;
        if (context.control.device == Gamepad.current)
        {
            if (controlScheme == ControlScheme.Keyboard) gamepadEvent?.Invoke();
            if (Gamepad.current.name.Contains("Dual"))
            {
                controlScheme = ControlScheme.PS4;
                controlSchemeChange?.Invoke();
            }

            else
            {

                if (controlScheme != ControlScheme.XBOX)
                    controlSchemeChange?.Invoke();
                controlScheme = ControlScheme.XBOX;

            }
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
        else if (context.control.device == Mouse.current)
        {
            controlScheme = ControlScheme.Mouse;
            controlSchemeChange?.Invoke();
            mouseEvent?.Invoke();

            //Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {

            if (controlScheme != ControlScheme.Keyboard && controlScheme != ControlScheme.Mouse)
                controlScheme = ControlScheme.Keyboard;

            controlSchemeChange?.Invoke();
            keyboardEvent?.Invoke();
            //Cursor.lockState = CursorLockMode.Confined;
            // Cursor.visible = true;
        }
        if (oldScheme != controlScheme)
            Debug.Log(controlScheme);
    }

    void OnTouchPad()
    {
        touchPadInput?.Invoke();
    }

    void RAnalogTapRight()
    {
        canTap = false;
        lastTapTime = Time.time;
        R3Right?.Invoke();
    }

    void RAnalogTapLeft()
    {
        canTap = false;
        lastTapTime = Time.time;
        R3Left?.Invoke();
    }

    public void OnLAnalog(InputValue value)
    {
        inputDirection = value.Get<Vector2>();
    }
    public void OnRAnalog(InputValue value)
    {
        lookDirection = value.Get<Vector2>();
    }

    public void OnWest(InputAction.CallbackContext context)
    {
        if (debug)
            print("Square");

        ChangeControlScheme(context);
        if (context.canceled)
        {
            westRelease?.Invoke();
            return;
        }
        westInput?.Invoke();
        if (!GameManager.isPaused && !GameManager.menuOpen)
        {
            //if (inputDirection.y > 0)
            //{
            //    InputBuffer(81);
            //}
            //else
            InputBuffer(1);
        }
    }
    public void OnWest()
    {
 
        westInput?.Invoke();
        if (!GameManager.isPaused && !GameManager.menuOpen)
        {
            InputBuffer(1);
        }
    }
    public void OnNorth(InputAction.CallbackContext context)
    {
        if (debug) print("Triangle");
        ChangeControlScheme(context);
        if (context.canceled)
        {
            northRelease?.Invoke();
            return;
        }
        northInput?.Invoke();
        if (!GameManager.isPaused && !GameManager.menuOpen)
        {
            //if (inputDirection.y > 0)
            //{
            //    InputBuffer(82);
            //}
            //else
            InputBuffer(2);
        }

    }
    public void OnNorth()
    {

        northInput?.Invoke();
        if (!GameManager.isPaused && !GameManager.menuOpen)
        {

            InputBuffer(2);
        }

    }
    public void OnSouth(InputAction.CallbackContext context)
    {
        ChangeControlScheme(context);


        if (debug) print("X");
        southInput?.Invoke();

        if (!GameManager.isPaused && !GameManager.menuOpen)
        {

            InputBuffer(3);
        }

    }
    public void OnSouth()
    {
        southInput?.Invoke();
        if (!GameManager.isPaused && !GameManager.menuOpen)
        {

            InputBuffer(3);
        }

    }
    public void OnEast(InputAction.CallbackContext context)
    {
        ChangeControlScheme(context);
        //Debug.Log(GameManager.isPaused + "" + GameManager.menuOpen);
        if (!context.performed) return;

        if (debug) print("O");
        eastInput?.Invoke();

        if (!GameManager.isPaused && !GameManager.menuOpen)
        {
            //if (inputDirection.y > 0)
            //{
            //    InputBuffer(84);
            //}
            //else
            InputBuffer(4);
        }
    }
    public void OnEast()
    {

        eastInput?.Invoke();

        if (!GameManager.isPaused && !GameManager.menuOpen)
        {
            InputBuffer(4);
        }
    }
    public void OnStart()
    {
        startInput?.Invoke();
    }

    public void OnSelect()
    {
        selectInput?.Invoke();
    }

    public void OnR1(InputAction.CallbackContext context)
    {
        if (debug) print("R1");
        R1input?.Invoke();
        tapCounter = 0;
        R1Hold = true;
        if (!dodgeTap)
            InputBuffer(5);
    }

    void OnR1Release()
    {
        R1release?.Invoke();
        if (dodgeTap)
            if (tapCounter < tapFrames)
            {
                if (debug) Debug.Log(tapCounter);
                InputBuffer(5);
            }
        R1Hold = false;
    }

    void OnSouthRelease()
    {
        southRelease?.Invoke();
        R1Hold = false;
    }

    public void OnL1(InputAction.CallbackContext context)
    {
        //print(context.ReadValueAsButton());
        if (debug) print("L1");
        L1input?.Invoke();
        InputBuffer(6);
    }

    void OnR2Press()
    {
        if (R2Hold) return;
        R2Hold = true;
        R2input?.Invoke();
    }

    void OnR2Release()
    {
        if (!R2Hold) return;
        R2Hold = false;
        R2release?.Invoke();
    }

    void OnL2Press()
    {
        if (L2Hold) return;
        L2Hold = true;
        L2input?.Invoke();
    }

    void OnL2Release()
    {
        if (!L2Hold) return;
        L2release?.Invoke();
        L2Hold = false;
    }

    void OnL1Release()
    {
        L1release?.Invoke();
    }
    void OnL3()
    {
        if (debug) print("L3");
        L3input?.Invoke();
    }
    void OnR3()
    {
        if (debug) print("R3");
        R3input?.Invoke();
    }

    void On1()
    {
        if (GameManager.isPaused) return;
        InputBuffer(7);
    }
    void On2()
    {
        if (GameManager.isPaused) return;
        InputBuffer(8);
    }
    void On3()
    {
        if (GameManager.isPaused) return;
        InputBuffer(9);
    }
    void On4()
    {
        if (GameManager.isPaused) return;
        InputBuffer(10);
    }
    public int Direction()
    {
        if (inputDirection.y > 0.7) return 8;
        if (inputDirection.y < -0.7) return 2;
        if (inputDirection.x > 0.7) return 6;
        if (inputDirection.x < -0.7) return 4;
        return 5;

        //if (netDirectionals[0])
        //{
        //    if (id == 1)
        //        return 8;
        //    else return 2;
        //}
        //else if (netDirectionals[2])
        //{
        //    if (id == 1)
        //        return 2;
        //    else return 8;
        //}
        //else if (netDirectionals[1])
        //{
        //    if (id == 1)
        //        return 6;
        //    else return 4;
        //}
        //else if (netDirectionals[3])
        //{
        //    if (id == 1)
        //        return 4;
        //    else return 6;
        //}
        //else
        //    return 5;
    }
    public void InputBuffer(int inputID)
    {
        // if (GameManager.isPaused) yield break;
        BufferedInput temp = new BufferedInput(inputID, Direction(), bufferWindow);
        bufferedInputs.Add(temp);
    }
}
[System.Serializable]
public class BufferedInput
{
    public BufferedInput(int input, int direction, int bufferWindow)
    {
        id = input;
        frame = bufferWindow;
        dir = direction;

    }
    public int id;
    public int dir;
    public int frame;
}

public enum ControlScheme { PS4, XBOX, Keyboard, Switch, Mouse }
