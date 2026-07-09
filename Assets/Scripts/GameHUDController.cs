using UnityEngine;

public class GameHUDController : MonoBehaviour
{
    public string missionText;
    public int money;

    void Update()
    {
        if(EconomySystem.Instance != null)
            money = EconomySystem.Instance.money;
    }

    public void SetMission(string text)
    {
        missionText = text;
    }
}
