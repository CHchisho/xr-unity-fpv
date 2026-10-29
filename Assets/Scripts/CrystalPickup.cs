using UnityEngine;

public class CrystalPickup : MonoBehaviour
{
    private float baseY;

    void Start()
    {
        baseY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
        Vector3 position = transform.position;
        position.y = baseY + Mathf.Sin(Time.time * 2f + baseY) * 0.25f;
        transform.position = position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        CourseManager.Instance.CollectCrystal();
        Destroy(gameObject);
    }
}
