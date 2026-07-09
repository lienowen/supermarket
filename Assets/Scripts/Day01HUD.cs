using UnityEngine;

public class Day01HUD : MonoBehaviour
{
    public MissionSystem mission;

    void OnGUI()
    {
        GUI.Box(new Rect(16, 16, 300, 120), "DAY 1 - Morning Shift");

        string progress = mission != null ? mission.GetProgress() : "0/10";
        GUI.Label(new Rect(32, 48, 260, 24), "Mission: Restock Drinks " + progress);

        int money = EconomySystem.Instance != null ? EconomySystem.Instance.money : 0;
        GUI.Label(new Rect(32, 72, 260, 24), "Money: $" + money);
        GUI.Label(new Rect(32, 96, 270, 24), "WASD move | E interact | Shift run");

        if (mission != null && mission.completed)
        {
            GUI.Box(new Rect(Screen.width * 0.5f - 160, 30, 320, 80), "MISSION COMPLETE");
            GUI.Label(new Rect(Screen.width * 0.5f - 125, 65, 280, 24), "Customers are now shopping!");
        }
    }
}
