using System;
using UnityEngine;

/// <summary>
/// Screen-space presentation for Day01. Existing art is composed as a deliberate
/// 2D management scene, independent of world-space sprite size and camera angle.
/// </summary>
public class Day01ScreenPresentation : MonoBehaviour
{
    public Day01TapFlowController tapFlow;
    public CartSystem cart;
    public ShelfSystem shelf;

    private ArtRuntimeCatalog art;
    private Texture2D solid;
    private Material cutoutMaterial;
    private GUIStyle zoneStyle;
    private GUIStyle smallStyle;
    private GUIStyle missingStyle;
    private GUIStyle instructionStyle;
    private ProductBox[] boxes = new ProductBox[0];
    private CustomerAI[] customers = new CustomerAI[0];
    private float refreshTimer;

    public Rect PlayRect
    {
        get
        {
            float top = Mathf.Clamp(Screen.height * 0.18f, 122f, 172f);
            return new Rect(0f, top, Screen.width, Mathf.Max(1f, Screen.height - top));
        }
    }

    public Rect CartRect
    {
        get
        {
            Rect play = PlayRect;
            Rect start = N(play, 0.30f, 0.53f, 0.18f, 0.33f);
            Rect end = N(play, 0.47f, 0.53f, 0.18f, 0.33f);
            float t = tapFlow != null ? tapFlow.CartVisualProgress : 0f;
            return LerpRect(start, end, t);
        }
    }

    public Rect ShelfRect => N(PlayRect, 0.56f, 0.16f, 0.36f, 0.66f);

    void Awake()
    {
        art = DesignedArtIntegration.Catalog;

        solid = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        solid.SetPixel(0, 0, Color.white);
        solid.Apply();

        Shader cutoutShader = Shader.Find("Supermarket/CheckerboardCutout");
        if (cutoutShader != null)
        {
            cutoutMaterial = new Material(cutoutShader);
            cutoutMaterial.SetFloat("_Cutoff", 0.08f);
            cutoutMaterial.SetFloat("_NeutralTolerance", 0.065f);
            cutoutMaterial.SetFloat("_CheckerTolerance", 0.035f);
        }

        BuildStyles();
        RefreshObjects();
    }

    void OnDestroy()
    {
        if (solid != null) Destroy(solid);
        if (cutoutMaterial != null) Destroy(cutoutMaterial);
    }

    void Update()
    {
        refreshTimer -= Time.deltaTime;
        if (refreshTimer <= 0f)
        {
            refreshTimer = 0.2f;
            RefreshObjects();
        }
    }

    void OnGUI()
    {
        GUI.depth = 20;
        if (art == null) art = DesignedArtIntegration.Catalog;
        if (zoneStyle == null) BuildStyles();

        Rect play = PlayRect;

        // Dark app frame, then a single coherent split scene.
        Fill(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0.065f, 0.08f, 0.09f));
        Fill(play, new Color(0.78f, 0.77f, 0.73f));

        Rect backroom = N(play, 0f, 0f, 0.49f, 1f);
        Rect sales = N(play, 0.49f, 0f, 0.51f, 1f);
        Fill(backroom, new Color(0.31f, 0.34f, 0.35f));
        Fill(sales, new Color(0.91f, 0.89f, 0.84f));

        // Floor bands add depth without returning to free-roaming 3D.
        Fill(N(play, 0f, 0.72f, 0.49f, 0.28f), new Color(0.43f, 0.45f, 0.45f));
        Fill(N(play, 0.49f, 0.72f, 0.51f, 0.28f), new Color(0.78f, 0.75f, 0.69f));
        Fill(new Rect(play.x + play.width * 0.486f, play.y, Mathf.Max(5f, play.width * 0.008f), play.height), new Color(0.08f, 0.10f, 0.11f));

        DrawZoneLabel(N(play, 0.018f, 0.025f, 0.18f, 0.065f), "BACKROOM", new Color(0.12f, 0.33f, 0.20f));
        DrawZoneLabel(N(play, 0.515f, 0.025f, 0.19f, 0.065f), "SALES FLOOR", new Color(0.17f, 0.38f, 0.58f));

