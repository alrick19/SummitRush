using UnityEngine;

public static class InputManager
{
    public static float GetHorizontal()
    {
        return Input.GetAxisRaw("Horizontal"); // -1 (left), 1 (right), or 0 (idle)
    }

    public static float GetVertical()
    {
        return Input.GetAxisRaw("Vertical"); // - 1 (down), 1 (up), or 0 (idle)
    }

    public static bool GetJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space); // Detects jump press
    }

    public static bool GetJumpHeld()
    {
        return Input.GetKey(KeyCode.Space); // Detects jump being held
    }

    public static bool GetJumpReleased()
    {
        return Input.GetKeyUp(KeyCode.Space); // Detects jump release
    }

    public static bool GetWallGrab()
    {
        return Input.GetKey(KeyCode.LeftShift); // Detects Wall Grab Hold
    }

    public static bool GetDash()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }
}