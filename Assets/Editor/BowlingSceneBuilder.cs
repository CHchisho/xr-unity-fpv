using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using Unity.XR.CoreUtils;

public static class BowlingSceneBuilder
{
    [MenuItem("Tools/Build Bowling Alley")]
    public static void BuildMenu()
    {
        Debug.Log(Build());
    }

    public static string Build()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject lightObject = new GameObject("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 0.4f;
        light.color = new Color(1f, 0.96f, 0.9f);
        light.shadows = LightShadows.Soft;
        lightObject.transform.rotation = Quaternion.Euler(36f, -70f, 0f);
        PlaceEnvironment();

        GameObject alley = Object.Instantiate(LoadModel("Assets/Models/Bowling/Alley.glb"));
        alley.name = "Alley";
        AddStaticColliders(alley);

        GameObject pinPrefab = LoadModel("Assets/Models/Bowling/Pin.glb");
        GameObject ballPrefab = LoadModel("Assets/Models/Bowling/Ball.glb");

        Renderer laneRenderer = FindNamed(alley.transform, "Lane").GetComponent<Renderer>();
        Bounds laneBounds = laneRenderer.bounds;
        float floorY = laneBounds.max.y;
        float backZ = laneBounds.min.z + 0.7f;

        BowlingPin[] pins = new BowlingPin[10];
        int index = 0;
        float spacing = 0.32f;
        for (int row = 0; row < 4; row++)
        {
            int count = row + 1;
            for (int column = 0; column < count; column++)
            {
                float x = (column - (count - 1) * 0.5f) * spacing;
                float z = backZ + row * spacing;
                GameObject pin = Object.Instantiate(pinPrefab, new Vector3(x, floorY, z), Quaternion.identity);
                pin.name = "Pin " + (index + 1);
                AddBody(pin, 1.4f, false);
                pins[index] = pin.AddComponent<BowlingPin>();
                index++;
            }
        }

        Transform stand = FindNamed(alley.transform, "BallStand");
        Vector3 ballPosition = stand != null
            ? stand.position + Vector3.up * 0.45f
            : new Vector3(-0.9f, floorY + 0.2f, -0.4f);
        GameObject ball = Object.Instantiate(ballPrefab, ballPosition, Quaternion.identity);
        ball.name = "Ball";
        Rigidbody ballBody = AddBody(ball, 5.5f, true);
        XRGrabInteractable grab = ball.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grab.throwOnDetach = true;
        grab.throwVelocityScale = 1.6f;
        grab.throwAngularVelocityScale = 1f;

        SphereCollider ballSphere = ball.GetComponent<SphereCollider>();
        if (ballSphere == null)
        {
            ballSphere = ball.AddComponent<SphereCollider>();
            ballSphere.radius = 0.108f;
            ballSphere.center = Vector3.zero;
        }
        MeshCollider[] ballMeshes = ball.GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < ballMeshes.Length; i++)
            Object.DestroyImmediate(ballMeshes[i]);

        Transform approach = FindNamed(alley.transform, "Approach");
        if (approach != null)
            approach.gameObject.AddComponent<TeleportationArea>();
        Transform deckLeft = FindNamed(alley.transform, "DeckLeft");
        if (deckLeft != null)
            deckLeft.gameObject.AddComponent<TeleportationArea>();
        Transform deckRight = FindNamed(alley.transform, "DeckRight");
        if (deckRight != null)
            deckRight.gameObject.AddComponent<TeleportationArea>();

        new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
        GameObject originPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Samples/XR Interaction Toolkit/3.7.0/Starter Assets/Prefabs/XR Origin (XR Rig).prefab");
        GameObject origin = (GameObject)PrefabUtility.InstantiatePrefab(originPrefab);
        origin.transform.position = new Vector3(0f, 0f, 2.15f);
        origin.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        ContinuousTurnProvider[] extraTurn = origin.GetComponentsInChildren<ContinuousTurnProvider>(true);
        for (int i = 0; i < extraTurn.Length; i++)
            extraTurn[i].enabled = false;
        XROrigin xrOrigin = origin.GetComponent<XROrigin>();
        if (xrOrigin != null)
            xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;

        GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
        button.name = "ResetButton";
        button.transform.position = new Vector3(0.72f, 1.05f, 1.55f);
        button.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        button.transform.localScale = new Vector3(0.18f, 0.18f, 0.08f);
        button.GetComponent<Renderer>().sharedMaterial = MakeMaterial("ResetButton", new Color(0.8f, 0.15f, 0.12f), 0.4f);
        XRSimpleInteractable reset = button.AddComponent<XRSimpleInteractable>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        GameObject canvasObject = new GameObject("Scoreboard");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObject.AddComponent<CanvasScaler>();
        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(900f, 280f);
        canvasRect.position = new Vector3(0f, 1.65f, 0.35f);
        canvasRect.rotation = Quaternion.Euler(0f, 180f, 0f);
        canvasRect.localScale = Vector3.one * 0.0022f;

        GameObject textObject = new GameObject("ScoreText");
        textObject.transform.SetParent(canvasObject.transform, false);
        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = 42;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        GameObject matchObject = new GameObject("BowlingMatch");
        BowlingMatch match = matchObject.AddComponent<BowlingMatch>();
        match.pins = pins;
        match.ballBody = ballBody;
        match.ballGrab = grab;
        match.resetButton = reset;
        match.scoreText = text;

        string path = "Assets/Scenes/BowlingAlley.unity";
        EditorSceneManager.SaveScene(scene, path);
        AddSceneToBuild(path);
        return "Saved " + path + " lane " + laneBounds + " ball " + ballPosition;
    }

    static void PlaceEnvironment()
    {
        Material sky = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SkyCC0.mat");
        if (sky != null)
        {
            RenderSettings.skybox = sky;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 0.65f;
        }

        GameObject model = LoadModel("Assets/Models/Bowling/Room.glb");
        if (model == null)
            return;
        GameObject room = Object.Instantiate(model);
        room.name = "BowlingRoom";
        Renderer[] renderers = room.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer.name.StartsWith("RoomLight"))
            {
                Light lamp = renderer.gameObject.AddComponent<Light>();
                lamp.type = LightType.Point;
                lamp.color = new Color(1f, 0.9f, 0.75f);
                lamp.intensity = 2.2f;
                lamp.range = 6.5f;
                lamp.shadows = LightShadows.Soft;
                continue;
            }

            GameObject target = renderer.gameObject;
            Bounds world = renderer.bounds;
            BoxCollider box = target.AddComponent<BoxCollider>();
            box.center = target.transform.InverseTransformPoint(world.center);
            Vector3 scale = target.transform.lossyScale;
            box.size = new Vector3(
                scale.x == 0f ? world.size.x : world.size.x / scale.x,
                scale.y == 0f ? world.size.y : world.size.y / scale.y,
                scale.z == 0f ? world.size.z : world.size.z / scale.z);
        }
    }

    static GameObject LoadModel(string path)
    {
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (model == null)
            throw new System.InvalidOperationException("Missing model " + path);
        return model;
    }

    static Rigidbody AddBody(GameObject root, float mass, bool sphereAlreadyHandled)
    {
        Rigidbody body = root.AddComponent<Rigidbody>();
        body.mass = mass;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            MeshFilter filter = renderers[i].GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null)
                continue;
            if (sphereAlreadyHandled && renderers[i].gameObject == root)
                continue;
            MeshCollider collider = renderers[i].gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = filter.sharedMesh;
            collider.convex = true;
        }
        return body;
    }

    static void AddStaticColliders(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            GameObject target = renderers[i].gameObject;
            Bounds world = renderers[i].bounds;
            BoxCollider box = target.AddComponent<BoxCollider>();
            box.center = target.transform.InverseTransformPoint(world.center);
            Vector3 scale = target.transform.lossyScale;
            box.size = new Vector3(
                scale.x == 0f ? world.size.x : world.size.x / scale.x,
                scale.y == 0f ? world.size.y : world.size.y / scale.y,
                scale.z == 0f ? world.size.z : world.size.z / scale.z);
        }
    }

    static Bounds RendererBounds(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }

    static Transform FindNamed(Transform root, string name)
    {
        if (root.name == name)
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindNamed(root.GetChild(i), name);
            if (found != null)
                return found;
        }
        return null;
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

    static void AddSceneToBuild(string path)
    {
        EditorBuildSettingsScene[] current = EditorBuildSettings.scenes;
        var next = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        next.Add(new EditorBuildSettingsScene(path, true));
        for (int i = 0; i < current.Length; i++)
        {
            if (current[i].path != path)
                next.Add(current[i]);
        }
        EditorBuildSettings.scenes = next.ToArray();
    }
}
