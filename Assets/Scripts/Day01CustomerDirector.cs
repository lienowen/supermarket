using UnityEngine;

public class Day01CustomerDirector : MonoBehaviour
{
    public MissionSystem mission;
    public Transform spawnPoint;
    public Transform shelfPoint;
    public Transform checkoutPoint;
    public Transform exitPoint;
    public int maxCustomers = 5;
    public float spawnInterval = 3f;

    private int spawned;
    private float timer;

    void Update()
    {
        if (mission == null || !mission.completed || spawned >= maxCustomers)
            return;

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;

        timer = 0f;
        SpawnCustomer();
    }

    void SpawnCustomer()
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject customer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        customer.name = "Customer_" + (spawned + 1);
        customer.transform.position = position;

        Renderer renderer = customer.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.HSVToRGB((spawned * 0.17f) % 1f, 0.55f, 0.95f);

        SimpleCustomerFlow flow = customer.AddComponent<SimpleCustomerFlow>();
        flow.shelfPoint = shelfPoint;
        flow.checkoutPoint = checkoutPoint;
        flow.exitPoint = exitPoint;
        flow.purchaseValue = 35 + spawned * 5;

        spawned++;
    }
}
