using UnityEngine;

public class MissionSystem : MonoBehaviour
{
    public int targetAmount = 10;
    public int currentAmount = 0;
    public bool completed = false;

    public string missionName = "Restock Drinks";

    public void AddRestock()
    {
        if (completed) return;

        currentAmount++;

        if (currentAmount >= targetAmount)
        {
            completed = true;
            ScoreSystem.Instance.CompleteMission();
        }
    }

    public string GetProgress()
    {
        return currentAmount + "/" + targetAmount;
    }
}
