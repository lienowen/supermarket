using UnityEngine;

public class EmployeeAI : MonoBehaviour
{
    public EmployeeType type = EmployeeType.Restocker;
    public float actionInterval = 3f;
    public float efficiency = 1f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < Mathf.Max(0.25f, actionInterval / Mathf.Max(0.1f, efficiency))) return;

        timer = 0f;
        PerformJob();
    }

    void PerformJob()
    {
        switch (type)
        {
            case EmployeeType.Restocker:
                RestockNearestShelf();
                break;
            case EmployeeType.Cashier:
                ServeCheckout();
                break;
            case EmployeeType.Cleaner:
                break;
        }
    }

    void RestockNearestShelf()
    {
        ShelfSystem[] shelves = FindObjectsOfType<ShelfSystem>();
        foreach (ShelfSystem shelf in shelves)
        {
            if (shelf.currentCount < shelf.capacity)
            {
                shelf.currentCount++;
                return;
            }
        }
    }

    void ServeCheckout()
    {
        CheckoutQueueSystem queue = FindObjectOfType<CheckoutQueueSystem>();
        if (queue == null) return;

        CustomerAI customer = queue.GetNext();
        if (customer != null)
            customer.state = CustomerState.Checkout;
    }
}
