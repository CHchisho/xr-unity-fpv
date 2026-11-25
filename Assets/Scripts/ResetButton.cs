using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ResetButton : MonoBehaviour
{
    public Transform cap;
    public BowlingMatch match;
    public float pressDistance = 0.018f;

    Vector3 restLocal;
    float shown;
    float releaseAt;

    void Awake()
    {
        if (cap == null)
            cap = transform;
        restLocal = cap.localPosition;

        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnPressed);
    }

    void OnPressed(SelectEnterEventArgs args)
    {
        shown = 1f;
        releaseAt = Time.time + 0.28f;
        if (match != null)
            match.ResetLane();

        if (args.interactorObject is Component component)
        {
            HapticImpulsePlayer player = component.GetComponentInParent<HapticImpulsePlayer>();
            if (player != null)
                player.SendHapticImpulse(0.45f, 0.08f);
        }

        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        if (interactable.isSelected)
            interactable.interactionManager.SelectExit(args.interactorObject, interactable);
    }

    void Update()
    {
        if (shown > 0f && Time.time >= releaseAt)
            shown = Mathf.MoveTowards(shown, 0f, Time.deltaTime * 4f);

        cap.localPosition = restLocal + new Vector3(0f, 0f, -pressDistance * shown);
    }
}
