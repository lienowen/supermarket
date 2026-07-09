using UnityEngine;

public class MissionSystem : MonoBehaviour
{
    public int targetAmount = 10;
    public int currentAmount;
    public bool completed;
    public string missionName = "Restock Drinks";

    public void AddRestock()
    {
        AddRestock(1);
    }

    public void AddRestock(int amount)
    {
        if (completed || amount <= 0) return;

        currentAmount = Mathf.Min(currentAmount + amount, targetAmount);

        if (currentAmount >= targetAmount)
        {
            completed = true;

            if (ScoreSystem.Instance != null)
                ScoreSystem.Instance.CompleteMission();

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.CompleteGame();
        }
    }

    public string GetProgress()
    {
        return currentAmount + "/" + targetAmount;
    }
}
