using UnityEngine;

public class CourseManager : MonoBehaviour
{
    public static CourseManager Instance { get; private set; }

    public int totalCrystals = 5;
    public int collected;
    public bool finished;

    private string status = "Collect every crystal, then step into the golden beacon. WASD to move, mouse to look, Space to jump.";

    void Awake()
    {
        Instance = this;
    }

    public void CollectCrystal()
    {
        collected++;
        if (collected >= totalCrystals)
            status = "All crystals collected. Reach the golden beacon to finish.";
        else
            status = "Crystal collected. " + (totalCrystals - collected) + " left.";
    }

    public void TryFinish()
    {
        if (finished)
            return;

        if (collected < totalCrystals)
        {
            status = "The beacon is locked. Crystals left: " + (totalCrystals - collected) + ".";
            return;
        }

        finished = true;
        status = "Level complete! You collected the crystals and reached the beacon.";
    }

    void OnGUI()
    {
        GUI.Box(new Rect(16, 16, 640, 88), "");
        GUI.Label(new Rect(28, 24, 610, 28), "Crystals: " + collected + " / " + totalCrystals);
        GUI.Label(new Rect(28, 52, 610, 40), finished ? "Level complete!" : status);
    }
}
