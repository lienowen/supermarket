#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ArtCatalogBuilder
{
    private const string ArtRoot = "Assets/Art";
    private const string ResourceFolder = "Assets/Resources";
    private const string GeneratedFolder = "Assets/Resources/Generated";
    private const string CatalogPath = "Assets/Resources/Generated/ArtRuntimeCatalog.asset";
    private const string SessionKey = "supermarket.art.catalog.checked";

    static ArtCatalogBuilder()
    {
        EditorApplication.delayCall += EnsureCatalog;
    }

    [MenuItem("Supermarket/Rebuild Art Catalog")]
    public static void RebuildCatalog()
    {
        BuildCatalog(true);
    }

    private static void EnsureCatalog()
    {
        if (SessionState.GetBool(SessionKey, false)) return;
        SessionState.SetBool(SessionKey, true);

        if (!AssetDatabase.IsValidFolder(ArtRoot))
        {
            Debug.LogWarning("ArtCatalogBuilder: Assets/Art was not found. Primitive fallback visuals will be used.");
            return;
        }

        BuildCatalog(false);
    }

    private static void BuildCatalog(bool forceLog)
    {
        EnsureFolder("Assets", "Resources");
        EnsureFolder(ResourceFolder, "Generated");

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { ArtRoot });
        List<string> texturePaths = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) continue;

            texturePaths.Add(path);
            EnsureSpriteImport(path);
        }

        AssetDatabase.Refresh();

        ArtRuntimeCatalog catalog = AssetDatabase.LoadAssetAtPath<ArtRuntimeCatalog>(CatalogPath);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<ArtRuntimeCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogPath);
        }

        catalog.player = FindBestSprite(texturePaths,
            new[] { "/characters/player/", "player" },
            new[] { "icon", "ui", "shadow" });

        catalog.customers = FindCustomerSprites(texturePaths);

        catalog.drinkBox = FindBestSprite(texturePaths,
            new[] { "cola_crate", "cola_box", "drink_box", "/products/" },
            new[] { "icon", "ui" });

        catalog.shoppingCart = FindBestSprite(texturePaths,
            new[] { "shopping_cart", "cart" },
            new[] { "icon", "ui" });

        catalog.drinkShelf = FindBestSprite(texturePaths,
            new[] { "shelf_drinks", "drink_shelf", "shelf" },
            new[] { "icon", "ui" });

        catalog.checkoutCounter = FindBestSprite(texturePaths,
            new[] { "checkout_counter", "checkout", "cashier" },
            new[] { "icon", "ui", "/characters/" });

        catalog.warehouseWall = FindBestSprite(texturePaths,
            new[] { "warehouse", "wall", "/environment/interior/" },
            new[] { "icon", "ui", "floor" });

        catalog.floor = FindBestSprite(texturePaths,
            new[] { "floor", "ground", "tile" },
            new[] { "icon", "ui" });

        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (forceLog || catalog.player != null || catalog.drinkBox != null || catalog.shoppingCart != null)
        {
            Debug.Log(
                "ArtCatalogBuilder: catalog ready. " +
                "Player=" + NameOf(catalog.player) + ", " +
                "Customers=" + (catalog.customers != null ? catalog.customers.Length : 0) + ", " +
                "Box=" + NameOf(catalog.drinkBox) + ", " +
                "Cart=" + NameOf(catalog.shoppingCart) + ", " +
                "Shelf=" + NameOf(catalog.drinkShelf) + ", " +
                "Checkout=" + NameOf(catalog.checkoutCounter)
            );
        }
    }

    private static void EnsureSpriteImport(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;

        bool changed = false;

        if (importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            changed = true;
        }

        if (importer.spriteImportMode != SpriteImportMode.Single)
        {
            importer.spriteImportMode = SpriteImportMode.Single;
            changed = true;
        }

        if (!importer.alphaIsTransparency)
        {
            importer.alphaIsTransparency = true;
            changed = true;
        }

        if (importer.mipmapEnabled)
        {
            importer.mipmapEnabled = false;
            changed = true;
        }

        if (changed)
            importer.SaveAndReimport();
    }

    private static Sprite[] FindCustomerSprites(List<string> paths)
    {
        List<Sprite> sprites = new List<Sprite>();

        foreach (string path in paths)
        {
            string normalized = Normalize(path);
            if (!normalized.Contains("/characters/customers/") && !normalized.Contains("customer"))
                continue;

            Sprite sprite = LoadSprite(path);
            if (sprite != null && !sprites.Contains(sprite))
                sprites.Add(sprite);
        }

        return sprites.ToArray();
    }

    private static Sprite FindBestSprite(List<string> paths, string[] preferred, string[] excluded)
    {
        string bestPath = null;
        int bestScore = int.MinValue;

        foreach (string path in paths)
        {
            string normalized = Normalize(path);
            int score = 0;

            foreach (string keyword in preferred)
            {
                string token = keyword.ToLowerInvariant();
                if (normalized.Contains(token))
                    score += token.StartsWith("/") ? 20 : 10;
            }

            foreach (string keyword in excluded)
            {
                if (normalized.Contains(keyword.ToLowerInvariant()))
                    score -= 30;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestPath = path;
            }
        }

        if (bestScore <= 0 || string.IsNullOrEmpty(bestPath))
            return null;

        return LoadSprite(bestPath);
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite != null) return sprite;

        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (UnityEngine.Object asset in assets)
        {
            sprite = asset as Sprite;
            if (sprite != null) return sprite;
        }

        return null;
    }

    private static string Normalize(string path)
    {
        return path.Replace('\\', '/').ToLowerInvariant();
    }

    private static string NameOf(Sprite sprite)
    {
        return sprite != null ? sprite.name : "none";
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = parent + "/" + child;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, child);
    }
}
#endif
