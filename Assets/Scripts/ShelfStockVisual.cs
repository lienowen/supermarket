using UnityEngine;

/// <summary>
/// Visual overlay for shelf inventory. Six small product sprites appear as the
/// player restocks and disappear again when customers buy them.
/// </summary>
public class ShelfStockVisual : MonoBehaviour
{
    public ShelfSystem shelf;
    public Sprite productSprite;
    public int slotCount = 6;

    private GameObject[] slots;
    private int lastCount = -1;

    public void Configure(ShelfSystem targetShelf, Sprite sprite, int count = 6)
    {
        shelf = targetShelf;
        productSprite = sprite;
        slotCount = Mathf.Max(1, count);
        BuildSlots();
        Refresh(true);
    }

    void Start()
    {
        if (slots == null || slots.Length == 0)
            BuildSlots();
    }

    void Update()
    {
        Refresh(false);
    }

    void BuildSlots()
    {
        if (productSprite == null)
        {
            ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
            if (catalog != null)
                productSprite = catalog.colaBox != null ? catalog.colaBox : catalog.drinkBox;
        }

        if (productSprite == null)
            return;

        Transform old = transform.Find("LiveStockSlots");
        if (old != null)
            Destroy(old.gameObject);

        GameObject root = new GameObject("LiveStockSlots");
        root.transform.SetParent(transform, false);
        root.transform.localPosition = new Vector3(0f, 0f, -0.08f);
        root.transform.localRotation = Quaternion.identity;

        slots = new GameObject[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = new GameObject("StockSlot_" + (i + 1));
            slot.transform.SetParent(root.transform, false);

            int column = i % 3;
            int row = i / 3;
            slot.transform.localPosition = new Vector3(
                (column - 1) * 0.54f,
                0.86f + row * 0.66f,
                0f
            );

            SpriteRenderer renderer = slot.AddComponent<SpriteRenderer>();
            renderer.sprite = productSprite;
            renderer.sortingOrder = 60 + i;
            renderer.color = Color.white;

            float aspect = productSprite.rect.height > 0f
                ? productSprite.rect.width / productSprite.rect.height
                : 1f;
            float height = 0.5f;
            float nativeHeight = productSprite.rect.height / Mathf.Max(1f, productSprite.pixelsPerUnit);
            float scale = height / Mathf.Max(0.01f, nativeHeight);
            slot.transform.localScale = new Vector3(scale, scale, 1f);

            slots[i] = slot;
        }
    }

    void Refresh(bool force)
    {
        if (shelf == null || slots == null)
            return;

        int count = Mathf.Clamp(shelf.currentCount, 0, slots.Length);
        if (!force && count == lastCount)
            return;

        lastCount = count;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
                slots[i].SetActive(i < count);
        }
    }
}