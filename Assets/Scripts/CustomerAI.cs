using UnityEngine;

public enum CustomerState
{
    Enter,
    Shopping,
    Checkout,
    Leave
}

public class CustomerAI : MonoBehaviour
{
    [Header("State")]
    public CustomerState state = CustomerState.Enter;
    public string wantedProduct = "cola_box";
    public bool foundProduct;
    public bool paid;

    [Header("Movement")]
    public float moveSpeed = 2.2f;
    public float arrivalDistance = 0.25f;
    public Transform shelfPoint;
    public Transform exitPoint;

    [Header("Shopping")]
    public float shoppingDuration = 1.2f;
    public int purchaseValue = 10;

    [Header("Queue")]
    public CheckoutQueueSystem checkoutQueue;
    public float patience = 45f;

    private float stateTimer;
    private float queueSeconds;
    private float remainingPatience;
    private Vector3 queueTarget;
    private bool hasQueueTarget;
    private bool joinedQueue;
    private bool leavingNotified;

    void Start()
    {
        remainingPatience = Mathf.Max(1f, patience);
    }

    void Update()
    {
        switch (state)
        {
            case CustomerState.Enter:
                UpdateEntering();
                break;
            case CustomerState.Shopping:
                UpdateShopping();
                break;
            case CustomerState.Checkout:
                UpdateCheckout();
                break;
            case CustomerState.Leave:
                UpdateLeaving();
                break;
        }
    }

    public void ConfigureRoute(Transform shelf, CheckoutQueueSystem queue, Transform exit)
    {
        shelfPoint = shelf;
        checkoutQueue = queue;
        exitPoint = exit;
    }

    void UpdateEntering()
    {
        if (MoveTo(shelfPoint))
        {
            state = CustomerState.Shopping;
            stateTimer = 0f;
        }
    }

    void UpdateShopping()
    {
        stateTimer += Time.deltaTime;
        if (stateTimer < shoppingDuration) return;

        foundProduct = TryCollectProduct();
        if (!foundProduct)
        {
            RecordSatisfaction(false);
            BeginLeave();
            return;
        }

        state = CustomerState.Checkout;
        TryJoinQueue();
    }

    void UpdateCheckout()
    {
        queueSeconds += Time.deltaTime;
        remainingPatience -= Time.deltaTime;

        if (!joinedQueue)
            TryJoinQueue();

        if (hasQueueTarget)
            MoveToPosition(queueTarget);

        if (remainingPatience <= 0f)
        {
            if (checkoutQueue != null)
                checkoutQueue.Leave(this);

            RecordSatisfaction(false);
            BeginLeave();
        }
    }

    void UpdateLeaving()
    {
        if (!MoveTo(exitPoint)) return;

        if (leavingNotified) return;
        leavingNotified = true;

        CustomerLifeCycle life = GetComponent<CustomerLifeCycle>();
        if (life != null)
            life.LeaveStore();
        else
            Destroy(gameObject);
    }

    void TryJoinQueue()
    {
        if (joinedQueue) return;

        if (checkoutQueue == null)
        {
            CompleteCheckout();
            return;
        }

        joinedQueue = checkoutQueue.Join(this);
    }

    bool TryCollectProduct()
    {
        ProductData product = ProductDatabase.Instance != null
            ? ProductDatabase.Instance.GetProduct(wantedProduct)
            : null;

        ShelfSystem[] shelves = FindObjectsOfType<ShelfSystem>();
        foreach (ShelfSystem shelf in shelves)
        {
            if (shelf == null || shelf.currentCount <= 0) continue;
            if (product != null && shelf.category != product.category) continue;

            shelf.currentCount--;
            return true;
        }

        return false;
    }

    public void SetQueueTarget(Vector3 target)
    {
        queueTarget = target;
        hasQueueTarget = true;
    }

    public bool IsAtQueueTarget(float tolerance = 0.35f)
    {
        if (!hasQueueTarget) return false;

        Vector3 delta = queueTarget - transform.position;
        delta.y = 0f;
        return delta.sqrMagnitude <= tolerance * tolerance;
    }

    public void CompleteCheckout()
    {
        if (paid || state == CustomerState.Leave) return;

        paid = true;
        joinedQueue = false;
        hasQueueTarget = false;

        int finalPrice = purchaseValue;
        PricingSystem pricing = FindObjectOfType<PricingSystem>();
        if (pricing != null)
            finalPrice = pricing.GetSellPrice(wantedProduct);

        if (EconomySystem.Instance != null)
            EconomySystem.Instance.AddIncome(finalPrice);

        if (StoreLevelSystem.Instance != null)
            StoreLevelSystem.Instance.AddExperience(10);

        AchievementSystem achievements = FindObjectOfType<AchievementSystem>();
        if (achievements != null)
        {
            achievements.AddProgress("first_sale", 1);
            achievements.AddProgress("earn_1000", finalPrice);
        }

        RecordSatisfaction(true);
        BeginLeave();
    }

    public void LostPatience()
    {
        remainingPatience = Mathf.Max(0f, remainingPatience - 10f);
    }

    void RecordSatisfaction(bool successfulPurchase)
    {
        CustomerSatisfactionSystem satisfaction = FindObjectOfType<CustomerSatisfactionSystem>();
        if (satisfaction != null)
            satisfaction.RecordCheckout(queueSeconds, successfulPurchase && foundProduct);
    }

    void BeginLeave()
    {
        joinedQueue = false;
        hasQueueTarget = false;
        state = CustomerState.Leave;
    }

    bool MoveTo(Transform target)
    {
        if (target == null) return true;
        return MoveToPosition(target.position);
    }

    bool MoveToPosition(Vector3 target)
    {
        Vector3 delta = target - transform.position;
        delta.y = 0f;

        if (delta.sqrMagnitude <= arrivalDistance * arrivalDistance)
            return true;

        Vector3 direction = delta.normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.forward = direction;
        return false;
    }
}
