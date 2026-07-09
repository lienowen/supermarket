using UnityEngine;
using System.Collections.Generic;

public class CheckoutQueueSystem : MonoBehaviour
{
    [Header("Queue")]
    public Transform checkoutPoint;
    public Vector3 queueDirection = Vector3.back;
    public float spacing = 1.1f;
    public int maxQueue = 8;

    [Header("Service")]
    public bool autoServe = true;
    public bool cashierAvailable = true;
    public float serviceSeconds = 2f;
    public bool busy;

    private readonly List<CustomerAI> customers = new List<CustomerAI>();
    private float serviceTimer;

    public int QueueCount => customers.Count;

    void Update()
    {
        CleanupQueue();
        UpdateQueueTargets();

        if (customers.Count == 0)
        {
            busy = false;
            serviceTimer = 0f;
            return;
        }

        if (!autoServe || !cashierAvailable)
        {
            busy = false;
            serviceTimer = 0f;
            return;
        }

        CustomerAI front = customers[0];
        if (front == null || !front.IsAtQueueTarget())
        {
            busy = false;
            serviceTimer = 0f;
            return;
        }

        busy = true;
        serviceTimer += Time.deltaTime;

        if (serviceTimer >= Mathf.Max(0.1f, serviceSeconds))
            ServeNextNow();
    }

    public bool Join(CustomerAI customer)
    {
        if (customer == null) return false;
        if (customers.Contains(customer)) return true;
        if (customers.Count >= maxQueue) return false;

        customers.Add(customer);
        UpdateQueueTargets();
        return true;
    }

    public void Leave(CustomerAI customer)
    {
        if (customer == null) return;

        int index = customers.IndexOf(customer);
        if (index < 0) return;

        customers.RemoveAt(index);
        if (index == 0)
            serviceTimer = 0f;

        UpdateQueueTargets();
    }

    public CustomerAI GetNext()
    {
        CleanupQueue();
        return customers.Count > 0 ? customers[0] : null;
    }

    public bool ServeNextNow()
    {
        CleanupQueue();
        if (customers.Count == 0) return false;

        CustomerAI customer = customers[0];
        customers.RemoveAt(0);

        busy = false;
        serviceTimer = 0f;
        UpdateQueueTargets();

        if (customer != null)
            customer.CompleteCheckout();

        return customer != null;
    }

    void UpdateQueueTargets()
    {
        Vector3 origin = checkoutPoint != null ? checkoutPoint.position : transform.position;
        Vector3 direction = queueDirection.sqrMagnitude > 0.001f
            ? queueDirection.normalized
            : Vector3.back;

        for (int i = 0; i < customers.Count; i++)
        {
            CustomerAI customer = customers[i];
            if (customer == null) continue;

            customer.SetQueueTarget(origin + direction * spacing * i);
        }
    }

    void CleanupQueue()
    {
        for (int i = customers.Count - 1; i >= 0; i--)
        {
            if (customers[i] == null)
                customers.RemoveAt(i);
        }
    }
}
