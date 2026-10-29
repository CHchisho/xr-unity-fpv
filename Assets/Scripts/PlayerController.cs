using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce = 7f;
    public Transform orientation;
    private float xInput;
    private float yInput;
    private Vector3 movingDirection;
    private Rigidbody rb;
    private bool jumpRequested;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Read input (keyboard)
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            jumpRequested = true;
    }

    void FixedUpdate()
    {
        // Calculate correct direction
        movingDirection = orientation.forward * yInput + orientation.right * xInput;
        // Move to correct direction
        rb.AddForce(movingDirection.normalized * speed, ForceMode.Force);
        // Do not exceed maximum speed
        Vector3 groundVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (groundVelocity.magnitude > speed)
        {
            Vector3 maxVelocity = groundVelocity.normalized * speed;
            rb.linearVelocity = new Vector3(maxVelocity.x, rb.linearVelocity.y, maxVelocity.z);
        }

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }
    }

    bool IsGrounded()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 1.2f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                continue;
            return true;
        }

        return false;
    }
}
