using UnityEngine;

public class ProductBox : MonoBehaviour, IInteractable
{
    public string productId = "cola_box";
    public bool picked = false;
    public Transform currentHolder;

    public void Interact(GameObject player)
    {
        if(picked)
        {
            Drop();
            return;
        }

        Pickup(player.transform);
    }

    public void Pickup(Transform holder)
    {
        picked = true;
        currentHolder = holder;
        transform.SetParent(holder);
        transform.localPosition = new Vector3(0, 1, 1);
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        picked = false;
        currentHolder = null;
        transform.SetParent(null);
    }
}
