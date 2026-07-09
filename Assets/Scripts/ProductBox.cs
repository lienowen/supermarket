using UnityEngine;

public class ProductBox : MonoBehaviour, IInteractable
{
    public string productId = "cola_box";
    public bool picked = false;

    public void Interact(GameObject player)
    {
        picked = !picked;
        transform.SetParent(picked ? player.transform : null);
        if (picked)
        {
            transform.localPosition = new Vector3(0,1,1);
        }
    }
}
