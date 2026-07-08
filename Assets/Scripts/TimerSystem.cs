using UnityEngine;

public class TimerSystem : MonoBehaviour
{
    public float timeLimit = 180f;
    public float remainTime;
    public bool running;

    void Start()
    {
        remainTime = timeLimit;
        running = true;
    }

    void Update()
    {
        if(!running)
            return;

        remainTime -= Time.deltaTime;

        if(remainTime <= 0)
        {
            remainTime = 0;
            running = false;
            Debug.Log("Mission Failed");
        }
    }
}
