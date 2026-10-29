using UnityEngine;

public class FinishBeacon : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        CourseManager.Instance.TryFinish();
    }
}
