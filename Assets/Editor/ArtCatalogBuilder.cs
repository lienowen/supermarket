#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ArtCatalogBuilder
{
    private const string ArtRoot = "Assets/Art";
    private const string ResourceFolder = "Assets/Resources";
    private const string GeneratedFolder = "Assets/Resources/Generated";
    private const string CatalogPath = "Assets/Resources/Generated/ArtRuntimeCatalog.asset";
    private const string SessionKey = "supermarket.art.catalog.v2.checked";

    static ArtCatalogBuilder()
    {
        EditorApplication.delayCall += EnsureCatalog;
    }

    [MenuItem("Supermarket/Rebuild Art Catalog")]
    public static void RebuildCatalog()
    {
        BuildCatalog(true, true);
    }

    private static void EnsureCatalog()
    {
        if (SessionState.GetBool(SessionKey, false)) return;
        SessionState.SetBool(SessionKey, true);

        if (!AssetDatabase.IsValidFolder(ArtRoot))
        {
            Debug.LogWarning("ArtCatalogBuilder: Assets/Art was not found.");
            return;
        }

        BuildCatalog(false, true);
    }

    private static void BuildCatalog(bool forceLog, bool fixImportSettings)
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

            if (fixImportSettings)
                EnsureSpriteImport(path);
        }

        if (fixImportSettings)
            AssetDatabase.Refresh();

        ArtRuntimeCatalog catalog = AssetDatabase.LoadAssetAtPath<ArtRuntimeCatalog>(CatalogPath);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<ArtRuntimeCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogPath);
        }

        catalog.playerIdle = LoadFirstSprite(
            "Assets/Art/Characters/Player/player_idle.png",
            "Assets/Art/Characters/Player/player_front.png"
        );
        catalog.playerCarry = LoadFirstSprite(
            "Assets/Art/Characters/Player/player_carry.png"
        );
        catalog.player = catalog.playerIdle != null
            ? catalog.playerIdle
            : FindBestSprite(texturePaths, new[] { "/characters/player/", "player" }, new[] { "icon", "ui", "shadow" });

        catalog.customers = FindCustomerSprites(texturePaths);

        catalog.colaBox = LoadFirstSprite("Assets/Art/Products/cola_box.png");
        catalog.waterBox = LoadFirstSprite("Assets/Art/Products/water_box.png");
        catalog.milkBox = LoadFirstSprite("Assets/Art/Products/milk_box.png");
        catalog.chipsBox = LoadFirstSprite("Assets/Art/Products/chips_box.png");
        catalog.drinkBox = catalog.colaBox != null
            ? catalog.colaBox
            : FindBestSprite(texturePaths, new[] { "cola_crate", "cola_box", "drink_box", "/products/" }, new[] { "icon", "ui" });

        catalog.shoppingCart = LoadFirstSprite(
            "Assets/Art/Props/shopping_cart.png"
        );
        if (catalog.shoppingCart == null)
            catalog.shoppingCart = FindBestSprite(texturePaths, new[] { "shopping_cart", "cart" }, new[] { "icon", "ui" });

        catalog.drinkShelf = LoadFirstSprite(
            "Assets/Art/Environment/Interior/shelf_drinks.png",
            "Assets/Art/Environment/Interior/shelf.png"
        );
        if (catalog.drinkShelf == null)
            catalog.drinkShelf = FindBestSprite(texturePaths, new[] { "shelf_drinks", "drink_shelf", "shelf" }, new[] { "icon", "ui" });

        catalog.checkoutCounter = LoadFirstSprite(
            "Assets/Art/Environment/Interior/checkout_counter.png",
            "Assets/Art/Environment/Interior/checkout.png"
        );
        if (catalog.checkoutCounter == null)
            catalog.checkoutCounter = FindBestSprite(texturePaths, new[] { "checkout_counter", "checkout", "cashier" }, new[] { "icon", "ui", "/characters/" });

        catalog.floor = LoadFirstSprite(
            "Assets/Art/Environment/Interior/floor.png",
            "Assets/Art/Environment/floor.png"
        );
        if (catalog.floor == null)
            catalog.floor = FindBestSprite(texturePaths, new[] { "floor", "ground", "tile" }, new[] { "icon", "ui" });

        catalog.wall = LoadFirstSprite(
            "Assets/Art/Environment/Interior/wall.png",
            "Assets/Art/Environment/wall.png"
        );
        catalog.warehouseWall = catalog.wall != null
            ? catalog.wall
            : FindBestSprite(texturePaths, new[] { "warehouse", "wall", "/environment/interior/" }, new[] { "icon", "ui", "floor" });

        catalog.missionPanel = LoadFirstSprite("Assets/Art/UI/mission_panel.png");
        catalog.coinIcon = LoadFirstSprite(
            "Assets/Art/UI/coin.png",
            "Assets/Art/UI/coin_stack.png"
        );
        catalog.starIcon = LoadFirstSprite("Assets/Art/UI/star.png");
        catalog.timerIcon = LoadFirstSprite("Assets/Art/UI/timer.png");
        catalog.buttonPlay = LoadFirstSprite("Assets/Art/UI/button_play.png");
        catalog.buttonUpgrade = LoadFirstSprite("Assets/Art/UI/button_upgrade.png");
        catalog.buttonNext = LoadFirstSprite("Assets/Art/UI/button_next.png");

        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (forceLog || catalog.player != null || catalog.drinkBox != null || catalog.missionPanel != null)
        {
            Debug.Log(
                "ArtCatalogBuilder: designed assets bound. " +
                "Player=" + NameOf(catalog.player) + ", " +
                "Customers=" + (catalog.customers != null ? catalog.customers.Length : 0) + ", " +
                "Cola=" + NameOf(catalog.colaBox) + ", " +
                "Cart=" + NameOf(catalog.shoppingCart) + ", " +
                "Shelf=" + NameOf(catalog.drinkShelf) + ", " +
                "Checkout=" + NameOf(catalog.checkoutCounter) + ", " +
                "MissionPanel=" + NameOf(catalog.missionPanel) + ", " +
                "Coin=" + NameOf(catalog.coinIcon)
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

    private static Sprite LoadFirstSprite(params string[] paths)
    {
        foreach (string path in paths)
        {
            Sprite sprite = LoadSprite(path);
            if (sprite != null)
                return sprite;
        }

        return null;
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite != null) return sprite;

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (Object asset in assets)
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
