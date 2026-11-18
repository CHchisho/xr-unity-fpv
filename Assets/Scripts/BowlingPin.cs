using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class BowlingPin : MonoBehaviour
{
    float nextHaptic;

    public bool IsFallen
    {
        get { return Vector3.Angle(transform.up, Vector3.up) > 40f; }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Time.time < nextHaptic || collision.relativeVelocity.magnitude < 1.1f)
            return;

        nextHaptic = Time.time + 0.2f;
        float amplitude = Mathf.Clamp(collision.relativeVelocity.magnitude / 6f, 0.15f, 0.7f);
        HapticImpulsePlayer[] players = Object.FindObjectsByType<HapticImpulsePlayer>(FindObjectsSortMode.None);
        for (int i = 0; i < players.Length; i++)
            players[i].SendHapticImpulse(amplitude, 0.08f);
    }
}
