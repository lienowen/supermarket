using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public int maxCustomers = 10;
    public float spawnInterval = 10f;

    private int currentCustomers;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCustomer), 2f, spawnInterval);
    }

    void SpawnCustomer()
    {
        if(currentCustomers >= maxCustomers) return;
        if(customerPrefab == null || spawnPoint == null) return;

        GameObject customer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        currentCustomers++;

        CustomerLifeCycle life = customer.AddComponent<CustomerLifeCycle>();
        life.onLeave = RemoveCustomer;
    }

    void RemoveCustomer()
    {
        currentCustomers--;
    }
}

public class CustomerLifeCycle : MonoBehaviour
{
    public System.Action onLeave;

    public void LeaveStore()
    {
        onLeave?.Invoke();
        Destroy(gameObject);
    }
}
