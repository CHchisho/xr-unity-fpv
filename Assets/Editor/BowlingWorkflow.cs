using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public static class BowlingWorkflow
{
    [MenuItem("Tools/Install Bowling Workflow")]
    public static void InstallMenu()
    {
        Debug.Log(Install());
    }

    public static string Install()
    {
        Replace("PinPit");
        Replace("BallReturn");
        Replace("ResetConsole");
        Replace("RoomFloorHole");

        GameObject oldBall = GameObject.Find("Ball");
        if (oldBall != null)
            Object.DestroyImmediate(oldBall);
        GameObject oldButton = GameObject.Find("ResetButton");
        if (oldButton != null)
            Object.DestroyImmediate(oldButton);

        OpenPit();
        CreateReturn();
        CreateButton();
        EditorSceneManagerMarkDirty();
        return "Bowling workflow installed";
    }

    static void OpenPit()
    {
        ShortenCollider("Lane", -9.55f);
        ShortenCollider("GutterLeft", -9.55f);
        ShortenCollider("GutterRight", -9.55f);

        GameObject backstop = GameObject.Find("Backstop");
        if (backstop != null)
        {
            Collider backCollider = backstop.GetComponent<Collider>();
            if (backCollider != null)
                backCollider.enabled = false;
            Renderer backRenderer = backstop.GetComponent<Renderer>();
            if (backRenderer != null)
                backRenderer.enabled = false;
        }

        GameObject floor = GameObject.Find("RoomFloor");
        if (floor != null)
        {
            Collider floorCollider = floor.GetComponent<Collider>();
            if (floorCollider != null)
                floorCollider.enabled = false;
            GameObject hole = new GameObject("RoomFloorHole");
            hole.transform.SetParent(floor.transform, false);
            AddBox(hole.transform, "FloorLeft", new Vector3(-2.42f, -0.1f, -3.25f), new Vector3(3.15f, 0.16f, 17.5f));
            AddBox(hole.transform, "FloorRight", new Vector3(2.42f, -0.1f, -3.25f), new Vector3(3.15f, 0.16f, 17.5f));
            AddBox(hole.transform, "FloorNear", new Vector3(0f, -0.1f, -2f), new Vector3(1.7f, 0.16f, 15f));
            AddBox(hole.transform, "FloorFar", new Vector3(0f, -0.1f, -11.72f), new Vector3(1.7f, 0.16f, 0.55f));
        }

        GameObject pit = new GameObject("PinPit");
        Material rubber = MakeMaterial("PitRubber", new Color(0.08f, 0.08f, 0.09f), 0.15f);
        Material wall = MakeMaterial("PitWall", new Color(0.16f, 0.16f, 0.17f), 0.25f);

        GameObject ramp = AddBox(pit.transform, "PitRamp", new Vector3(0f, -0.18f, -10.15f), new Vector3(1.35f, 0.06f, 0.7f), rubber);
        ramp.transform.rotation = Quaternion.Euler(-22f, 0f, 0f);
        Vector3 lowEnd = ramp.transform.TransformPoint(0f, 0.5f, -0.5f);
        Vector3 highEnd = ramp.transform.TransformPoint(0f, 0.5f, 0.5f);
        if (lowEnd.y > highEnd.y)
            ramp.transform.rotation = Quaternion.Euler(22f, 0f, 0f);

        AddBox(pit.transform, "PitFloor", new Vector3(0f, -0.48f, -10.7f), new Vector3(1.45f, 0.08f, 1.15f), rubber);
        AddBox(pit.transform, "PitWallLeft", new Vector3(-0.78f, -0.12f, -10.7f), new Vector3(0.08f, 0.7f, 1.35f), wall);
        AddBox(pit.transform, "PitWallRight", new Vector3(0.78f, -0.12f, -10.7f), new Vector3(0.08f, 0.7f, 1.35f), wall);
        AddBox(pit.transform, "PitWallBack", new Vector3(0f, -0.05f, -11.32f), new Vector3(1.6f, 0.9f, 0.08f), wall);
    }

    static void CreateReturn()
    {
        GameObject root = new GameObject("BallReturn");
        Material housing = MakeMaterial("ReturnHousing", new Color(0.18f, 0.18f, 0.2f), 0.35f);
        Material rail = MakeMaterial("ReturnRail", new Color(0.42f, 0.44f, 0.46f), 0.55f);
        PhysicsMaterial slick = new PhysicsMaterial("ReturnSlide");
        slick.dynamicFriction = 0.04f;
        slick.staticFriction = 0.04f;
        slick.frictionCombine = PhysicsMaterialCombine.Minimum;
        slick.bounceCombine = PhysicsMaterialCombine.Minimum;
        slick.bounciness = 0.02f;

        GameObject body = AddBox(root.transform, "ReturnBody", new Vector3(1.38f, 0.32f, 1.4f), new Vector3(0.46f, 0.64f, 2.05f), housing);
        body.GetComponent<Collider>().material = slick;

        GameObject track = AddBox(root.transform, "ReturnTrack", new Vector3(1.38f, 0.7f, 1.4f), new Vector3(0.32f, 0.03f, 1.85f), rail);
        track.transform.rotation = Quaternion.Euler(6f, 0f, 0f);
        track.GetComponent<Collider>().material = slick;

        Vector3 front = track.transform.TransformPoint(0f, 0.5f, 0.5f);
        Vector3 back = track.transform.TransformPoint(0f, 0.5f, -0.5f);
        if (front.z < back.z)
        {
            Vector3 swap = front;
            front = back;
            back = swap;
        }

        Vector3 along = (front - back).normalized;
        Vector3 up = track.transform.up;
        GameObject stop = AddBox(root.transform, "ReturnStop", front + along * 0.08f + up * 0.05f, new Vector3(0.34f, 0.12f, 0.05f), housing);
        stop.transform.rotation = track.transform.rotation;
        Vector3 leftRail = track.transform.TransformPoint(-0.5f, 1.6f, 0f);
        Vector3 rightRail = track.transform.TransformPoint(0.5f, 1.6f, 0f);
        AddBox(root.transform, "ReturnRailLeft", leftRail, new Vector3(0.035f, 0.07f, 1.85f), housing).transform.rotation = track.transform.rotation;
        AddBox(root.transform, "ReturnRailRight", rightRail, new Vector3(0.035f, 0.07f, 1.85f), housing).transform.rotation = track.transform.rotation;

        BallReturn rack = root.AddComponent<BallReturn>();
        rack.ballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Bowling/Ball.glb");
        rack.frontCenter = front + up * 0.12f - along * 0.2f;
        rack.backCenter = back + up * 0.12f + along * 0.1f;
        rack.capacity = 4;

        BowlingMatch match = Object.FindFirstObjectByType<BowlingMatch>();
        if (match != null)
            match.ballReturn = rack;
    }

    static void CreateButton()
    {
        GameObject console = new GameObject("ResetConsole");
        console.transform.position = new Vector3(-1.22f, 0f, 1.72f);
        Material housing = MakeMaterial("ButtonHousing", new Color(0.14f, 0.15f, 0.17f), 0.4f);
        Material panel = MakeMaterial("ButtonPanel", new Color(0.23f, 0.25f, 0.28f), 0.45f);
        Material capMat = MakeMaterial("ButtonCap", new Color(0.8f, 0.12f, 0.1f), 0.35f);

        AddBox(console.transform, "ConsoleBase", new Vector3(0f, 0.5f, 0f), new Vector3(0.32f, 1f, 0.36f), housing);
        AddBox(console.transform, "ConsolePanel", new Vector3(0.15f, 1.05f, 0f), new Vector3(0.04f, 0.46f, 0.34f), panel);

        GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cap.name = "ResetCap";
        cap.transform.SetParent(console.transform, false);
        cap.transform.localPosition = new Vector3(0.2f, 1.05f, 0f);
        cap.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        cap.transform.localScale = new Vector3(0.18f, 0.18f, 0.055f);
        cap.GetComponent<Renderer>().sharedMaterial = capMat;

        XRSimpleInteractable interactable = cap.AddComponent<XRSimpleInteractable>();
        XRPokeFilter poke = cap.AddComponent<XRPokeFilter>();
        poke.pokeInteractable = interactable;
        poke.pokeCollider = cap.GetComponent<Collider>();
        poke.pokeConfiguration.Value.pokeDirection = PokeAxis.NegativeZ;
        poke.pokeConfiguration.Value.interactionDepthOffset = 0.02f;
        poke.pokeConfiguration.Value.enablePokeAngleThreshold = true;
        poke.pokeConfiguration.Value.pokeAngleThreshold = 70f;

        ResetButton button = cap.AddComponent<ResetButton>();
        button.cap = cap.transform;
        button.match = Object.FindFirstObjectByType<BowlingMatch>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        GameObject canvasObject = new GameObject("ResetLabel");
        canvasObject.transform.SetParent(console.transform, false);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        RectTransform rect = canvasObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(240f, 70f);
        rect.localPosition = new Vector3(0.2f, 1.28f, 0f);
        rect.localRotation = Quaternion.Euler(0f, -90f, 0f);
        rect.localScale = Vector3.one * 0.0016f;
        GameObject textObject = new GameObject("Label");
        textObject.transform.SetParent(canvasObject.transform, false);
        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = 42;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = "RESET";
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    static void ShortenCollider(string name, float maxZ)
    {
        GameObject target = GameObject.Find(name);
        if (target == null)
            return;
        BoxCollider box = target.GetComponent<BoxCollider>();
        if (box == null)
            return;
        Vector3 worldCenter = target.transform.TransformPoint(box.center);
        Vector3 scale = target.transform.lossyScale;
        Vector3 worldSize = new Vector3(box.size.x * scale.x, box.size.y * scale.y, box.size.z * scale.z);
        float currentMax = worldCenter.z + worldSize.z * 0.5f;
        float cut = currentMax - maxZ;
        if (cut <= 0.001f)
            return;
        worldSize.z -= cut;
        worldCenter.z -= cut * 0.5f;
        box.size = new Vector3(
            scale.x == 0f ? worldSize.x : worldSize.x / scale.x,
            scale.y == 0f ? worldSize.y : worldSize.y / scale.y,
            scale.z == 0f ? worldSize.z : worldSize.z / scale.z);
        box.center = target.transform.InverseTransformPoint(worldCenter);
    }

    static GameObject AddBox(Transform parent, string name, Vector3 worldPosition, Vector3 worldSize, Material material)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, true);
        box.transform.position = worldPosition;
        box.transform.rotation = Quaternion.identity;
        box.transform.localScale = worldSize;
        if (material != null)
            box.GetComponent<Renderer>().sharedMaterial = material;
        return box;
    }

    static GameObject AddBox(Transform parent, string name, Vector3 worldPosition, Vector3 worldSize)
    {
        Material floor = MakeMaterial("RoomFloorFill", new Color(0.22f, 0.16f, 0.13f), 0.35f);
        return AddBox(parent, name, worldPosition, worldSize, floor);
    }

    static Material MakeMaterial(string name, Color color, float smoothness)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        Material material = new Material(shader);
        material.name = name;
        material.SetColor("_BaseColor", color);
        material.SetFloat("_Smoothness", smoothness);
        return material;
    }

    static void Replace(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
            Object.DestroyImmediate(existing);
    }

    static void EditorSceneManagerMarkDirty()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
