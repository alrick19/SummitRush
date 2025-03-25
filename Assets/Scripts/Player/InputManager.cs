using UnityEngine;

public static class InputManager
{
    private static bool inputLocked = false;

    public static void LockInput() => inputLocked = true;
    public static void UnlockInput() => inputLocked = false;

    public static float GetHorizontal()
    {
        if (inputLocked) return 0f;
        return Input.GetAxisRaw("Horizontal");
    }

    public static float GetVertical()
    {
        if (inputLocked) return 0f;
        return Input.GetAxisRaw("Vertical");
    }

    public static bool GetJumpInput()
    {
        return !inputLocked && Input.GetKeyDown(KeyCode.Space);
    }

    public static bool GetJumpHeld()
    {
        return !inputLocked && Input.GetKey(KeyCode.Space);
    }

    public static bool GetJumpReleased()
    {
        return !inputLocked && Input.GetKeyUp(KeyCode.Space);
    }

    public static bool GetWallGrab()
    {
        return !inputLocked && Input.GetKey(KeyCode.LeftShift);
    }

    public static bool GetDash()
    {
        return !inputLocked && Input.GetKey(KeyCode.Mouse0);
    }
}