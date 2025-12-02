using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class UIUtility
{
    public static bool IsPointerOverUI()
    {
        // Touch input
        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.primaryTouch.press.isPressed)
            {
                int fingerId = Touchscreen.current.primaryTouch.touchId.ReadValue();
                if (EventSystem.current.IsPointerOverGameObject(fingerId))
                    return true;
            }
        }

        // Mouse input
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return true;
        }

        return false;
    }
}
