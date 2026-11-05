using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraRayController : MonoBehaviour
{
    Text infoText;
    GameObject infoBox;
    Slider delaySlider;
    Image reticle;
    InspectOnGaze currentInspect;
    float delayTimer = 0f;

    void Start()
    {
        infoText = GameObject.Find("InfoText").GetComponent<Text>();
        infoBox = GameObject.Find("InfoBox");
        infoText.enabled = false;
        if (infoBox != null)
            infoBox.SetActive(false);

        delaySlider = GameObject.Find("DelaySlider").GetComponent<Slider>();
        delaySlider.gameObject.SetActive(false);

        GameObject reticleObject = GameObject.Find("Reticle");
        if (reticleObject != null)
            reticle = reticleObject.GetComponent<Image>();
    }

    void Update()
    {
        Vector3 rayDirection = transform.forward;
        Debug.DrawRay(transform.position, rayDirection * 20f, Color.green);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, rayDirection, out hit, 50f))
        {
            if (hit.collider.CompareTag("InfoSpot"))
            {
                DisplayInfoText(hit.collider.gameObject.name);
                SetInspect(hit.collider.GetComponentInParent<InspectOnGaze>());
                delayTimer = 0f;
                delaySlider.gameObject.SetActive(false);
                SetReticle(new Color(0.3f, 0.85f, 1f));
            }
            else if (hit.collider.CompareTag("TeleportSpot"))
            {
                HideInfo();
                SetInspect(null);
                MoveToScene(hit.collider.gameObject.name);
                SetReticle(new Color(1f, 0.78f, 0.2f));
            }
            else
            {
                ResetHit();
            }
        }
        else
        {
            ResetHit();
        }
    }

    void DisplayInfoText(string gameObjectName)
    {
        if (infoBox != null)
            infoBox.SetActive(true);
        infoText.enabled = true;

        if (gameObjectName == "Sneaker")
            infoText.text = "Photogrammetry sneaker\nScanned shoe from the media lab. Keep looking and the model turns so you can inspect it.";
        else if (gameObjectName == "SneakerKiri")
            infoText.text = "KIRI sneaker\nShoe scan from KIRI Engine. Keep looking and this model turns as well.";
        else if (gameObjectName == "PanoramaMarker")
            infoText.text = "Stitched lab panorama\nA different photo from the Ricoh room. Look at the portal for two seconds to go back.";
        else
            infoText.text = "This is a default message.";
    }

    void MoveToScene(string sceneName)
    {
        delayTimer = delayTimer + Time.deltaTime;
        delaySlider.gameObject.SetActive(true);
        delaySlider.value = delayTimer;
        if (delayTimer > 2f)
            SceneManager.LoadScene(sceneName);
    }

    void ResetHit()
    {
        HideInfo();
        delaySlider.gameObject.SetActive(false);
        delayTimer = 0f;
        SetInspect(null);
        SetReticle(Color.white);
    }

    void HideInfo()
    {
        infoText.enabled = false;
        if (infoBox != null)
            infoBox.SetActive(false);
    }

    void SetInspect(InspectOnGaze next)
    {
        if (currentInspect != null && currentInspect != next)
            currentInspect.looking = false;
        currentInspect = next;
        if (currentInspect != null)
            currentInspect.looking = true;
    }

    void SetReticle(Color color)
    {
        if (reticle != null)
            reticle.color = color;
    }
}
