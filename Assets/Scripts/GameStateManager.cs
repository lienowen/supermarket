using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public enum State
    {
        Loading,
        Playing,
        Completed,
        GameOver
    }

    public State currentState = State.Loading;

    void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        currentState = State.Playing;
    }

    public void CompleteGame()
    {
        currentState = State.Completed;
    }
}
