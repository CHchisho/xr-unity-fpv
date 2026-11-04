using UnityEngine;

public class CameraMouseControl : MonoBehaviour
{
    public float sensitivyX = 400f;
    public float sensitivyY = 400f;
    private float xRotation;
    private float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Vector3 euler = transform.eulerAngles;
        yRotation = euler.y;
        xRotation = euler.x;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivyX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivyY;
        yRotation = yRotation + mouseX;
        xRotation = xRotation - mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
