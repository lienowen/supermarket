using UnityEngine;

public class AdRewardSystem : MonoBehaviour
{
    public bool adReady = true;

    public void ShowRewardAd()
    {
        if(!adReady)
            return;

        GiveReward();
    }

    void GiveReward()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(200);
        }
    }
}
