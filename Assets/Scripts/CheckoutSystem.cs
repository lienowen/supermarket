using UnityEngine;

public class CheckoutSystem : MonoBehaviour
{
    public int queueLimit = 5;
    public int income;
    public CheckoutQueueSystem queueSystem;

    void Awake()
    {
        if (queueSystem == null)
            queueSystem = GetComponent<CheckoutQueueSystem>();

        if (queueSystem == null)
            queueSystem = gameObject.AddComponent<CheckoutQueueSystem>();

        queueSystem.maxQueue = Mathf.Max(1, queueLimit);
    }

    public bool JoinQueue(CustomerAI customer)
    {
        return queueSystem != null && queueSystem.Join(customer);
    }

    public void Checkout()
    {
        if (queueSystem == null) return;

        int before = EconomySystem.Instance != null
            ? EconomySystem.Instance.totalIncome
            : 0;

        if (!queueSystem.ServeNextNow()) return;

        if (EconomySystem.Instance != null)
            income += Mathf.Max(0, EconomySystem.Instance.totalIncome - before);
    }

    public int GetQueueCount()
    {
        return queueSystem != null ? queueSystem.QueueCount : 0;
    }
}
