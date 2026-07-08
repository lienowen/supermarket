using UnityEngine;
using System.Collections.Generic;

public class CheckoutSystem : MonoBehaviour
{
    public int queueLimit = 5;
    public List<CustomerAI> queue = new List<CustomerAI>();
    public int income;

    public bool JoinQueue(CustomerAI customer)
    {
        if(queue.Count >= queueLimit)
            return false;

        queue.Add(customer);
        customer.state = CustomerState.Checkout;
        return true;
    }

    public void Checkout()
    {
        if(queue.Count == 0)
            return;

        queue.RemoveAt(0);
        income += 10;
    }
}
