using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAxisControl : MonoBehaviour
{
    private Controls controls;

    // Controller Axis
    private float AxisInputX;
    private float AxisInputY;

    // Keyboard Axis
    private float KeyboardUp;
    private float KeyboardDown;
    private float KeyboardLeft;
    private float KeyboardRight;

    private void Awake()
    {
        controls = new Controls();

        switch (Config.controlConfig)
        {
            case ControlConfig.Arrows:
                SetArrowControls();
                GetComponent<PlayerInput>().SwitchCurrentActionMap("Gameplay");
                controls.Gameplay.Enable();
                controls.GameplayWASD.Disable();
                break;

            case ControlConfig.WASD:
                SetWASDControls();
                GetComponent<PlayerInput>().SwitchCurrentActionMap("GameplayWASD");
                controls.GameplayWASD.Enable();
                controls.Gameplay.Disable();
                break;
        }
    }

    private void SetArrowControls()
    {
        controls.Gameplay.MoveX.performed += ctx => AxisInputX = ctx.ReadValue<float>();

        controls.Gameplay.KeyboardLeft.performed += ctx => KeyboardLeft = ctx.ReadValue<float>();
        controls.Gameplay.KeyboardLeft.canceled += ctx => KeyboardLeft = 0;

        controls.Gameplay.KeyboardRight.performed += ctx => KeyboardRight = ctx.ReadValue<float>();
        controls.Gameplay.KeyboardRight.canceled += ctx => KeyboardRight = 0;

        controls.Gameplay.MoveY.performed += ctx => AxisInputY = ctx.ReadValue<float>();

        controls.Gameplay.KeyboardUp.performed += ctx => KeyboardUp = ctx.ReadValue<float>();
        controls.Gameplay.KeyboardUp.canceled += ctx => KeyboardUp = 0;

        controls.Gameplay.KeyboardDown.performed += ctx => KeyboardDown = ctx.ReadValue<float>();
        controls.Gameplay.KeyboardDown.canceled += ctx => KeyboardDown = 0;
    }

    private void SetWASDControls()
    {
        controls.GameplayWASD.MoveX.performed += ctx => AxisInputX = ctx.ReadValue<float>();

        controls.GameplayWASD.KeyboardLeft.performed += ctx => KeyboardLeft = ctx.ReadValue<float>();
        controls.GameplayWASD.KeyboardLeft.canceled += ctx => KeyboardLeft = 0;

        controls.GameplayWASD.KeyboardRight.performed += ctx => KeyboardRight = ctx.ReadValue<float>();
        controls.GameplayWASD.KeyboardRight.canceled += ctx => KeyboardRight = 0;

        controls.GameplayWASD.MoveY.performed += ctx => AxisInputY = ctx.ReadValue<float>();

        controls.GameplayWASD.KeyboardUp.performed += ctx => KeyboardUp = ctx.ReadValue<float>();
        controls.GameplayWASD.KeyboardUp.canceled += ctx => KeyboardUp = 0;

        controls.GameplayWASD.KeyboardDown.performed += ctx => KeyboardDown = ctx.ReadValue<float>();
        controls.GameplayWASD.KeyboardDown.canceled += ctx => KeyboardDown = 0;
    }

    #region Getters
    public float GetAxisInputX()
    {
        return AxisInputX;
    }

    public float GetAxisInputY()
    {
        return AxisInputY;
    }

    public float GetKeyboardUp()
    {
        return KeyboardUp;
    }

    public float GetKeyboardDown()
    {
        return KeyboardDown;
    }

    public float GetKeyboardLeft()
    {
        return KeyboardLeft;
    }

    public float GetKeyboardRight()
    {
        return KeyboardRight;
    }
    #endregion
}
