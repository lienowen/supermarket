using UnityEngine;

[System.Serializable]
public class LevelConfig
{
    public int day;
    public string mission;
    public int targetCount;
    public int reward;
}

public class LevelManager : MonoBehaviour
{
    public LevelConfig currentLevel;

    void Start()
    {
        currentLevel = new LevelConfig();
        currentLevel.day = 1;
        currentLevel.mission = "Restock Drinks";
        currentLevel.targetCount = 10;
        currentLevel.reward = 200;
    }
}
