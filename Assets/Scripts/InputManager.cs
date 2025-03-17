using UnityEngine;

public static class InputManager
{
    public static float GetMoveInput()
    {
        return Input.GetAxisRaw("Horizontal"); // Returns -1, 0, or 1 (Left, no input, right)
    }
}