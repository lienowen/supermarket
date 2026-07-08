using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public int CalculateReward(int timeLeft, int accuracy)
    {
        int reward = 100;
        reward += timeLeft;
        reward += accuracy * 10;
        return reward;
    }
}
