using UnityEngine;

public class Day01CustomerDirector : MonoBehaviour
{
    public MissionSystem mission;
    public CustomerSpawner spawner;
    public int totalCustomers = 5;
    public float spawnInterval = 3f;

    private int spawned;
    private float timer;
    private bool waveFinished;

    void Update()
    {
        if (mission == null || spawner == null) return;

        if (mission.completed && !waveFinished && spawned >= totalCustomers && spawner.CurrentCustomers == 0)
        {
            waveFinished = true;

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.CompleteGame();

            return;
        }

        if (!mission.completed || spawned >= totalCustomers) return;

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

    public int GetCompletedCustomers()
    {
        if (spawner == null) return 0;
        return Mathf.Max(0, spawned - spawner.CurrentCustomers);
    }

    public bool IsWaveFinished()
    {
        return waveFinished;
    }
}
