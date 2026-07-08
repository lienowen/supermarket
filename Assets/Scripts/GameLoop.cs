using UnityEngine;

public enum GameState
{
    Ready,
    Playing,
    Complete,
    Failed
}

public class GameLoop : MonoBehaviour
{
    public GameState state = GameState.Ready;
    public int progress;
    public int target = 20;

    public void StartDay()
    {
        state = GameState.Playing;
        progress = 0;
    }

    public void AddProgress()
    {
        progress++;

        if(progress >= target)
        {
            state = GameState.Complete;
        }
    }

    public void FailDay()
    {
        state = GameState.Failed;
    }
}
