using UnityEngine;
using UnityEngine.Rendering;

public class Day01EnvironmentLayout
{
    public Vector3 playerSpawn;
    public Vector3 boxOrigin;
    public Vector3 cartPosition;
    public Vector3 shelfPosition;
    public Vector3 checkoutPosition;
    public Vector3 customerSpawn;
    public Vector3 customerShelfPoint;
    public Vector3 customerCheckoutPoint;
    public Vector3 customerExitPoint;
}

public static class Day01EnvironmentBuilder
{
    private static Material floorLight;
    private static Material floorWarehouse;
    private static Material wallWarm;
    private static Material wallAccent;
    private static Material blue;
    private static Material green;
    private static Material yellow;
    private static Material dark;
    private static Material glass;
    private static Material lightPanel;

    public static Day01EnvironmentLayout Build()
    {
        EnsureMaterials();
        ConfigureLighting();

        GameObject root = new GameObject("Environment");

        Box("MainFloor", root.transform, new Vector3(0f, -0.18f, 1f), new Vector3(24f, 0.32f, 18f), floorLight, true);
        Box("WarehouseFloor", root.transform, new Vector3(-6.2f, 0.01f, -1.7f), new Vector3(7.6f, 0.05f, 8.4f), floorWarehouse, false);
        Box("CheckoutFloor", root.transform, new Vector3(5.6f, 0.015f, 4.1f), new Vector3(7.6f, 0.06f, 4.6f), yellow, false);

        Box("BackWall", root.transform, new Vector3(0f, 2.2f, -7.85f), new Vector3(24f, 4.4f, 0.3f), wallWarm, true);
        Box("LeftWall", root.transform, new Vector3(-11.85f, 2.2f, 1f), new Vector3(0.3f, 4.4f, 18f), wallWarm, true);
        Box("RightWall", root.transform, new Vector3(11.85f, 2.2f, 1f), new Vector3(0.3f, 4.4f, 18f), wallWarm, true);

        Box("WarehouseDivider", root.transform, new Vector3(-2.15f, 1.8f, -3.6f), new Vector3(0.25f, 3.6f, 8.3f), wallAccent, true);
        Box("WarehouseHeader", root.transform, new Vector3(-6.6f, 3.35f, 2.9f), new Vector3(7.9f, 0.5f, 0.35f), dark, false);

        BuildEntrance(root.transform);
        BuildFridgeBank(root.transform);
        BuildDecorAisle(root.transform);
        BuildProduceIsland(root.transform);
        BuildCeilingLights(root.transform);

        DesignedArtIntegration.ApplyEnvironment(root.transform);

        return new Day01EnvironmentLayout
        {
            playerSpawn = new Vector3(-5.8f, 0f, 0.7f),
            boxOrigin = new Vector3(-8.2f, 0.05f, -4.6f),
            cartPosition = new Vector3(-3.5f, 0.05f, 0.6f),
            shelfPosition = new Vector3(4.5f, 0.05f, -1.6f),
            checkoutPosition = new Vector3(6.6f, 0.05f, 4.4f),
            customerSpawn = new Vector3(-8.8f, 0f, 7.2f),
            customerShelfPoint = new Vector3(3.25f, 0f, -1.6f),
            customerCheckoutPoint = new Vector3(5.2f, 0f, 4.4f),
            customerExitPoint = new Vector3(-8.8f, 0f, 7.2f)
        };
    }

    private static void BuildEntrance(Transform parent)
    {
        Box("EntranceLeft", parent, new Vector3(-10.4f, 1.5f, 8.15f), new Vector3(2.1f, 3f, 0.35f), wallAccent, true);
        Box("EntranceRight", parent, new Vector3(-5.8f, 1.5f, 8.15f), new Vector3(2.1f, 3f, 0.35f), wallAccent, true);
        Box("EntranceTop", parent, new Vector3(-8.1f, 3.25f, 8.15f), new Vector3(6.7f, 0.5f, 0.35f), blue, true);
        Box("DoorGlass", parent, new Vector3(-8.1f, 1.5f, 8.1f), new Vector3(2.4f, 3f, 0.08f), glass, false);

        Box("EntranceSign", parent, new Vector3(-8.1f, 3.28f, 8.38f), new Vector3(3.8f, 0.28f, 0.08f), dark, false);
    }

    private static void BuildFridgeBank(Transform parent)
    {
        for (int i = 0; i < 4; i++)
        {
            float z = -5.7f + i * 2.25f;
            Box("FridgeBody_" + i, parent, new Vector3(10.65f, 1.35f, z), new Vector3(1.2f, 2.7f, 2f), dark, true);
            Box("FridgeDoor_" + i, parent, new Vector3(10.02f, 1.38f, z), new Vector3(0.05f, 2.35f, 1.75f), glass, false);
            Box("FridgeLight_" + i, parent, new Vector3(9.96f, 2.47f, z), new Vector3(0.06f, 0.06f, 1.55f), blue, false);
        }

        Box("ColdDrinksSign", parent, new Vector3(10.0f, 3.2f, -2.2f), new Vector3(0.12f, 0.42f, 3.4f), blue, false);
    }

