using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public Vector3 delta = new Vector3(8f, 0f, 0f);
    public float period = 4f;

    private Vector3 start;
    private Rigidbody rb;

    void Start()
    {
        start = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time, period) / period;
        Vector3 next = Vector3.Lerp(start, start + delta, t);
        if (rb != null)
            rb.MovePosition(next);
        else
            transform.position = next;
    }
}
