using UnityEngine;

public enum CustomerState
{
    Enter,
    Shopping,
    Checkout,
    Leave
}

public class CustomerAI : MonoBehaviour
{
    public CustomerState state = CustomerState.Enter;
    public string wantedProduct = "cola";
    public float patience = 100f;

    void Update()
    {
        switch(state)
        {
            case CustomerState.Enter:
                state = CustomerState.Shopping;
                break;

            case CustomerState.Shopping:
                break;

            case CustomerState.Checkout:
                break;

            case CustomerState.Leave:
                break;
        }
    }

    public void LostPatience()
    {
        patience -= 10;
        if(patience <= 0)
            state = CustomerState.Leave;
    }
}
