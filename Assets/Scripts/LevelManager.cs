using UnityEngine;

[System.Serializable]
public class LevelRuntimeConfig
{
    public int day;
    public string mission;
    public int targetCount;
    public int reward;
}

public class LevelManager : MonoBehaviour
{
    public LevelRuntimeConfig currentLevel;

    void Start()
    {
        currentLevel = new LevelRuntimeConfig();
        currentLevel.day = 1;
        currentLevel.mission = "Restock Drinks";
        currentLevel.targetCount = 10;
        currentLevel.reward = 200;
    }
}
