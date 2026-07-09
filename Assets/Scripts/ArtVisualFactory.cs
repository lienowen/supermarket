using UnityEngine;

public static class ArtVisualFactory
{
    private const string CatalogResourcePath = "Generated/ArtRuntimeCatalog";
    private static ArtRuntimeCatalog cachedCatalog;

    public static ArtRuntimeCatalog Catalog
    {
        get
        {
            if (cachedCatalog == null)
                cachedCatalog = Resources.Load<ArtRuntimeCatalog>(CatalogResourcePath);

            return cachedCatalog;
        }
    }

    public static bool HasCatalog()
    {
        return Catalog != null;
    }

    public static void ApplyPlayer(GameObject target)
    {
        ApplyBillboard(target, Catalog != null ? Catalog.player : null, 2.2f, new Vector3(0f, 1f, 0f), true);
    }

    public static void ApplyCustomer(GameObject target, int index)
    {
        Sprite sprite = Catalog != null ? Catalog.GetCustomer(index) : null;
        ApplyBillboard(target, sprite, 2.1f, new Vector3(0f, 1f, 0f), true);
    }

    public static void ApplyDrinkBox(GameObject target)
    {
        ApplyBillboard(target, Catalog != null ? Catalog.drinkBox : null, 0.9f, new Vector3(0f, 0.45f, 0f), false);
    }

    public static void ApplyCart(GameObject target)
    {
        ApplyBillboard(target, Catalog != null ? Catalog.shoppingCart : null, 1.7f, new Vector3(0f, 0.8f, 0f), true);
    }

    public static void ApplyShelf(GameObject target)
    {
        ApplyBillboard(target, Catalog != null ? Catalog.drinkShelf : null, 2.8f, new Vector3(0f, 1.35f, 0f), true);
    }

    public static void ApplyCheckout(GameObject target)
    {
        ApplyBillboard(target, Catalog != null ? Catalog.checkoutCounter : null, 2.4f, new Vector3(0f, 1.05f, 0f), true);
    }

    public static void ApplyWall(GameObject target)
    {
        ApplyBillboard(target, Catalog != null ? Catalog.warehouseWall : null, 4f, Vector3.zero, false);
    }

    public static void ApplyFloor(GameObject target)
    {
        Sprite sprite = Catalog != null ? Catalog.floor : null;
        if (sprite == null) return;

        DisablePrimitiveRenderer(target);

        GameObject visual = new GameObject("ArtVisual");
        visual.transform.SetParent(target.transform, false);
        visual.transform.localPosition = Vector3.up * 0.26f;
        visual.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = -20;

        FitSprite(renderer, Mathf.Max(target.transform.localScale.x, target.transform.localScale.z));
    }

    private static void ApplyBillboard(GameObject target, Sprite sprite, float height, Vector3 localPosition, bool keepVertical)
    {
        if (target == null || sprite == null) return;

        DisablePrimitiveRenderer(target);

        Transform existing = target.transform.Find("ArtVisual");
        GameObject visual = existing != null ? existing.gameObject : new GameObject("ArtVisual");
        visual.transform.SetParent(target.transform, false);
        visual.transform.localPosition = localPosition;
        visual.transform.localRotation = Quaternion.identity;

        SpriteRenderer renderer = visual.GetComponent<SpriteRenderer>();
        if (renderer == null)
            renderer = visual.AddComponent<SpriteRenderer>();

        renderer.sprite = sprite;
        renderer.sortingOrder = 5;

        FitSprite(renderer, height);

        WorldSpriteBillboard billboard = visual.GetComponent<WorldSpriteBillboard>();
        if (billboard == null)
            billboard = visual.AddComponent<WorldSpriteBillboard>();

        billboard.keepVertical = keepVertical;
    }

    private static void FitSprite(SpriteRenderer renderer, float targetHeight)
    {
        if (renderer == null || renderer.sprite == null) return;

        float sourceHeight = renderer.sprite.bounds.size.y;
        if (sourceHeight <= 0.001f) return;

        float scale = targetHeight / sourceHeight;
        renderer.transform.localScale = Vector3.one * scale;
    }

    private static void DisablePrimitiveRenderer(GameObject target)
    {
        MeshRenderer mesh = target.GetComponent<MeshRenderer>();
        if (mesh != null)
            mesh.enabled = false;
    }
}
