using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class DesignedArtIntegration
{
    private const string CatalogPath = "Generated/ArtRuntimeCatalog";
    private const string IntegrationRoot = "DesignedArt";

    private static ArtRuntimeCatalog cachedCatalog;
    private static readonly Dictionary<int, Material> decalMaterials = new Dictionary<int, Material>();

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
                catalog.missionPanel != null ||
                catalog.playerIdle != null);
    }

    public static void ApplyEnvironment(Transform environmentRoot)
    {
        if (environmentRoot == null || Catalog == null) return;

        ApplyTextureMaterial(
            FindDeep(environmentRoot, "MainFloor"),
            Catalog.floor,
            new Color(0.95f, 0.95f, 0.95f),
            new Vector2(5f, 4f)
        );

        Sprite wallSprite = Catalog.wall != null ? Catalog.wall : Catalog.warehouseWall;
        ApplyTextureMaterial(
            FindDeep(environmentRoot, "BackWall"),
            wallSprite,
            new Color(0.92f, 0.92f, 0.92f),
            new Vector2(4f, 1f)
        );
        ApplyTextureMaterial(
            FindDeep(environmentRoot, "LeftWall"),
            wallSprite,
            new Color(0.92f, 0.92f, 0.92f),
            new Vector2(3f, 1f)
        );
        ApplyTextureMaterial(
            FindDeep(environmentRoot, "RightWall"),
            wallSprite,
            new Color(0.92f, 0.92f, 0.92f),
            new Vector2(3f, 1f)
        );
    }

    public static void ApplyProduct(GameObject target, string productId)
    {
        if (target == null || Catalog == null) return;

        Sprite sprite = Catalog.GetProduct(productId);
        if (sprite == null) return;

        Transform root = PrepareRoot(target);

        CreateDecal(
            root,
            "ProductFrontArt",
            sprite,
            new Vector3(0f, 0.34f, 0.377f),
            Quaternion.identity,
            0.62f,
            12
        );

        CreateDecal(
            root,
            "ProductSideArt",
            sprite,
            new Vector3(0.437f, 0.34f, 0f),
            Quaternion.Euler(0f, 90f, 0f),
            0.42f,
            11
        );
    }

    public static void ApplyCart(GameObject target)
    {
        if (target == null || Catalog == null || Catalog.shoppingCart == null) return;

        Transform root = PrepareRoot(target);
        CreateDecal(
            root,
            "CartDesignBadge",
            Catalog.shoppingCart,
            new Vector3(0f, 1.16f, 0.485f),
            Quaternion.identity,
            0.5f,
            10
        );
    }

    public static void ApplyShelf(GameObject target)
    {
        if (target == null || Catalog == null || Catalog.drinkShelf == null) return;

        Transform root = PrepareRoot(target);
        CreateDecal(
            root,
            "ShelfDesignedHeader",
            Catalog.drinkShelf,
            new Vector3(0f, 3.03f, 0.105f),
            Quaternion.identity,
            1.45f,
            10
        );
    }

    public static void ApplyCheckout(GameObject target)
    {
        if (target == null || Catalog == null || Catalog.checkoutCounter == null) return;

        Transform root = PrepareRoot(target);
        CreateDecal(
            root,
            "CheckoutDesignedPanel",
            Catalog.checkoutCounter,
            new Vector3(0f, 0.56f, 0.475f),
            Quaternion.identity,
            0.72f,
            10
        );
    }

    public static Texture2D GetTexture(Sprite sprite)
    {
        return sprite != null ? sprite.texture : null;
    }

    private static Transform PrepareRoot(GameObject target)
    {
        Transform existing = target.transform.Find(IntegrationRoot);
        if (existing != null)
        {
            Object.Destroy(existing.gameObject);
        }

        GameObject rootObject = new GameObject(IntegrationRoot);
        rootObject.transform.SetParent(target.transform, false);
        rootObject.transform.localPosition = Vector3.zero;
        rootObject.transform.localRotation = Quaternion.identity;
        rootObject.transform.localScale = Vector3.one;
        return rootObject.transform;
    }

    private static void CreateDecal(
        Transform parent,
        string name,
        Sprite sprite,
        Vector3 localPosition,
        Quaternion localRotation,
        float width,
        int sortingOrder)
    {
        if (parent == null || sprite == null) return;

        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.SetParent(parent, false);
        quad.transform.localPosition = localPosition;
        quad.transform.localRotation = localRotation;

        float aspect = sprite.rect.height > 0f
            ? sprite.rect.width / sprite.rect.height
            : 1f;
        float height = width / Mathf.Max(0.05f, aspect);
        quad.transform.localScale = new Vector3(width, height, 1f);

        Collider collider = quad.GetComponent<Collider>();
        if (collider != null)
            Object.Destroy(collider);

        Renderer renderer = quad.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = GetDecalMaterial(sprite);
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sortingOrder = sortingOrder;
        }
    }

    private static Material GetDecalMaterial(Sprite sprite)
    {
        int key = sprite.texture.GetInstanceID();
        Material cached;
        if (decalMaterials.TryGetValue(key, out cached) && cached != null)
            return cached;

        Shader shader = Shader.Find("Unlit/Transparent");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Standard");

        Material material = new Material(shader);
        material.name = "DesignedArt_" + sprite.name;
        material.mainTexture = sprite.texture;
        material.color = Color.white;

        if (material.HasProperty("_Cutoff"))
            material.SetFloat("_Cutoff", 0.05f);

        decalMaterials[key] = material;
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
}
