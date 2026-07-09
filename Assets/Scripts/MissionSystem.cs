using UnityEngine;

public class MissionSystem : MonoBehaviour
{
    public int targetAmount = 10;
    public int currentAmount;
    public bool completed;
    public string missionName = "Restock Drinks";
    public int reward = 200;
    public bool rewardClaimed;

    public void AddRestock()
    {
        AddRestock(1);
    }

    public void AddRestock(int amount)
    {
        if (completed || amount <= 0) return;

        currentAmount = Mathf.Min(currentAmount + amount, targetAmount);

        AchievementSystem achievements = FindObjectOfType<AchievementSystem>();
        if (achievements != null)
            achievements.AddProgress("restock_10", amount);

        if (currentAmount >= targetAmount)
        {
            completed = true;

            if (!rewardClaimed && EconomySystem.Instance != null)
            {
                EconomySystem.Instance.AddReward(reward);
                rewardClaimed = true;
            }

            if (ScoreSystem.Instance != null)
                ScoreSystem.Instance.CompleteMission();
        }
    }

    public string GetProgress()
    {
        return currentAmount + "/" + targetAmount;
    }
}
