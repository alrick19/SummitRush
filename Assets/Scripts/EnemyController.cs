using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPoint;
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPoint = pointB.transform;
        anim.SetBool("isRunning", true);
    }

    // Update is called once per frame
    void Update()
    {
        // Vector2 point = currentPoint.position - transform.position;
        // if(currentPoint == pointB.transform)
        // {
        //     rb.linearVelocity = new Vector2(speed, 0);
        // }else
        // {
        //     rb.linearVelocity = new Vector2(-speed, 0);
        // }
        // if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        // {
        //     Flip();
        //     currentPoint = pointA.transform;    
        // }
        // if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        // {
        //     Flip();
        //     currentPoint = pointB.transform;    
        // }
        Vector2 direction = (currentPoint.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            Flip();
            currentPoint = currentPoint == pointB.transform ? pointA.transform : pointB.transform;
        }

    }
    private void Flip()
    {
        // Vector3 localScale = transform.localScale;
        // localScale.x *= -1;
        // transform.localScale = localScale;
        if (currentPoint.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.2f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.2f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
}
