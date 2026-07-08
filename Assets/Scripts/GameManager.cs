using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coins = 450;
    public int score = 0;
    public int day = 1;

    void Awake()
    {
        Instance = this;
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public void AddCoins(int value)
    {
        coins += value;
    }
}
