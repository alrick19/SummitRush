using System.Collections;
using UnityEngine;

public class Icicle : MonoBehaviour, IResettableHazard
{
    private Vector2 startPos;
    private Rigidbody2D rb;
    private bool hasFallen = false;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();

        RoomHazardManager room = GetComponentInParent<RoomHazardManager>();
        if (room != null)
        {
            room.Register(this); // Register this icicle in the room's hazard manager
        }


    }

    public void TriggerFall()
    {
        if (hasFallen) return;
        hasFallen = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 4f;
    }

    public void ResetHazard()
    {
        hasFallen = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        transform.position = startPos;
        gameObject.SetActive(true);
    }

}