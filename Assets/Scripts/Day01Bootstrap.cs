using UnityEngine;

public class Day01Bootstrap : MonoBehaviour
{
    public PrefabRegistry registry;
    public MissionSystem mission;
    public EconomySystem economy;

    void Start()
    {
        ValidateSetup();

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

    void ValidateSetup()
    {
        if(registry == null)
            Debug.LogWarning("Day01Bootstrap: PrefabRegistry missing");

        if(mission == null)
            Debug.LogWarning("Day01Bootstrap: MissionSystem missing");

        if(economy == null)
            Debug.LogWarning("Day01Bootstrap: EconomySystem missing");
    }
}
