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
    public string wantedProduct = "cola_box";
    public float patience = 100f;
    public int purchaseValue = 10;

    void Update()
    {
        switch(state)
        {
            case CustomerState.Enter:
                FindProduct();
                break;
            case CustomerState.Shopping:
                GoCheckout();
                break;
            case CustomerState.Checkout:
                Pay();
                break;
            case CustomerState.Leave:
                break;
        }
    }

    void FindProduct()
    {
        state = CustomerState.Shopping;
    }

    void GoCheckout()
    {
        state = CustomerState.Checkout;
    }

    void Pay()
    {
        if(EconomySystem.Instance != null)
            EconomySystem.Instance.AddIncome(purchaseValue);

        state = CustomerState.Leave;
    }

    public void LostPatience()
    {
        patience -= 10;
        if(patience <= 0)
            state = CustomerState.Leave;
    }
}
