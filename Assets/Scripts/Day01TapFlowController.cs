using UnityEngine;

/// <summary>
/// Fast rescue controller for Day01: fixed-camera, tap/click interactions.
/// Keeps the existing economy, mission, cart and shelf systems, but removes the
/// need for WASD movement and proximity-based E-key interaction.
/// </summary>
public class Day01TapFlowController : MonoBehaviour
{
    public Camera gameplayCamera;
    public CartSystem cart;
    public ShelfSystem shelf;
    public MissionSystem mission;

    [Header("Cart Move")]
    public float cartMoveSpeed = 6f;
    public Vector3 cartShelfOffset = new Vector3(-2.4f, 0f, 1.1f);

    private ProductBox selectedBox;
    private Vector3 selectedOriginalScale;
    private Vector3 cartTarget;
    private bool movingCart;
    private bool cartAtShelf;

    public string CurrentHint
    {
        get
        {
            if (mission != null && mission.completed)
                return "Store open • customers are shopping";
            if (movingCart)
                return "Moving cart to the shelf";
            if (cartAtShelf)
                return "Tap the drink shelf to restock";
            if (selectedBox != null)
                return "Tap the cart to load the selected box";
            if (cart != null && cart.GetCount() > 0)
                return "Tap another box, or tap the cart to move it";
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
        UpdateCartMovement();

        if (movingCart)
            return;

        if (TryGetPointerDown(out Vector2 screenPosition))
            HandleTap(screenPosition);
    }

    void HandleTap(Vector2 screenPosition)
    {
        if (gameplayCamera == null)
            return;

        Ray ray = gameplayCamera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f))
            return;

        ProductBox box = hit.collider.GetComponentInParent<ProductBox>();
        if (box != null)
        {
            SelectBox(box);
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
        if (box == null || box.picked || movingCart || cartAtShelf)
            return;

        ClearSelectionVisual();

        selectedBox = box;
        selectedOriginalScale = box.transform.localScale;
        box.transform.localScale = selectedOriginalScale * 1.08f;
    }

    void HandleCartTap(CartSystem tappedCart)
    {
        if (tappedCart == null || tappedCart != cart)
            return;

        if (selectedBox != null)
        {
            ProductBox box = selectedBox;
            ClearSelectionVisual(false);

            if (!cart.AddProduct(box))
            {
                selectedBox = box;
                selectedOriginalScale = box.transform.localScale;
                box.transform.localScale = selectedOriginalScale * 1.08f;
            }
            return;
        }

        if (cart.GetCount() <= 0 || cartAtShelf || shelf == null)
            return;

        cartTarget = shelf.transform.position + cartShelfOffset;
        cartTarget.y = cart.transform.position.y;
        movingCart = true;
    }

    void HandleShelfTap(ShelfSystem tappedShelf)
    {
        if (tappedShelf == null || tappedShelf != shelf || !cartAtShelf || cart == null)
            return;

        ProductBox product = cart.RemoveOneProduct();
        if (product != null)
            shelf.Restock(product);
    }

    void UpdateCartMovement()
    {
        if (!movingCart || cart == null)
            return;

        cart.transform.position = Vector3.MoveTowards(
            cart.transform.position,
            cartTarget,
            cartMoveSpeed * Time.deltaTime
        );

        if ((cart.transform.position - cartTarget).sqrMagnitude <= 0.01f)
        {
            cart.transform.position = cartTarget;
            movingCart = false;
            cartAtShelf = true;
        }
    }

    void ClearSelectionVisual(bool clearReference = true)
    {
        if (selectedBox != null)
            selectedBox.transform.localScale = selectedOriginalScale;

        if (clearReference)
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