    private static void BuildDecorAisle(Transform parent)
    {
        for (int row = 0; row < 2; row++)
        {
            float z = row == 0 ? 1.15f : -4.1f;
            Box("DecorShelfBase_" + row, parent, new Vector3(1f, 0.45f, z), new Vector3(4.4f, 0.9f, 1.1f), dark, true);
            Box("DecorShelfTop_" + row, parent, new Vector3(1f, 1.7f, z), new Vector3(4.4f, 0.12f, 1.1f), wallAccent, false);

            for (int i = 0; i < 9; i++)
            {
                Material productMaterial = i % 3 == 0 ? blue : i % 3 == 1 ? green : yellow;
                Box(
                    "DecorProduct_" + row + "_" + i,
                    parent,
                    new Vector3(-0.8f + i * 0.45f, 1.12f, z),
                    new Vector3(0.28f, 0.72f, 0.42f),
                    productMaterial,
                    false
                );
            }
        }
    }

    private static void BuildProduceIsland(Transform parent)
    {
        Box("ProduceIsland", parent, new Vector3(7.3f, 0.5f, -3.6f), new Vector3(3.1f, 1f, 2f), green, true);
        Box("ProduceTop", parent, new Vector3(7.3f, 1.08f, -3.6f), new Vector3(3.3f, 0.16f, 2.2f), dark, false);

        for (int x = -2; x <= 2; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                GameObject fruit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                fruit.name = "Produce";
                fruit.transform.SetParent(parent, false);
                fruit.transform.position = new Vector3(7.3f + x * 0.48f, 1.28f, -3.6f + z * 0.52f);
                fruit.transform.localScale = Vector3.one * 0.3f;

                Renderer renderer = fruit.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = (x + z) % 2 == 0 ? yellow : green;

                Collider collider = fruit.GetComponent<Collider>();
                if (collider != null)
                    Object.Destroy(collider);
            }
        }
    }

    private static void BuildCeilingLights(Transform parent)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector3 position = new Vector3(x * 6.5f, 4.25f, 0.5f + z * 5.2f);
                Box("CeilingPanel", parent, position, new Vector3(2.7f, 0.06f, 0.5f), lightPanel, false);
            }
        }
    }

    private static void ConfigureLighting()
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.38f, 0.41f, 0.45f);
        RenderSettings.fog = false;

        Light directional = Object.FindObjectOfType<Light>();
        if (directional != null)
        {
            directional.type = LightType.Directional;
            directional.intensity = 0.55f;
            directional.color = new Color(1f, 0.95f, 0.88f);
            directional.shadows = LightShadows.Soft;
            directional.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        }

        CreatePointLight("WarmFill", new Vector3(0f, 4f, 1f), new Color(1f, 0.84f, 0.68f), 0.32f, 11f);
        CreatePointLight("CheckoutFill", new Vector3(6.5f, 3.5f, 4f), new Color(1f, 0.88f, 0.7f), 0.25f, 7f);
    }

    private static void CreatePointLight(string name, Vector3 position, Color color, float intensity, float range)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = position;

        Light light = obj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.None;
    }

    private static GameObject Box(
        string name,
        Transform parent,
        Vector3 position,
        Vector3 scale,
        Material material,
        bool addCollider)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent, true);
        obj.transform.position = position;
        obj.transform.localScale = scale;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.On;
            renderer.receiveShadows = true;
        }

        Collider collider = obj.GetComponent<Collider>();
        if (!addCollider && collider != null)
            Object.Destroy(collider);

        return obj;
    }

    private static void EnsureMaterials()
    {
        if (floorLight != null) return;

        floorLight = Mat(new Color(0.68f, 0.72f, 0.75f), 0.1f);
        floorWarehouse = Mat(new Color(0.31f, 0.36f, 0.42f), 0.04f);
        wallWarm = Mat(new Color(0.78f, 0.76f, 0.72f), 0.06f);
        wallAccent = Mat(new Color(0.57f, 0.64f, 0.69f), 0.06f);
        blue = Mat(new Color(0.07f, 0.34f, 0.65f), 0.25f);
        green = Mat(new Color(0.12f, 0.49f, 0.24f), 0.14f);
        yellow = Mat(new Color(0.82f, 0.58f, 0.12f), 0.14f);
        dark = Mat(new Color(0.11f, 0.14f, 0.18f), 0.38f);
        glass = Mat(new Color(0.32f, 0.58f, 0.7f), 0.45f);
        lightPanel = Mat(new Color(0.92f, 0.9f, 0.78f), 0.12f);
    }

    private static Material Mat(Color color, float smoothness)
    {
        Shader shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        Material material = new Material(shader);
        material.color = color;

        if (material.HasProperty("_Glossiness"))
            material.SetFloat("_Glossiness", smoothness);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);

        return material;
    }
}
