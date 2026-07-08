using UnityEngine;

[System.Serializable]
public class LevelConfigData
{
    public int day;
    public string title;
    public string[] products;
    public int targetAmount;
    public int timeLimit;
    public int reward;
}

public class LevelConfig : MonoBehaviour
{
    public LevelConfigData Day1()
    {
        return new LevelConfigData
        {
            day = 1,
            title = "Restock Drinks",
            products = new string[]{"cola", "water"},
            targetAmount = 20,
            timeLimit = 180,
            reward = 500
        };
    }
}
