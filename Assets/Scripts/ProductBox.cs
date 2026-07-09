using UnityEngine;

public class ProductBox : MonoBehaviour, IInteractable
{
    public string productId = "cola_box";
    public bool picked;
    public Transform currentHolder;

    public void Interact(GameObject player)
    {
        if (player == null) return;

        CarrySystem carry = player.GetComponent<CarrySystem>();
        if (carry == null)
        {
            Debug.LogWarning("ProductBox: player has no CarrySystem");
            return;
        }

        if (picked && currentHolder != null && currentHolder.IsChildOf(player.transform))
        {
            carry.Drop();
            return;
        }

        carry.Pickup(this);
    }

    public void SetCarried(Transform holder)
    {
        picked = true;
        currentHolder = holder;
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        picked = false;
        currentHolder = null;
        transform.SetParent(null);
    }
}
