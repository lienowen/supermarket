using UnityEngine;

/// <summary>
/// Builds Day01 as a fixed presentation stage using the designed art already in the project.
/// The stage itself is static; only boxes, cart, shelf stock and customers move.
/// </summary>
public static class Day01StaticStageBuilder
{
    public static Day01EnvironmentLayout Build()
    {
        GameObject root = new GameObject("Day01StaticStage");

        CreateFloor(root.transform);
        CreateZonePanels(root.transform);
        CreateDesignedDecorations(root.transform);

        return new Day01EnvironmentLayout
        {
            playerSpawn = new Vector3(-7.4f, 0f, 0.4f),
            boxOrigin = new Vector3(-8.2f, 0.05f, -3.8f),
            cartPosition = new Vector3(-4.1f, 0.05f, -1.2f),
            shelfPosition = new Vector3(4.1f, 0.05f, -1.5f),
            checkoutPosition = new Vector3(6.3f, 0.05f, 4.35f),
            customerSpawn = new Vector3(-8.9f, 0f, 6.7f),
            customerShelfPoint = new Vector3(2.85f, 0f, -1.45f),
            customerCheckoutPoint = new Vector3(5.0f, 0f, 4.35f),
            customerExitPoint = new Vector3(-8.9f, 0f, 6.7f)
        };
    }

    static void CreateFloor(Transform parent)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "StaticStageFloor";
        floor.transform.SetParent(parent, false);
        floor.transform.position = new Vector3(0f, -0.18f, 0.7f);
        floor.transform.localScale = new Vector3(23.5f, 0.28f, 17.2f);

        Renderer renderer = floor.GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Sprites/Default");

            Material material = new Material(shader);
            material.color = new Color(0.70f, 0.72f, 0.72f);
            renderer.sharedMaterial = material;
        }
    }

    static void CreateZonePanels(Transform parent)
    {
        CreatePanel(
            "WarehouseZone",
            parent,
            new Vector3(-6.4f, 0.01f, -1.4f),
            new Vector3(8.5f, 0.04f, 10.2f),
            new Color(0.31f, 0.36f, 0.40f)
        );

        CreatePanel(
            "SalesZone",
            parent,
            new Vector3(4.3f, 0.015f, 0.8f),
            new Vector3(11.8f, 0.05f, 14.4f),
            new Color(0.84f, 0.82f, 0.77f)
        );

        CreatePanel(
            "CheckoutZone",
            parent,
            new Vector3(6.4f, 0.025f, 4.4f),
            new Vector3(7.2f, 0.06f, 3.8f),
            new Color(0.92f, 0.70f, 0.26f)
        );
    }

    static void CreatePanel(string name, Transform parent, Vector3 position, Vector3 scale, Color color)
    {
        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = name;
        panel.transform.SetParent(parent, false);
        panel.transform.position = position;
        panel.transform.localScale = scale;

        Renderer renderer = panel.GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Sprites/Default");

            Material material = new Material(shader);
            material.color = color;
            renderer.sharedMaterial = material;
        }

        Collider collider = panel.GetComponent<Collider>();
        if (collider != null)
            Object.Destroy(collider);
    }

    static void CreateDesignedDecorations(Transform parent)
    {
        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null) return;

        Add(parent, DesignedArtIntegration.CreateWarehouseCornerDecoration(new Vector3(-7.5f, 0f, -6.1f)));
        Add(parent, DesignedArtIntegration.CreatePalletStackDecoration(new Vector3(-9.4f, 0f, -2.2f)));
        Add(parent, DesignedArtIntegration.CreateFridgeDecoration(new Vector3(9.35f, 0f, -3.2f)));
        Add(parent, DesignedArtIntegration.CreatePromoStandDecoration(new Vector3(7.1f, 0f, 0.75f)));
        Add(parent, DesignedArtIntegration.CreatePlantDecoration(new Vector3(-9.8f, 0f, 6.0f)));
    }

    static void Add(Transform parent, GameObject child)
    {
        if (child != null)
            child.transform.SetParent(parent, true);
    }
}