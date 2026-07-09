using UnityEngine;

public class Day01Bootstrap : MonoBehaviour
{
    public PrefabRegistry registry;
    public MissionSystem mission;
    public EconomySystem economy;

    void Start()
    {
        if(registry != null)
        {
            registry.SpawnDay01Objects();
        }

        if(mission != null)
        {
            mission.currentAmount = 0;
            mission.completed = false;
        }

        if(economy != null)
        {
            economy.totalIncome = 0;
        }
    }
}
