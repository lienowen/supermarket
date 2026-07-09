using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AchievementData
{
    public string id;
    public string title;
    public int target;
    public int progress;
    public bool unlocked;
}

public class AchievementSystem : MonoBehaviour
{
    public List<AchievementData> achievements = new List<AchievementData>();

    void Awake()
    {
        if (achievements.Count == 0)
        {
            achievements.Add(new AchievementData(){id="first_sale",title="First Sale",target=1});
            achievements.Add(new AchievementData(){id="restock_10",title="Stock Hero",target=10});
            achievements.Add(new AchievementData(){id="earn_1000",title="Growing Business",target=1000});
        }
    }

    public void AddProgress(string id, int amount = 1)
    {
        AchievementData achievement = achievements.Find(x => x.id == id);
        if (achievement == null || achievement.unlocked || amount <= 0) return;

        achievement.progress = Mathf.Min(achievement.progress + amount, achievement.target);
        if (achievement.progress >= achievement.target)
            achievement.unlocked = true;
    }
}
