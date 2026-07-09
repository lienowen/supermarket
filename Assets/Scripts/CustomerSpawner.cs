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

        Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        currentCustomers++;
    }
}
