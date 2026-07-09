using System;
using UnityEngine;

/// <summary>
/// Screen-space presentation layer for Day01. It deliberately ignores world-space
/// sprite scale/orientation and composes the existing art like a 2D management game.
/// </summary>
public class Day01ScreenPresentation : MonoBehaviour
{
    public Day01TapFlowController tapFlow;
    public CartSystem cart;
    public ShelfSystem shelf;

    private ArtRuntimeCatalog art;
    private Texture2D solid;
    private GUIStyle zoneStyle;
    private GUIStyle smallStyle;
    private GUIStyle missingStyle;
    private ProductBox[] boxes = Array.Empty<ProductBox>();
    private CustomerAI[] customers = Array.Empty<CustomerAI>();
    private float refreshTimer;

    public Rect PlayRect
    {
        get
        {
            float top = Mathf.Clamp(Screen.height * 0.17f, 118f, 166f);
            return new Rect(0f, top, Screen.width, Mathf.Max(1f, Screen.height - top));
        }
    }

    public Rect CartRect
    {
        get
        {
            Rect play = PlayRect;
            Rect a = N(play, 0.31f, 0.53f, 0.19f, 0.35f);
            Rect b = N(play, 0.48f, 0.52f, 0.19f, 0.35f);
            float t = tapFlow != null ? tapFlow.CartVisualProgress : 0f;
            return LerpRect(a, b, t);
        }
    }

    public Rect ShelfRect => N(PlayRect, 0.57f, 0.19f, 0.36f, 0.62f);

    void Awake()
    {
        art = DesignedArtIntegration.Catalog;
        solid = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        solid.SetPixel(0, 0, Color.white);
        solid.Apply();
        BuildStyles();
        RefreshObjects();
    }

    void OnDestroy()
    {
        if (solid != null) Destroy(solid);
    }

    void Update()
    {
        refreshTimer -= Time.deltaTime;
        if (refreshTimer <= 0f)
        {
            refreshTimer = 0.25f;
            RefreshObjects();
        }
    }

    void OnGUI()
    {
        GUI.depth = 20;
        if (art == null) art = DesignedArtIntegration.Catalog;
        if (zoneStyle == null) BuildStyles();

        Rect play = PlayRect;
        Fill(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0.07f, 0.09f, 0.10f));
        Fill(play, new Color(0.76f, 0.76f, 0.72f));

        Rect backroom = N(play, 0f, 0f, 0.49f, 1f);
        Rect sales = N(play, 0.49f, 0f, 0.51f, 1f);
        Fill(backroom, new Color(0.33f, 0.34f, 0.33f));
        Fill(sales, new Color(0.86f, 0.84f, 0.79f));
        Fill(new Rect(play.x + play.width * 0.486f, play.y, Mathf.Max(4f, play.width * 0.008f), play.height), new Color(0.08f, 0.10f, 0.11f));

        DrawZoneLabel(N(play, 0.018f, 0.025f, 0.17f, 0.065f), "BACKROOM", new Color(0.14f, 0.34f, 0.20f));
        DrawZoneLabel(N(play, 0.515f, 0.025f, 0.18f, 0.065f), "SALES FLOOR", new Color(0.20f, 0.39f, 0.57f));

        // Large, deliberate composition. No tiny billboard sprites.
        DrawSprite(art != null ? art.warehouseCorner : null, N(play, 0.025f, 0.11f, 0.42f, 0.47f), ScaleMode.ScaleToFit);
        DrawSprite(art != null ? art.palletBoxStack : null, N(play, 0.015f, 0.57f, 0.25f, 0.37f), ScaleMode.ScaleToFit);
        DrawSprite(art != null ? art.fridgeDoubleDrinks : null, N(play, 0.70f, 0.08f, 0.28f, 0.44f), ScaleMode.ScaleToFit);
        DrawSprite(art != null ? art.promoStandSuperSale : null, N(play, 0.86f, 0.50f, 0.12f, 0.27f), ScaleMode.ScaleToFit);
        DrawSprite(art != null ? art.checkoutCounter : null, N(play, 0.73f, 0.58f, 0.24f, 0.29f), ScaleMode.ScaleToFit);

