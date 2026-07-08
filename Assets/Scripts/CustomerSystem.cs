using UnityEngine;

public class CustomerSystem : MonoBehaviour
{
    public int customers = 0;
    public int satisfaction = 100;

    public void AddCustomer()
    {
        customers++;
    }

    public void ReduceSatisfaction(int value)
    {
        satisfaction -= value;
        if(satisfaction < 0)
            satisfaction = 0;
    }
}
