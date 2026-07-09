using UnityEngine;
using System.Collections.Generic;

public class CheckoutQueueSystem : MonoBehaviour
{
    public Queue<CustomerAI> queue = new Queue<CustomerAI>();
    public bool busy;

    public void Join(CustomerAI customer)
    {
        if(customer != null)
            queue.Enqueue(customer);
    }

    public CustomerAI GetNext()
    {
        if(queue.Count == 0) return null;
        return queue.Dequeue();
    }
}
