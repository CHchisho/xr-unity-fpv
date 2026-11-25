using UnityEngine;
using UnityEngine.UI;

public class BowlingMatch : MonoBehaviour
{
    public BowlingPin[] pins;
    public BallReturn ballReturn;
    public Text scoreText;

    Vector3[] pinPositions;
    Quaternion[] pinRotations;

    void Awake()
    {
        pinPositions = new Vector3[pins.Length];
        pinRotations = new Quaternion[pins.Length];
        for (int i = 0; i < pins.Length; i++)
        {
            pinPositions[i] = pins[i].transform.position;
            pinRotations[i] = pins[i].transform.rotation;
        }

    }

    void Update()
    {
        int fallen = 0;
        for (int i = 0; i < pins.Length; i++)
        {
            if (pins[i].IsFallen)
                fallen++;
        }

        if (scoreText == null)
            return;

        string headline = fallen == 0 ? "Take a ball from the return." : fallen + " / " + pins.Length + " pins down";
        if (fallen == pins.Length)
            headline = "Strike. Press RESET.";
        scoreText.text = headline + "\nBalls roll up when you take one.\nKnocked pins drop into the pit.";
    }

    public void ResetLane()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            Rigidbody body = pins[i].GetComponent<Rigidbody>();
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.position = pinPositions[i];
            body.rotation = pinRotations[i];
        }

        if (ballReturn != null)
            ballReturn.Refill();
    }
}
