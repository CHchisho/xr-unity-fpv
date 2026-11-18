using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BowlingMatch : MonoBehaviour
{
    public BowlingPin[] pins;
    public Rigidbody ballBody;
    public XRGrabInteractable ballGrab;
    public XRSimpleInteractable resetButton;
    public Text scoreText;

    Vector3[] pinPositions;
    Quaternion[] pinRotations;
    Vector3 ballPosition;
    Quaternion ballRotation;

    void Awake()
    {
        pinPositions = new Vector3[pins.Length];
        pinRotations = new Quaternion[pins.Length];
        for (int i = 0; i < pins.Length; i++)
        {
            pinPositions[i] = pins[i].transform.position;
            pinRotations[i] = pins[i].transform.rotation;
        }

        ballPosition = ballBody.position;
        ballRotation = ballBody.rotation;

        if (resetButton != null)
            resetButton.selectEntered.AddListener(OnResetSelected);
        if (ballGrab != null)
            ballGrab.selectEntered.AddListener(OnBallGrabbed);
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

        string headline = fallen == 0 ? "Roll the ball." : fallen + " / " + pins.Length + " pins down";
        if (fallen == pins.Length)
            headline = "Strike! Press the red button.";
        scoreText.text = headline + "\nLeft stick move, right stick turn.\nGrip grabs the ball. Trigger points to teleport or the reset button.";
    }

    void OnResetSelected(SelectEnterEventArgs args)
    {
        Pulse(args.interactorObject, 0.45f, 0.1f);
        ResetLane();
    }

    void OnBallGrabbed(SelectEnterEventArgs args)
    {
        Pulse(args.interactorObject, 0.25f, 0.05f);
    }

    public void ResetLane()
    {
        if (ballGrab != null && ballGrab.isSelected)
        {
            IXRSelectInteractor interactor = ballGrab.interactorsSelecting[0];
            ballGrab.interactionManager.SelectExit(interactor, ballGrab);
        }

        for (int i = 0; i < pins.Length; i++)
        {
            Rigidbody body = pins[i].GetComponent<Rigidbody>();
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.position = pinPositions[i];
            body.rotation = pinRotations[i];
        }

        ballBody.linearVelocity = Vector3.zero;
        ballBody.angularVelocity = Vector3.zero;
        ballBody.position = ballPosition;
        ballBody.rotation = ballRotation;
    }

    static void Pulse(IXRInteractor interactor, float amplitude, float duration)
    {
        if (interactor is not Component component)
            return;
        HapticImpulsePlayer player = component.GetComponentInParent<HapticImpulsePlayer>();
        if (player != null)
            player.SendHapticImpulse(amplitude, duration);
    }
}
