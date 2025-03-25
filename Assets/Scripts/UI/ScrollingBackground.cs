using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float scrollSpeed = 1f; 
    public float backgroundWidth = 37; 

    private Transform[] backgrounds;

    private void Start()
    {
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            backgrounds[i] = transform.GetChild(i);
        }
    }

    private void Update()
    {
        foreach (Transform bg in backgrounds)
        {
            bg.position += Vector3.left * scrollSpeed * Time.deltaTime;

            // when background has moved completely off screen, move it to the right end
            if (bg.position.x <= -backgroundWidth)
            {
                float rightmostX = GetRightmostBackgroundX();
                bg.position = new Vector3(rightmostX + backgroundWidth, bg.position.y, bg.position.z);
            }
        }
    }

    private float GetRightmostBackgroundX()
    {
        float maxX = float.MinValue;
        foreach (Transform bg in backgrounds)
        {
            if (bg.position.x > maxX)
                maxX = bg.position.x;
        }
        return maxX;
    }
}