        DrawBoxes(play);
        DrawCart();
        DrawShelf();
        DrawCustomers(play);
    }

    public bool TryGetBoxIndexAt(Vector2 screenPosition, out int index)
    {
        index = -1;
        Vector2 guiPoint = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
        Rect play = PlayRect;
        for (int i = 0; i < 6; i++)
        {
            if (GetBoxRect(play, i).Contains(guiPoint))
            {
                index = i;
                return true;
            }
        }
        return false;
    }

    public bool HitCart(Vector2 screenPosition)
    {
        Vector2 p = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
        return CartRect.Contains(p);
    }

    public bool HitShelf(Vector2 screenPosition)
    {
        Vector2 p = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
        return ShelfRect.Contains(p);
    }

    public ProductBox GetBoxByIndex(int index)
    {
        if (boxes == null || index < 0 || index >= boxes.Length) return null;
        return boxes[index];
    }

    void DrawBoxes(Rect play)
    {
        Sprite sprite = art != null ? (art.colaBox != null ? art.colaBox : art.drinkBox) : null;
        for (int i = 0; i < 6; i++)
        {
            ProductBox box = GetBoxByIndex(i);
            if (box == null || !box.gameObject.activeInHierarchy || box.picked) continue;
            if (tapFlow != null && tapFlow.IsBoxInTransit(box)) continue;

            Rect r = GetBoxRect(play, i);
            if (tapFlow != null && tapFlow.SelectedBox == box)
                r = Grow(r, 1.10f);
            DrawSprite(sprite, r, ScaleMode.ScaleToFit);
        }
    }

    void DrawCart()
    {
        DrawSprite(art != null ? art.shoppingCart : null, CartRect, ScaleMode.ScaleToFit);
        if (cart != null && cart.GetCount() > 0)
        {
            Rect r = CartRect;
            GUI.Label(new Rect(r.xMax - 64f, r.y + 8f, 58f, 28f), cart.GetCount() + "/6", smallStyle);
        }
    }

    void DrawShelf()
    {
        Rect r = ShelfRect;
        DrawSprite(art != null ? art.drinkShelf : null, r, ScaleMode.ScaleToFit);

        int count = shelf != null ? Mathf.Clamp(shelf.currentCount, 0, 6) : 0;
        Sprite product = art != null ? (art.colaBox != null ? art.colaBox : art.drinkBox) : null;
        for (int i = 0; i < count; i++)
        {
            int col = i % 3;
            int row = i / 3;
            Rect p = new Rect(
                r.x + r.width * (0.21f + col * 0.20f),
                r.y + r.height * (0.30f + row * 0.24f),
                r.width * 0.16f,
                r.height * 0.20f
            );
            DrawSprite(product, p, ScaleMode.ScaleToFit);
        }

        for (int i = count; i < 6; i++)
        {
            int col = i % 3;
            int row = i / 3;
            Rect m = new Rect(
                r.x + r.width * (0.20f + col * 0.20f),
                r.y + r.height * (0.29f + row * 0.24f),
                r.width * 0.17f,
                r.height * 0.21f
            );
            GUI.Box(m, "MISSING", missingStyle);
        }
    }

    void DrawCustomers(Rect play)
    {
        if (art == null || customers == null) return;
        for (int i = 0; i < customers.Length; i++)
        {
            CustomerAI c = customers[i];
            if (c == null) continue;
            Sprite s = art.GetCustomer(Mathf.Abs(c.GetInstanceID()));
            float nx = Mathf.InverseLerp(-10f, 10f, c.transform.position.x);
            float ny = Mathf.InverseLerp(7f, -7f, c.transform.position.z);
            Rect r = new Rect(
                play.x + nx * play.width - play.width * 0.035f,
                play.y + ny * play.height - play.height * 0.11f,
                play.width * 0.07f,
                play.height * 0.22f
            );
            DrawSprite(s, r, ScaleMode.ScaleToFit);
        }
    }

    void RefreshObjects()
    {
        boxes = FindObjectsOfType<ProductBox>();
        Array.Sort(boxes, (a, b) => string.CompareOrdinal(a != null ? a.name : string.Empty, b != null ? b.name : string.Empty));
        customers = FindObjectsOfType<CustomerAI>();
    }

    Rect GetBoxRect(Rect play, int index)
    {
        int col = index % 3;
        int row = index / 3;
        return N(play, 0.055f + col * 0.115f, 0.61f + row * 0.18f, 0.12f, 0.18f);
    }

    void DrawZoneLabel(Rect rect, string text, Color color)
    {
        Fill(rect, color);
        GUI.Label(rect, text, zoneStyle);
    }

    void DrawSprite(Sprite sprite, Rect rect, ScaleMode mode)
    {
        if (sprite == null || sprite.texture == null) return;
        Rect tr = sprite.textureRect;
        Texture2D tex = sprite.texture;
        Rect uv = new Rect(tr.x / tex.width, tr.y / tex.height, tr.width / tex.width, tr.height / tex.height);
        GUI.DrawTextureWithTexCoords(rect, tex, uv, true);
    }

    void Fill(Rect rect, Color color)
    {
        if (solid == null) return;
        Color old = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, solid, ScaleMode.StretchToFill);
        GUI.color = old;
    }

    void BuildStyles()
    {
        zoneStyle = new GUIStyle(GUI.skin.label);
        zoneStyle.alignment = TextAnchor.MiddleCenter;
        zoneStyle.fontSize = Mathf.RoundToInt(Mathf.Clamp(Screen.height * 0.027f, 16f, 26f));
        zoneStyle.fontStyle = FontStyle.Bold;
        zoneStyle.normal.textColor = Color.white;

        smallStyle = new GUIStyle(GUI.skin.label);
        smallStyle.alignment = TextAnchor.MiddleCenter;
        smallStyle.fontSize = 17;
        smallStyle.fontStyle = FontStyle.Bold;
        smallStyle.normal.textColor = Color.white;

        missingStyle = new GUIStyle(GUI.skin.box);
        missingStyle.alignment = TextAnchor.LowerCenter;
        missingStyle.fontSize = 10;
        missingStyle.fontStyle = FontStyle.Bold;
        missingStyle.normal.textColor = new Color(0.95f, 0.25f, 0.20f);
    }

    static Rect N(Rect parent, float x, float y, float w, float h)
    {
        return new Rect(parent.x + parent.width * x, parent.y + parent.height * y, parent.width * w, parent.height * h);
    }

    static Rect LerpRect(Rect a, Rect b, float t)
    {
        t = Mathf.Clamp01(t);
        return new Rect(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.width, b.width, t), Mathf.Lerp(a.height, b.height, t));
    }

    static Rect Grow(Rect r, float factor)
    {
        Vector2 c = r.center;
        r.width *= factor;
        r.height *= factor;
        r.center = c;
        return r;
    }
}