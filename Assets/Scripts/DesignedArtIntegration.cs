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
            "coin=" + NameOf(catalog.coinIcon) + ", " +
            "floorTexture=" + NameOf(catalog.floor) + ", " +
            "wallTexture=" + NameOf(catalog.wall);
    }

    public static void ApplyEnvironment(Transform environmentRoot)
    {
        if (environmentRoot == null || Catalog == null) return;

        // Only dedicated seamless texture slots are accepted here.
        // Concept renders, sale posters, wet-floor signs and object illustrations are never
        // allowed to tile across the supermarket floor or walls.
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
        if (wallTexture != null)
        {
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
    }

    public static void ApplyProduct(GameObject target, string productId)
    {
        if (target == null || Catalog == null) return;

        Sprite sprite = Catalog.GetProduct(productId);
        if (sprite == null) return;

        Transform root = PrepareRoot(target);

        // Product art is allowed only as a small front label on an existing 3D crate.
        // It is not used as the crate geometry itself and is not repeated over the scene.
        CreateDecal(
            root,
            "ProductFrontArt",
            sprite,
            new Vector3(0f, 0.34f, 0.377f),
            Quaternion.identity,
            0.5f,
            12
        );
    }

    public static void ApplyCart(GameObject target)
    {
        // shopping_cart.png is a rendered object illustration, not a texture map.
        // Keep the 3D cart mesh clean. The sprite remains available for UI/catalog use.
    }

    public static void ApplyShelf(GameObject target)
    {
        // shelf_drinks.png is a design reference render, not a world-space texture.
        // The procedural 3D shelf already follows its color/layout direction.
    }

    public static void ApplyCheckout(GameObject target)
    {
        // checkout_counter.png is a design reference render, not a texture map.
        // Keep the 3D checkout geometry and use this art in future shop/catalog UI.
    }

    public static Texture2D GetTexture(Sprite sprite)
    {
        return sprite != null ? sprite.texture : null;
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

    private static string NameOf(Sprite sprite)
    {
        return sprite != null ? sprite.name : "none";
    }
}
