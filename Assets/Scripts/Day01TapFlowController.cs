using UnityEngine;

/// <summary>
/// Fixed-camera Day01 flow. Screen-space hit areas come from Day01ScreenPresentation,
/// while the existing cart/shelf/mission systems remain the source of truth.
/// </summary>
public class Day01TapFlowController : MonoBehaviour
{
    public Camera gameplayCamera;
    public CartSystem cart;
    public ShelfSystem shelf;
    public MissionSystem mission;
    public Day01ScreenPresentation presentation;

    [Header("Flow")]
    public int boxesRequiredBeforeMove = 6;
    public float cartMoveSpeed = 6f;
    public Vector3 cartShelfOffset = new Vector3(-2.4f, 0f, 1.1f);

    [Header("Tap Animation")]
    public float itemFlyDuration = 0.42f;
    public float itemFlyArc = 1.2f;
    public float selectedScale = 1.1f;

    private ProductBox selectedBox;
    private Vector3 selectedOriginalScale;
    private Vector3 cartTarget;
    private Vector3 cartMoveStart;
    private bool movingCart;
    private bool cartAtShelf;

    private ProductBox flyingBox;
    private FlyAction flyAction;
    private Vector3 flyStart;
    private Vector3 flyEnd;
    private Vector3 flyStartScale;
    private float flyTimer;

    enum FlyAction
    {
        None,
        LoadCart,
        RestockShelf
    }

    public ProductBox SelectedBox => selectedBox;
    public ProductBox FlyingBox => flyingBox;
    public bool IsMovingCart => movingCart;
    public bool CartAtShelf => cartAtShelf;
    public float FlyingVisualProgress => flyingBox != null ? Mathf.Clamp01(flyTimer) : 0f;
    public bool FlyingToCart => flyingBox != null && flyAction == FlyAction.LoadCart;

    public float CartVisualProgress
    {
        get
        {
            if (cartAtShelf) return 1f;
            if (!movingCart || cart == null) return 0f;
            float total = Vector3.Distance(cartMoveStart, cartTarget);
            if (total <= 0.001f) return 1f;
            return 1f - Mathf.Clamp01(Vector3.Distance(cart.transform.position, cartTarget) / total);
        }
    }

    public string CurrentHint
    {
        get
        {
            if (mission != null && mission.completed)
                return "Store open • customers are shopping";
            if (flyingBox != null)
                return flyAction == FlyAction.LoadCart ? "Loading box..." : "Restocking shelf...";
            if (movingCart)
                return "Moving cart to the sales floor";
            if (cartAtShelf)
                return cart != null && cart.GetCount() > 0
                    ? "Tap a missing shelf slot to restock"
                    : "Shelf stocked • opening store";
            if (selectedBox != null)
                return "Tap the cart to load the selected box";
            if (cart != null && cart.GetCount() >= boxesRequiredBeforeMove)
                return "Cart ready • tap the cart to move it";
            if (cart != null && cart.GetCount() > 0)
                return "Load cart  " + cart.GetCount() + "/" + boxesRequiredBeforeMove;
            return "Tap a drink box to select it";
        }
    }

    void Awake()
    {
        if (gameplayCamera == null)
            gameplayCamera = Camera.main;
    }

    void Update()
    {
        UpdateFlyingBox();
        UpdateCartMovement();

        if (movingCart || flyingBox != null)
            return;

        if (TryGetPointerDown(out Vector2 screenPosition))
            HandleTap(screenPosition);
    }

    public bool IsBoxInTransit(ProductBox box)
    {
        return box != null && flyingBox == box;
    }

