using UnityEngine;

public class CustomerShoppingSystem : MonoBehaviour
{
    public string targetProduct = "cola_box";
    public bool bought;

    public void BuyProduct()
    {
        if(bought) return;

        bought = true;

        if(EconomySystem.Instance != null)
        {
            EconomySystem.Instance.AddIncome(10);
        }
    }
}
