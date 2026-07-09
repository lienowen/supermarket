using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance;

    public int speedBonus;
    public int accuracyScore;
    public int completionScore;

    void Awake()
    {
        Instance = this;
    }

    public void CompleteMission()
    {
        completionScore = 1000;
        CalculateFinalScore();
    }

    public int CalculateFinalScore()
    {
        return speedBonus + accuracyScore + completionScore;
    }
}
