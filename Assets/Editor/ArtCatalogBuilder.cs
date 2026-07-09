#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ArtCatalogBuilder
{
    private const string ArtRoot = "Assets/Art";
    private const string ResourceFolder = "Assets/Resources";
    private const string CatalogPath = "Assets/Resources/Generated/ArtRuntimeCatalog.asset";
    private const string SessionKey = "supermarket.art.catalog.v4.checked";

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

        // Characters.
        catalog.playerIdle = LoadVerifiedSprite(
            "Assets/Art/Characters/Player/player_idle.png",
            "Assets/Art/Characters/Player/player_front.png"
        );
        catalog.playerCarry = LoadVerifiedSprite(
            "Assets/Art/Characters/Player/player_carry.png"
        );
        catalog.player = catalog.playerIdle != null
            ? catalog.playerIdle
            : FindBestSprite(
                texturePaths,
                new[] { "/characters/player/", "player" },
                new[] { "icon", "ui", "shadow" }
            );
        catalog.customers = FindCustomerSprites(texturePaths);

        // Product cutouts.
        catalog.colaBox = LoadVerifiedSprite("Assets/Art/Products/cola_box.png");
        catalog.waterBox = LoadVerifiedSprite("Assets/Art/Products/water_box.png");
        catalog.milkBox = LoadVerifiedSprite("Assets/Art/Products/milk_box.png");
        catalog.chipsBox = LoadVerifiedSprite("Assets/Art/Products/chips_box.png");
        catalog.drinkBox = catalog.colaBox;

        // Main gameplay cutouts. Keep several exact historical locations because the packs
        // were uploaded in multiple batches and some files live in older folders.
        catalog.shoppingCart = LoadVerifiedSprite(
            "Assets/Art/Props/shopping_cart.png",
            "Assets/Art/Environment/shopping_cart.png"
        );
        catalog.drinkShelf = LoadVerifiedSprite(
            "Assets/Art/Environment/Interior/shelf_drinks.png"
        );
        catalog.checkoutCounter = LoadVerifiedSprite(
            "Assets/Art/Environment/Interior/checkout_counter.png"
        );
        catalog.fridgeDoubleDrinks = LoadVerifiedSprite(
            "Assets/Art/Environment/Interior/fridge_double_drinks.png"
        );

        // Existing Day01 decoration pack.
        catalog.warehouseCorner = LoadVerifiedSprite(
            "Assets/Art/Environment/Storage/warehouse_corner.png",
            "Assets/Art/Maps/Locations/warehouse_corner.png"
        );
        catalog.palletBoxStack = LoadVerifiedSprite(
            "Assets/Art/Environment/Storage/pallet_box_stack.png",
            "Assets/Art/Props/pallet_box_stack.png"
        );
        catalog.promoStandSuperSale = LoadVerifiedSprite(
            "Assets/Art/Props/Promo/promo_stand_super_sale.png",
            "Assets/Art/Decorations/promo_stand_super_sale.png"
        );
        catalog.pottedPlantLarge = LoadVerifiedSprite(
            "Assets/Art/Decorations/potted_plant_large.png",
            "Assets/Art/Props/potted_plant_large.png"
        );

        // World surfaces only accept dedicated tile textures. No fuzzy matching here.
        catalog.floor = LoadVerifiedSprite(
            "Assets/Art/Environment/Textures/floor_tile.png",
            "Assets/Art/Environment/Interior/floor_tile.png"
        );
        catalog.wall = LoadVerifiedSprite(
            "Assets/Art/Environment/Textures/wall_tile.png",
            "Assets/Art/Environment/Interior/wall_tile.png"
        );
        catalog.warehouseWall = catalog.wall;

        // UI pack.
        catalog.missionPanel = LoadVerifiedSprite(
            "Assets/Art/UI/mission_panel.png",
            "Assets/Art/Economy/UI/Panels/mission_panel.png"
        );
        catalog.coinIcon = LoadVerifiedSprite(
            "Assets/Art/UI/coin.png",
            "Assets/Art/Currency/coin.png"
        );
        catalog.coinStack = LoadVerifiedSprite(
            "Assets/Art/Currency/coin_stack.png",
            "Assets/Art/UI/coin_stack.png",
            "Assets/Art/Economy/UI/Panels/coin_stack.png"
        );
        catalog.starIcon = LoadVerifiedSprite("Assets/Art/UI/star.png");
        catalog.timerIcon = LoadVerifiedSprite("Assets/Art/UI/timer.png");
        catalog.buttonPlay = LoadVerifiedSprite("Assets/Art/UI/button_play.png");
        catalog.buttonUpgrade = LoadVerifiedSprite("Assets/Art/UI/button_upgrade.png");
        catalog.buttonNext = LoadVerifiedSprite("Assets/Art/UI/button_next.png");

        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        ReportPlaceholder("Assets/Art/Props/shopping_cart.png");
        ReportPlaceholder("Assets/Art/UI/mission_panel.png");

        if (forceLog || catalog.player != null || catalog.colaBox != null)
        {
            Debug.Log(
                "ArtCatalogBuilder: Day01 cutout pack ready. " +
                "Player=" + NameOf(catalog.player) + ", " +
                "Customers=" + (catalog.customers != null ? catalog.customers.Length : 0) + ", " +
                "Cola=" + NameOf(catalog.colaBox) + ", " +
                "Cart=" + NameOf(catalog.shoppingCart) + ", " +
                "Shelf=" + NameOf(catalog.drinkShelf) + ", " +
                "Checkout=" + NameOf(catalog.checkoutCounter) + ", " +
                "Fridge=" + NameOf(catalog.fridgeDoubleDrinks) + ", " +
                "Warehouse=" + NameOf(catalog.warehouseCorner) + ", " +
                "Pallet=" + NameOf(catalog.palletBoxStack) + ", " +
                "Promo=" + NameOf(catalog.promoStandSuperSale) + ", " +
                "Plant=" + NameOf(catalog.pottedPlantLarge) + ", " +
                "MissionPanel=" + NameOf(catalog.missionPanel) + ", " +
                "Coin=" + NameOf(catalog.coinIcon) + ", " +
                "CoinStack=" + NameOf(catalog.coinStack)
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
            if (IsValidSprite(sprite) && !sprites.Contains(sprite))
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

        Sprite sprite = LoadSprite(bestPath);
        return IsValidSprite(sprite) ? sprite : null;
    }

    private static Sprite LoadVerifiedSprite(params string[] paths)
    {
        foreach (string path in paths)
        {
            Sprite sprite = LoadSprite(path);
            if (IsValidSprite(sprite))
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

    private static bool IsValidSprite(Sprite sprite)
    {
        return sprite != null &&
               sprite.texture != null &&
               sprite.texture.width >= 8 &&
               sprite.texture.height >= 8 &&
               sprite.rect.width >= 8f &&
               sprite.rect.height >= 8f;
    }

    private static void ReportPlaceholder(string path)
    {
        if (!File.Exists(path)) return;

        FileInfo info = new FileInfo(path);
        if (info.Length == 0)
            Debug.LogWarning("ArtCatalogBuilder: zero-byte placeholder asset ignored: " + path);
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
