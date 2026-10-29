using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FirstPersonSceneBuilder
{
    [MenuItem("Tools/Build First Person Course")]
    public static void BuildMenu()
    {
        Debug.Log(Build());
    }

    public static string Build()
    {
        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Textures");

        Texture2D groundTex = LoadGroundTexture("Assets/Textures/Ground.png");
        Material groundMat = LoadOrCreate("Assets/Materials/Ground.mat", Color.white, false, groundTex, new Vector2(4f, 4f), 0.05f);
        Material wallMat = LoadOrCreate("Assets/Materials/Wall.mat", new Color(0.45f, 0.38f, 0.32f), false, null, Vector2.one, 0.2f);
        Material obstacleMat = LoadOrCreate("Assets/Materials/Obstacle.mat", new Color(0.55f, 0.22f, 0.16f), false, null, Vector2.one, 0.25f);
        Material movingMat = LoadOrCreate("Assets/Materials/MovingObstacle.mat", new Color(0.75f, 0.35f, 0.1f), false, null, Vector2.one, 0.3f);
        Material playerMat = LoadOrCreate("Assets/Materials/Player.mat", new Color(0.2f, 0.45f, 0.85f), false, null, Vector2.one, 0.4f);
        Material crystalMat = LoadOrCreate("Assets/Materials/Crystal.mat", new Color(0.2f, 0.95f, 1f), true, null, Vector2.one, 0.8f);
        Material beaconMat = LoadOrCreate("Assets/Materials/Beacon.mat", new Color(1f, 0.78f, 0.15f), true, null, Vector2.one, 0.6f);
        Material treeMat = LoadOrCreate("Assets/Materials/Tree.mat", new Color(0.18f, 0.42f, 0.2f), false, null, Vector2.one, 0.15f);

        DestroyIfExists("Course");
        DestroyIfExists("Player");
        DestroyIfExists("GameManager");

        GameObject course = new GameObject("Course");

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.SetParent(course.transform, false);
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(8f, 1f, 8f);
        ground.GetComponent<Renderer>().sharedMaterial = groundMat;

        CreateBox(course.transform, "WallNorth", new Vector3(0f, 2.5f, 40f), new Vector3(82f, 5f, 1f), wallMat);
        CreateBox(course.transform, "WallSouth", new Vector3(0f, 2.5f, -40f), new Vector3(82f, 5f, 1f), wallMat);
        CreateBox(course.transform, "WallEast", new Vector3(40f, 2.5f, 0f), new Vector3(1f, 5f, 82f), wallMat);
        CreateBox(course.transform, "WallWest", new Vector3(-40f, 2.5f, 0f), new Vector3(1f, 5f, 82f), wallMat);

        CreateBox(course.transform, "CrateA", new Vector3(-14f, 1f, -12f), new Vector3(4f, 2f, 4f), obstacleMat);
        CreateBox(course.transform, "CrateB", new Vector3(10f, 1.5f, -6f), new Vector3(3f, 3f, 8f), obstacleMat);
        CreateBox(course.transform, "Barrier", new Vector3(-8f, 1f, 10f), new Vector3(8f, 2f, 2f), obstacleMat);
        CreateBox(course.transform, "Tower", new Vector3(16f, 2f, 18f), new Vector3(4f, 4f, 4f), obstacleMat);
        CreateBox(course.transform, "Step", new Vector3(-22f, 0.5f, 4f), new Vector3(4f, 1f, 4f), obstacleMat);

        GameObject mover = CreateBox(course.transform, "SlidingGate", new Vector3(0f, 1f, 0f), new Vector3(4f, 2f, 2f), movingMat);
        Rigidbody moverBody = mover.AddComponent<Rigidbody>();
        moverBody.isKinematic = true;
        moverBody.useGravity = false;
        moverBody.interpolation = RigidbodyInterpolation.Interpolate;
        moverBody.constraints = RigidbodyConstraints.FreezeRotation;
        MovingObstacle motion = mover.AddComponent<MovingObstacle>();
        motion.delta = new Vector3(14f, 0f, 0f);
        motion.period = 3.5f;

        CreateTree(course.transform, new Vector3(-30f, 0f, -28f), treeMat);
        CreateTree(course.transform, new Vector3(28f, 0f, -24f), treeMat);
        CreateTree(course.transform, new Vector3(-32f, 0f, 20f), treeMat);
        CreateTree(course.transform, new Vector3(30f, 0f, 26f), treeMat);

        CreateCrystal(course.transform, "Crystal1", new Vector3(-24f, 1.3f, -22f), crystalMat);
        CreateCrystal(course.transform, "Crystal2", new Vector3(22f, 1.3f, -18f), crystalMat);
        CreateCrystal(course.transform, "Crystal3", new Vector3(-18f, 1.6f, 4.8f), crystalMat);
        CreateCrystal(course.transform, "Crystal4", new Vector3(22f, 1.3f, 8f), crystalMat);
        CreateCrystal(course.transform, "Crystal5", new Vector3(0f, 1.3f, 24f), crystalMat);

        GameObject pad = CreateBox(course.transform, "BeaconPad", new Vector3(0f, 0.25f, 34f), new Vector3(6f, 0.5f, 6f), beaconMat);
        GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillar.name = "BeaconPillar";
        pillar.transform.SetParent(course.transform, false);
        pillar.transform.position = new Vector3(0f, 2.5f, 34f);
        pillar.transform.localScale = new Vector3(0.5f, 2.5f, 0.5f);
        pillar.GetComponent<Renderer>().sharedMaterial = beaconMat;

        GameObject zone = new GameObject("BeaconZone");
        zone.transform.SetParent(course.transform, false);
        zone.transform.position = new Vector3(0f, 2f, 34f);
        BoxCollider zoneCollider = zone.AddComponent<BoxCollider>();
        zoneCollider.isTrigger = true;
        zoneCollider.size = new Vector3(5f, 4f, 5f);
        zone.AddComponent<FinishBeacon>();

        Light glow = new GameObject("BeaconLight").AddComponent<Light>();
        glow.transform.SetParent(pillar.transform, false);
        glow.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        glow.type = LightType.Point;
        glow.color = new Color(1f, 0.85f, 0.3f);
        glow.intensity = 6f;
        glow.range = 10f;

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1.05f, -32f);
        Rigidbody body = player.AddComponent<Rigidbody>();
        body.mass = 1f;
        body.linearDamping = 5f;
        body.angularDamping = 0.05f;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        body.freezeRotation = true;

        GameObject model = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        model.name = "Body";
        model.transform.SetParent(player.transform, false);
        model.GetComponent<Renderer>().sharedMaterial = playerMat;

        GameObject orientation = new GameObject("Orientation");
        orientation.transform.SetParent(player.transform, false);

        GameObject cameraPosition = new GameObject("CameraPosition");
        cameraPosition.transform.SetParent(player.transform, false);
        cameraPosition.transform.localPosition = new Vector3(0f, 0.7f, 0f);

        PlayerController controller = player.AddComponent<PlayerController>();
        controller.speed = 50f;
        controller.jumpForce = 7f;
        controller.orientation = orientation.transform;

        Camera camera = Camera.main;
        if (camera == null)
            return "Main Camera is missing";

        CameraController existing = camera.GetComponent<CameraController>();
        if (existing != null)
            Object.DestroyImmediate(existing);

        CameraController cameraController = camera.gameObject.AddComponent<CameraController>();
        cameraController.cameraPosition = cameraPosition.transform;
        cameraController.orientation = orientation.transform;
        cameraController.sensitivyX = 400f;
        cameraController.sensitivyY = 400f;
        camera.nearClipPlane = 0.2f;
        camera.transform.position = cameraPosition.transform.position;
        camera.transform.rotation = Quaternion.identity;

        GameObject managerObject = new GameObject("GameManager");
        CourseManager manager = managerObject.AddComponent<CourseManager>();
        manager.totalCrystals = 5;

        AssetDatabase.SaveAssets();
        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        if (scene.path.EndsWith("SampleScene.unity"))
        {
            string error = AssetDatabase.RenameAsset(scene.path, "FirstPersonCourse");
            if (!string.IsNullOrEmpty(error))
                return "Scene saved, rename failed: " + error;
        }

        AssetDatabase.SaveAssets();
        return "Built First Person course. Ground, player, walls, obstacles, crystals and beacon are in the scene.";
    }

    static void EnsureFolder(string parent, string name)
    {
        string path = parent + "/" + name;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, name);
    }

    static void DestroyIfExists(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
            Object.DestroyImmediate(existing);
    }

    static GameObject CreateBox(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, false);
        box.transform.position = position;
        box.transform.localScale = scale;
        box.GetComponent<Renderer>().sharedMaterial = material;
        return box;
    }

    static void CreateTree(Transform parent, Vector3 position, Material material)
    {
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Tree";
        trunk.transform.SetParent(parent, false);
        trunk.transform.position = position + new Vector3(0f, 1.2f, 0f);
        trunk.transform.localScale = new Vector3(0.4f, 1.2f, 0.4f);
        trunk.GetComponent<Renderer>().sharedMaterial = material;

        GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        crown.name = "TreeCrown";
        crown.transform.SetParent(trunk.transform, false);
        crown.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        crown.transform.localScale = new Vector3(3.2f, 1.6f, 3.2f);
        crown.GetComponent<Renderer>().sharedMaterial = material;
    }

    static void CreateCrystal(Transform parent, string name, Vector3 position, Material material)
    {
        GameObject crystal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        crystal.name = name;
        crystal.transform.SetParent(parent, false);
        crystal.transform.position = position;
        crystal.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        crystal.GetComponent<Renderer>().sharedMaterial = material;
        crystal.GetComponent<SphereCollider>().isTrigger = true;
        crystal.AddComponent<CrystalPickup>();
    }

    static Texture2D LoadGroundTexture(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.mipmapEnabled = true;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static Material LoadOrCreate(string path, Color color, bool emission, Texture texture, Vector2 scale, float smoothness)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        material.shader = shader;
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);

        if (texture != null)
        {
            if (material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", texture);
                material.SetTextureScale("_BaseMap", scale);
            }
            material.mainTexture = texture;
            material.mainTextureScale = scale;
        }

        if (emission)
        {
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            if (material.HasProperty("_EmissionColor"))
                material.SetColor("_EmissionColor", color * 1.4f);
        }

        EditorUtility.SetDirty(material);
        return material;
    }
}
