using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Action<Vector2> onTouchInput;

    private bool canInput = true;
    public Vector2 RawMovementInput { get; private set; }
    public int NormaInputX { get; set; }
    public int NormaInputY { get; set; }

    public bool FireInput { get; set; }

    public Vector2 TurnInput { get; private set; }

    public void SetCanInput(bool val)
    {
        canInput = val;
        StopInput();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (!canInput)
        {
            return;
        }
        RawMovementInput = context.ReadValue<Vector2>();
        NormaInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        NormaInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
    }

    public void OnTurnInput(InputAction.CallbackContext context)
    {
        if (!canInput)
        {
            return;
        }
        TurnInput = context.ReadValue<Vector2>();
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {
        if (!canInput)
        {
            return;
        }
        if (context.started)
        {
            FireInput = true;
        }
        if (context.canceled)
        {
            FireInput = false;
        }
    }


    public void OnPoint(InputAction.CallbackContext context)
    {
        if (!canInput)
        {
            return;
        }
        if (context.performed)
        {
            var pos = context.ReadValue<Vector2>();
            onTouchInput?.Invoke(pos);
        }
    }

    public bool IsScreenBeingTouched() =>
        Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;

    public void StopInput()
    {
        NormaInputX = 0;
        NormaInputY = 0;
        FireInput = false;
    }

}
