using UnityEngine;

public class InspectOnGaze : MonoBehaviour
{
    public float degreesPerSecond = 35f;
    public bool looking;

    void Update()
    {
        if (!looking)
            return;

        transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f, Space.World);
    }
}
