using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class DesignedArtIntegration
{
    private const string CatalogPath = "Generated/ArtRuntimeCatalog";
    private const string IntegrationRoot = "DesignedArt";
    private const string ProceduralRoot = "ProceduralVisual";

    private static ArtRuntimeCatalog cachedCatalog;
    private static readonly Dictionary<int, Material> cutoutMaterials = new Dictionary<int, Material>();

    public static ArtRuntimeCatalog Catalog
    {
        get
        {
            if (cachedCatalog == null)
                cachedCatalog = Resources.Load<ArtRuntimeCatalog>(CatalogPath);

            return cachedCatalog;
        }
    }

    public static bool HasDesignedArt()
    {
        ArtRuntimeCatalog catalog = Catalog;
        return catalog != null &&
               (catalog.colaBox != null ||
                catalog.coinIcon != null ||
                catalog.playerIdle != null ||
                catalog.drinkShelf != null ||
                catalog.checkoutCounter != null);
    }

    public static string GetBindingSummary()
    {
        ArtRuntimeCatalog catalog = Catalog;
        if (catalog == null)
            return "DesignedArt catalog missing";

        return
            "DesignedArt " +
            "player=" + NameOf(catalog.playerIdle != null ? catalog.playerIdle : catalog.player) + ", " +
            "cola=" + NameOf(catalog.colaBox) + ", " +
            "cart=" + NameOf(catalog.shoppingCart) + ", " +
            "shelf=" + NameOf(catalog.drinkShelf) + ", " +
            "checkout=" + NameOf(catalog.checkoutCounter) + ", " +
            "mission=" + NameOf(catalog.missionPanel) + ", " +
            "coin=" + NameOf(catalog.coinIcon);
    }

    public static void ApplyEnvironment(Transform environmentRoot)
    {
        if (environmentRoot == null || Catalog == null) return;

        if (Catalog.floor != null)
        {
            ApplyTextureMaterial(
                FindDeep(environmentRoot, "MainFloor"),
                Catalog.floor,
                new Color(0.88f, 0.88f, 0.88f),
                new Vector2(5f, 4f)
            );
        }

        Sprite wallTexture = Catalog.wall != null ? Catalog.wall : Catalog.warehouseWall;
        if (wallTexture == null) return;

        ApplyTextureMaterial(
            FindDeep(environmentRoot, "BackWall"),
            wallTexture,
            new Color(0.86f, 0.86f, 0.86f),
            new Vector2(4f, 1f)
        );
        ApplyTextureMaterial(
            FindDeep(environmentRoot, "LeftWall"),
            wallTexture,
            new Color(0.86f, 0.86f, 0.86f),
            new Vector2(3f, 1f)
        );
        ApplyTextureMaterial(
            FindDeep(environmentRoot, "RightWall"),
            wallTexture,
            new Color(0.86f, 0.86f, 0.86f),
            new Vector2(3f, 1f)
        );
    }

    public static void ApplyProduct(GameObject target, string productId)
    {
        if (target == null || Catalog == null) return;

        Sprite sprite = Catalog.GetProduct(productId);
        if (sprite == null) return;

        ApplyWorldCutout(
            target,
            sprite,
            1.05f,
            new Vector3(0f, 0.47f, 0f),
            true,
            true
        );
    }

    public static void ApplyCart(GameObject target)
    {
        if (target == null || Catalog == null || Catalog.shoppingCart == null) return;

        ApplyWorldCutout(
            target,
            Catalog.shoppingCart,
            2.05f,
            new Vector3(0f, 0.95f, 0f),
            true,
            true
        );
    }

    public static void ApplyShelf(GameObject target)
    {
        if (target == null || Catalog == null || Catalog.drinkShelf == null) return;

        ApplyWorldCutout(
            target,
            Catalog.drinkShelf,
            3.45f,
            new Vector3(0f, 1.58f, 0f),
            true,
            true
        );
    }

    public static void ApplyCheckout(GameObject target)
    {
        if (target == null || Catalog == null || Catalog.checkoutCounter == null) return;

        ApplyWorldCutout(
            target,
            Catalog.checkoutCounter,
            2.55f,
            new Vector3(0f, 1.2f, 0f),
            true,
            true
        );
    }

    public static Texture2D GetTexture(Sprite sprite)
    {
        return sprite != null ? sprite.texture : null;
    }

    private static void ApplyWorldCutout(
        GameObject target,
        Sprite sprite,
        float worldHeight,
        Vector3 localPosition,
        bool faceCamera,
        bool hideProceduralVisual)
    {
        if (target == null || sprite == null) return;

        Transform root = PrepareRoot(target);

        if (hideProceduralVisual)
        {
            Transform procedural = target.transform.Find(ProceduralRoot);
            if (procedural != null)
                procedural.gameObject.SetActive(false);
        }

        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "Cutout_" + sprite.name;
        quad.transform.SetParent(root, false);
        quad.transform.localPosition = localPosition;
        quad.transform.localRotation = Quaternion.identity;

        float aspect = sprite.rect.height > 0f
            ? sprite.rect.width / sprite.rect.height
            : 1f;

        float worldWidth = worldHeight * Mathf.Max(0.2f, aspect);
        Vector3 parentScale = target.transform.lossyScale;
        quad.transform.localScale = new Vector3(
            SafeDivide(worldWidth, parentScale.x),
            SafeDivide(worldHeight, parentScale.y),
            1f
        );

        Collider collider = quad.GetComponent<Collider>();
        if (collider != null)
            Object.Destroy(collider);

        Renderer renderer = quad.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = GetCutoutMaterial(sprite);
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sortingOrder = 20;
        }

        if (faceCamera)
        {
            WorldSpriteBillboard billboard = quad.AddComponent<WorldSpriteBillboard>();
            billboard.keepVertical = true;
        }
    }

    private static Transform PrepareRoot(GameObject target)
    {
        Transform existing = target.transform.Find(IntegrationRoot);
        if (existing != null)
            Object.Destroy(existing.gameObject);

        GameObject rootObject = new GameObject(IntegrationRoot);
        rootObject.transform.SetParent(target.transform, false);
        rootObject.transform.localPosition = Vector3.zero;
        rootObject.transform.localRotation = Quaternion.identity;
        rootObject.transform.localScale = Vector3.one;
        return rootObject.transform;
    }

    private static Material GetCutoutMaterial(Sprite sprite)
    {
        int key = sprite.texture.GetInstanceID();
        Material cached;
        if (cutoutMaterials.TryGetValue(key, out cached) && cached != null)
            return cached;

        Shader shader = Shader.Find("Supermarket/CheckerboardCutout");
        if (shader == null)
            shader = Shader.Find("Unlit/Transparent");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader);
        material.name = "Cutout_" + sprite.name;
        material.mainTexture = sprite.texture;
        material.color = Color.white;

        if (material.HasProperty("_Cutoff"))
            material.SetFloat("_Cutoff", 0.08f);
        if (material.HasProperty("_NeutralTolerance"))
            material.SetFloat("_NeutralTolerance", 0.065f);
        if (material.HasProperty("_CheckerTolerance"))
            material.SetFloat("_CheckerTolerance", 0.035f);

        cutoutMaterials[key] = material;
        return material;
    }

    private static void ApplyTextureMaterial(
        Transform target,
        Sprite sprite,
        Color tint,
        Vector2 tiling)
    {
        if (target == null || sprite == null) return;

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null) return;

        sprite.texture.wrapMode = TextureWrapMode.Repeat;
        sprite.texture.filterMode = FilterMode.Bilinear;

        Shader shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        Material material = new Material(shader);
        material.name = "DesignedSurface_" + sprite.name;
        material.mainTexture = sprite.texture;
        material.mainTextureScale = tiling;
        material.color = tint;

        if (material.HasProperty("_Glossiness"))
            material.SetFloat("_Glossiness", 0.08f);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", 0.08f);

        renderer.sharedMaterial = material;
    }

    private static Transform FindDeep(Transform root, string name)
    {
        if (root == null) return null;
        if (root.name == name) return root;

        foreach (Transform child in root)
        {
            Transform found = FindDeep(child, name);
            if (found != null) return found;
        }

        return null;
    }

    private static float SafeDivide(float value, float divisor)
    {
        float magnitude = Mathf.Abs(divisor);
        return magnitude < 0.001f ? value : value / magnitude;
    }

    private static string NameOf(Sprite sprite)
    {
        return sprite != null ? sprite.name : "none";
    }
}
