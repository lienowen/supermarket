using UnityEngine;

public class Day01CustomerDirector : MonoBehaviour
{
    public MissionSystem mission;
    public CustomerSpawner spawner;
    public int totalCustomers = 5;
    public float spawnInterval = 3f;

    private int spawned;
    private float timer;

    void Update()
    {
        if (mission == null || !mission.completed) return;
        if (spawner == null || spawned >= totalCustomers) return;

        timer += Time.deltaTime;
        if (timer < Mathf.Max(0.5f, spawnInterval)) return;

        timer = 0f;
        GameObject customer = spawner.SpawnCustomer();
        if (customer != null)
            spawned++;
    }

    public int GetRemainingCustomers()
    {
        return Mathf.Max(0, totalCustomers - spawned);
    }
}