        // Existing art, deliberately sized. Nothing should float as a tiny billboard.
        DrawSprite(art != null ? art.warehouseCorner : null, N(play, 0.025f, 0.11f, 0.42f, 0.48f));
        DrawSprite(art != null ? art.palletBoxStack : null, N(play, 0.005f, 0.55f, 0.25f, 0.38f));
        DrawSprite(art != null ? art.fridgeDoubleDrinks : null, N(play, 0.69f, 0.08f, 0.29f, 0.45f));
        DrawSprite(art != null ? art.promoStandSuperSale : null, N(play, 0.87f, 0.50f, 0.11f, 0.27f));
        DrawSprite(art != null ? art.checkoutCounter : null, N(play, 0.74f, 0.61f, 0.23f, 0.28f));

        DrawInstructions(play);
        DrawBoxes(play);
        DrawCart();
        DrawShelf();
        DrawFlyingBox(play);
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

    void DrawInstructions(Rect play)
    {
        Rect leftHint = N(play, 0.055f, 0.43f, 0.38f, 0.09f);
        Rect rightHint = N(play, 0.57f, 0.84f, 0.34f, 0.08f);

        Fill(leftHint, new Color(0.06f, 0.08f, 0.09f, 0.82f));
        GUI.Label(leftHint, "1  Tap a box     2  Load the cart     3  Move to shelf", instructionStyle);

        Fill(rightHint, new Color(0.06f, 0.08f, 0.09f, 0.82f));
        GUI.Label(rightHint, "4  Tap a MISSING slot to restock", instructionStyle);
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
            {
                Fill(Grow(r, 1.15f), new Color(1f, 0.78f, 0.18f, 0.25f));
                r = Grow(r, 1.10f);
            }
            DrawSprite(sprite, r);
        }
    }

    void DrawCart()
    {
        Rect r = CartRect;
        if (tapFlow != null && tapFlow.IsMovingCart)
            Fill(Grow(r, 1.06f), new Color(0.20f, 0.70f, 1f, 0.16f));

        DrawSprite(art != null ? art.shoppingCart : null, r);

        if (cart != null && cart.GetCount() > 0)
        {
            Rect badge = new Rect(r.xMax - 62f, r.y + 8f, 56f, 30f);
            Fill(badge, new Color(0.08f, 0.10f, 0.11f, 0.92f));
            GUI.Label(badge, cart.GetCount() + "/6", smallStyle);
        }
    }

    void DrawShelf()
    {
        Rect r = ShelfRect;
        DrawSprite(art != null ? art.drinkShelf : null, r);

        int count = shelf != null ? Mathf.Clamp(shelf.currentCount, 0, 6) : 0;
        Sprite product = art != null ? (art.colaBox != null ? art.colaBox : art.drinkBox) : null;

        for (int i = 0; i < count; i++)
            DrawSprite(product, GetShelfSlotRect(r, i));

        for (int i = count; i < 6; i++)
        {
            Rect m = GetShelfSlotRect(r, i);
            Fill(m, new Color(0.10f, 0.12f, 0.13f, 0.22f));
            GUI.Box(m, "MISSING", missingStyle);
        }
    }

    void DrawFlyingBox(Rect play)
    {
        if (tapFlow == null || tapFlow.FlyingBox == null || art == null) return;

        Sprite product = art.colaBox != null ? art.colaBox : art.drinkBox;
        float t = tapFlow.FlyingVisualProgress;
        float eased = 1f - Mathf.Pow(1f - t, 3f);

        Rect start;
        Rect end;

        if (tapFlow.FlyingToCart)
        {
            int index = Array.IndexOf(boxes, tapFlow.FlyingBox);
            start = index >= 0 ? GetBoxRect(play, index) : N(play, 0.20f, 0.66f, 0.11f, 0.17f);
            Rect c = CartRect;
            end = new Rect(c.center.x - c.width * 0.20f, c.center.y - c.height * 0.18f, c.width * 0.40f, c.height * 0.36f);
        }
        else
        {
            Rect c = CartRect;
            start = new Rect(c.center.x - c.width * 0.20f, c.center.y - c.height * 0.18f, c.width * 0.40f, c.height * 0.36f);
            int slot = shelf != null ? Mathf.Clamp(shelf.currentCount, 0, 5) : 0;
            end = GetShelfSlotRect(ShelfRect, slot);
        }

        Rect flying = LerpRect(start, end, eased);
        flying.y -= Mathf.Sin(t * Mathf.PI) * Mathf.Max(32f, play.height * 0.10f);
        flying = Grow(flying, 1f + Mathf.Sin(t * Mathf.PI) * 0.12f);
        DrawSprite(product, flying);
    }

    void DrawCustomers(Rect play)
    {
        if (art == null || customers == null) return;

        for (int i = 0; i < customers.Length; i++)
        {
            CustomerAI customer = customers[i];
            if (customer == null) continue;

            Sprite sprite = art.GetCustomer(Mathf.Abs(customer.GetInstanceID()));
            float nx = Mathf.InverseLerp(-10f, 10f, customer.transform.position.x);
            float ny = Mathf.InverseLerp(7f, -7f, customer.transform.position.z);

            Rect r = new Rect(
                play.x + nx * play.width - play.width * 0.035f,
                play.y + ny * play.height - play.height * 0.11f,
                play.width * 0.07f,
                play.height * 0.22f
            );
            DrawSprite(sprite, r);
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

    Rect GetShelfSlotRect(Rect shelfRect, int index)
    {
        int col = index % 3;
        int row = index / 3;
        return new Rect(
            shelfRect.x + shelfRect.width * (0.20f + col * 0.20f),
            shelfRect.y + shelfRect.height * (0.29f + row * 0.24f),
            shelfRect.width * 0.17f,
            shelfRect.height * 0.21f
        );
    }

    void DrawZoneLabel(Rect rect, string text, Color color)
    {
        Fill(rect, color);
        GUI.Label(rect, text, zoneStyle);
    }

    void DrawSprite(Sprite sprite, Rect rect)
    {
        if (sprite == null || sprite.texture == null) return;

        Rect tr = sprite.textureRect;
        Texture2D tex = sprite.texture;
        Rect uv = new Rect(tr.x / tex.width, tr.y / tex.height, tr.width / tex.width, tr.height / tex.height);

        if (cutoutMaterial != null && Event.current.type == EventType.Repaint)
        {
            Graphics.DrawTexture(rect, tex, uv, 0, 0, 0, 0, cutoutMaterial);
            return;
        }

        if (cutoutMaterial == null)
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
        missingStyle.alignment = TextAnchor.MiddleCenter;
        missingStyle.fontSize = 10;
        missingStyle.fontStyle = FontStyle.Bold;
        missingStyle.normal.textColor = new Color(0.95f, 0.25f, 0.20f);

        instructionStyle = new GUIStyle(GUI.skin.label);
        instructionStyle.alignment = TextAnchor.MiddleCenter;
        instructionStyle.fontSize = Mathf.RoundToInt(Mathf.Clamp(Screen.height * 0.019f, 12f, 18f));
        instructionStyle.fontStyle = FontStyle.Bold;
        instructionStyle.normal.textColor = new Color(0.95f, 0.96f, 0.97f);
    }

    static Rect N(Rect parent, float x, float y, float w, float h)
    {
        return new Rect(parent.x + parent.width * x, parent.y + parent.height * y, parent.width * w, parent.height * h);
    }

    static Rect LerpRect(Rect a, Rect b, float t)
    {
        t = Mathf.Clamp01(t);
        return new Rect(
            Mathf.Lerp(a.x, b.x, t),
            Mathf.Lerp(a.y, b.y, t),
            Mathf.Lerp(a.width, b.width, t),
            Mathf.Lerp(a.height, b.height, t)
        );
    }

    static Rect Grow(Rect r, float factor)
    {
        Vector2 center = r.center;
        r.width *= factor;
        r.height *= factor;
        r.center = center;
        return r;
    }
}