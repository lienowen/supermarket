using UnityEngine;

public class DayFlowManager : MonoBehaviour
{
    public int currentDay = 1;
    public bool completed;

    public void CompleteDay()
    {
        completed = true;
    }

    public void NextDay()
    {
        if(!completed)
            return;

        currentDay++;
        completed = false;
    }
}