    void HandleTap(Vector2 screenPosition)
    {
        if (presentation != null)
        {
            if (presentation.TryGetBoxIndexAt(screenPosition, out int boxIndex))
            {
                ProductBox box = presentation.GetBoxByIndex(boxIndex);
                if (box != null) SelectBox(box);
                return;
            }

            if (presentation.HitCart(screenPosition))
            {
                HandleCartTap(cart);
                return;
            }

            if (presentation.HitShelf(screenPosition))
            {
                HandleShelfTap(shelf);
                return;
            }
        }

        if (gameplayCamera == null) return;
        Ray ray = gameplayCamera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f)) return;

        ProductBox worldBox = hit.collider.GetComponentInParent<ProductBox>();
        if (worldBox != null)
        {
            SelectBox(worldBox);
            return;
        }

        CartSystem tappedCart = hit.collider.GetComponentInParent<CartSystem>();
        if (tappedCart != null)
        {
            HandleCartTap(tappedCart);
            return;
        }

        ShelfSystem tappedShelf = hit.collider.GetComponentInParent<ShelfSystem>();
        if (tappedShelf != null)
            HandleShelfTap(tappedShelf);
    }

    void SelectBox(ProductBox box)
    {
        if (box == null || box.picked || movingCart || cartAtShelf || box.transform.parent != null)
            return;

        ClearSelectionVisual();
        selectedBox = box;
        selectedOriginalScale = box.transform.localScale;
        box.transform.localScale = selectedOriginalScale * selectedScale;
    }

    void HandleCartTap(CartSystem tappedCart)
    {
        if (tappedCart == null || tappedCart != cart) return;

        if (selectedBox != null)
        {
            ProductBox box = selectedBox;
            ClearSelectionVisual();
            BeginFly(box, GetCartLoadPoint(), FlyAction.LoadCart);
            return;
        }

        if (cartAtShelf || shelf == null) return;
        if (cart.GetCount() < boxesRequiredBeforeMove) return;

        cartMoveStart = cart.transform.position;
        cartTarget = shelf.transform.position + cartShelfOffset;
        cartTarget.y = cart.transform.position.y;
        movingCart = true;
    }

    void HandleShelfTap(ShelfSystem tappedShelf)
    {
        if (tappedShelf == null || tappedShelf != shelf || !cartAtShelf || cart == null) return;

        ProductBox product = cart.RemoveOneProduct();
        if (product == null) return;

        product.gameObject.SetActive(true);
        BeginFly(product, GetShelfDropPoint(), FlyAction.RestockShelf);
    }

    void BeginFly(ProductBox box, Vector3 target, FlyAction action)
    {
        if (box == null) return;

        flyingBox = box;
        flyAction = action;
        flyStart = box.transform.position;
        flyEnd = target;
        flyStartScale = box.transform.localScale;
        flyTimer = 0f;

        box.transform.SetParent(null, true);
        Collider collider = box.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
    }

    void UpdateFlyingBox()
    {
        if (flyingBox == null) return;

        float duration = Mathf.Max(0.05f, itemFlyDuration);
        flyTimer += Time.deltaTime / duration;
        float t = Mathf.Clamp01(flyTimer);
        float eased = 1f - Mathf.Pow(1f - t, 3f);

        Vector3 position = Vector3.Lerp(flyStart, flyEnd, eased);
        position.y += Mathf.Sin(t * Mathf.PI) * itemFlyArc;
        flyingBox.transform.position = position;
        flyingBox.transform.localScale = flyStartScale * (1f + Mathf.Sin(t * Mathf.PI) * 0.12f);

        if (t < 1f) return;

        ProductBox completed = flyingBox;
        FlyAction completedAction = flyAction;
        flyingBox = null;
        flyAction = FlyAction.None;
        completed.transform.position = flyEnd;
        completed.transform.localScale = flyStartScale;

        if (completedAction == FlyAction.LoadCart)
        {
            if (!cart.AddProduct(completed))
            {
                Collider collider = completed.GetComponent<Collider>();
                if (collider != null) collider.enabled = true;
            }
            return;
        }

        if (completedAction == FlyAction.RestockShelf)
            shelf.Restock(completed);
    }

    void UpdateCartMovement()
    {
        if (!movingCart || cart == null) return;

        cart.transform.position = Vector3.MoveTowards(cart.transform.position, cartTarget, cartMoveSpeed * Time.deltaTime);
        if ((cart.transform.position - cartTarget).sqrMagnitude <= 0.01f)
        {
            cart.transform.position = cartTarget;
            movingCart = false;
            cartAtShelf = true;
        }
    }

    Vector3 GetCartLoadPoint()
    {
        if (cart == null) return Vector3.zero;
        int count = cart.GetCount();
        return cart.transform.position + new Vector3(0f, 1.0f + count * 0.12f, 0f);
    }

    Vector3 GetShelfDropPoint()
    {
        if (shelf == null) return Vector3.zero;
        int count = shelf.currentCount;
        float x = ((count % 3) - 1) * 0.42f;
        float y = 0.9f + (count / 3) * 0.52f;
        return shelf.transform.position + new Vector3(x, y, -0.35f);
    }

    void ClearSelectionVisual()
    {
        if (selectedBox != null)
            selectedBox.transform.localScale = selectedOriginalScale;
        selectedBox = null;
    }

    bool TryGetPointerDown(out Vector2 screenPosition)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                screenPosition = touch.position;
                return true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            screenPosition = Input.mousePosition;
            return true;
        }

        screenPosition = Vector2.zero;
        return false;
    }
}