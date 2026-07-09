using UnityEngine;

public class RestockFlowController : MonoBehaviour
{
    public CartSystem cart;
    public ShelfSystem drinkShelf;
    public MissionSystem mission;

    public void DeliverCartToShelf()
    {
        if (cart.GetCount() <= 0) return;

        while (cart.GetCount() > 0 && !drinkShelf.IsComplete())
        {
            drinkShelf.currentCount++;
            mission.AddRestock();
            break;
        }
    }

    public bool IsFinished()
    {
        return mission.completed;
    }
}
