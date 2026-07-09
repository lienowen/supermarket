using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public int maxCustomers = 10;
    public float spawnInterval = 10f;
    public bool autoSpawn = true;

    [Header("Route")]
    public Transform shelfPoint;
    public CheckoutQueueSystem checkoutQueue;
    public Transform exitPoint;

    private int currentCustomers;
    private int spawnedSequence;

    public int CurrentCustomers => currentCustomers;

    void Start()
    {
        if (autoSpawn)
            InvokeRepeating(nameof(SpawnCustomer), 2f, Mathf.Max(0.5f, spawnInterval));
    }

    public GameObject SpawnCustomer()
    {
        if (currentCustomers >= maxCustomers) return null;

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject customer;

        if (customerPrefab != null)
        {
            customer = Instantiate(customerPrefab, position, Quaternion.identity);
        }
        else
        {
            customer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            customer.transform.position = position + Vector3.up;
        }

        spawnedSequence++;
        customer.name = "Customer_" + spawnedSequence;
        currentCustomers++;

        CustomerAI ai = customer.GetComponent<CustomerAI>();
        if (ai == null)
            ai = customer.AddComponent<CustomerAI>();

        ai.ConfigureRoute(shelfPoint, checkoutQueue, exitPoint);

        CustomerLifeCycle life = customer.GetComponent<CustomerLifeCycle>();
        if (life == null)
            life = customer.AddComponent<CustomerLifeCycle>();

        life.onLeave += RemoveCustomer;

        Renderer renderer = customer.GetComponentInChildren<Renderer>();
        if (renderer != null && customerPrefab == null)
        {
            float hue = (spawnedSequence * 0.17f) % 1f;
            renderer.material.color = Color.HSVToRGB(hue, 0.55f, 0.95f);
        }

        return customer;
    }

    void RemoveCustomer()
    {
        currentCustomers = Mathf.Max(0, currentCustomers - 1);
    }
}

public class CustomerLifeCycle : MonoBehaviour
{
    public System.Action onLeave;
    private bool leaving;

    public void LeaveStore()
    {
        if (leaving) return;
        leaving = true;

        onLeave?.Invoke();
        Destroy(gameObject);
    }
}
