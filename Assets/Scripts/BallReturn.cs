using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BallReturn : MonoBehaviour
{
    public GameObject ballPrefab;
    public Vector3 frontCenter;
    public Vector3 backCenter;
    public int capacity = 4;

    static readonly Color[] Colors =
    {
        new Color(0.78f, 0.12f, 0.1f),
        new Color(0.12f, 0.32f, 0.78f),
        new Color(0.16f, 0.55f, 0.28f),
        new Color(0.92f, 0.48f, 0.08f),
        new Color(0.48f, 0.18f, 0.66f),
        new Color(0.1f, 0.1f, 0.12f),
        new Color(0.9f, 0.78f, 0.15f)
    };

    readonly List<Rigidbody> balls = new List<Rigidbody>();
    int nextColor;

    public void Refill()
    {
        for (int i = balls.Count - 1; i >= 0; i--)
        {
            Rigidbody body = balls[i];
            if (body == null)
                continue;
            Release(body.gameObject);
            body.gameObject.SetActive(false);
            Destroy(body.gameObject);
        }

        balls.Clear();
        for (int i = 0; i < capacity; i++)
        {
            float along = 0.16f + i * 0.24f;
            Vector3 position = frontCenter + (backCenter - frontCenter).normalized * along;
            position.y = Mathf.Lerp(frontCenter.y, backCenter.y, along / Vector3.Distance(frontCenter, backCenter));
            Spawn(position, Vector3.zero);
        }
    }

    void Awake()
    {
        if (balls.Count == 0 && ballPrefab != null)
            Refill();
    }

    void Spawn(Vector3 position, Vector3 velocity)
    {
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
        ball.name = "Ball";
        Tint(ball, Colors[nextColor % Colors.Length]);
        nextColor++;

        Rigidbody body = ball.GetComponent<Rigidbody>();
        if (body == null)
            body = ball.AddComponent<Rigidbody>();
        body.mass = 5.5f;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.linearDamping = 0.05f;
        body.angularDamping = 0.35f;
        body.linearVelocity = velocity;
        body.angularVelocity = Vector3.zero;

        SphereCollider sphere = ball.GetComponent<SphereCollider>();
        if (sphere == null)
            sphere = ball.AddComponent<SphereCollider>();
        sphere.radius = 0.108f;
        sphere.center = Vector3.zero;

        MeshCollider[] meshes = ball.GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < meshes.Length; i++)
            DestroyImmediate(meshes[i]);

        XRGrabInteractable grab = ball.GetComponent<XRGrabInteractable>();
        if (grab == null)
            grab = ball.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grab.throwOnDetach = true;
        grab.throwVelocityScale = 1.6f;
        grab.throwAngularVelocityScale = 1f;
        grab.selectEntered.AddListener(OnTaken);

        balls.Add(body);
    }

    void OnTaken(SelectEnterEventArgs args)
    {
        Rigidbody taken = args.interactableObject.transform.GetComponent<Rigidbody>();
        if (CountOnRack(taken) < capacity)
        {
            Vector3 down = (frontCenter - backCenter).normalized;
            Spawn(backCenter, down * 0.65f);
        }
    }

    void LateUpdate()
    {
        int loose = 0;
        for (int i = 0; i < balls.Count; i++)
        {
            Rigidbody body = balls[i];
            if (body != null && !IsHeld(body) && !IsOnRack(body))
                loose++;
        }

        if (loose <= 2)
            return;

        for (int i = 0; i < balls.Count; i++)
        {
            Rigidbody body = balls[i];
            if (body == null || IsHeld(body) || IsOnRack(body))
                continue;
            if (body.linearVelocity.sqrMagnitude > 0.2f)
                continue;
            balls.RemoveAt(i);
            Destroy(body.gameObject);
            return;
        }
    }

    int CountOnRack(Rigidbody except)
    {
        int count = 0;
        for (int i = 0; i < balls.Count; i++)
        {
            Rigidbody body = balls[i];
            if (body == null || body == except)
                continue;
            if (IsOnRack(body))
                count++;
        }
        return count;
    }

    bool IsOnRack(Rigidbody body)
    {
        if (IsHeld(body))
            return false;
        Vector3 position = body.position;
        return Mathf.Abs(position.x - frontCenter.x) < 0.5f
            && position.z <= frontCenter.z + 0.4f
            && position.z >= backCenter.z - 0.45f
            && position.y > 0.35f
            && position.y < 1.5f;
    }

    static bool IsHeld(Rigidbody body)
    {
        XRGrabInteractable grab = body.GetComponent<XRGrabInteractable>();
        return grab != null && grab.isSelected;
    }

    static void Release(GameObject ball)
    {
        XRGrabInteractable grab = ball.GetComponent<XRGrabInteractable>();
        if (grab != null && grab.isSelected)
            grab.interactionManager.SelectExit(grab.interactorsSelecting[0], grab);
    }

    static void Tint(GameObject ball, Color color)
    {
        Renderer[] renderers = ball.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].name.IndexOf("Hole") >= 0)
                continue;
            Material[] materials = renderers[i].materials;
            for (int m = 0; m < materials.Length; m++)
            {
                if (materials[m].HasProperty("baseColorFactor"))
                {
                    materials[m].SetTexture("baseColorTexture", null);
                    materials[m].SetColor("baseColorFactor", color);
                    if (materials[m].HasProperty("metallicFactor"))
                        materials[m].SetFloat("metallicFactor", 0.15f);
                    if (materials[m].HasProperty("roughnessFactor"))
                        materials[m].SetFloat("roughnessFactor", 0.45f);
                }
                if (materials[m].HasProperty("_BaseColor"))
                    materials[m].SetColor("_BaseColor", color);
            }
        }
    }
}
