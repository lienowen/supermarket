using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public float interactDistance = 2f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            IInteractable target = hit.collider.GetComponent<IInteractable>();
            if (target != null)
            {
                target.Interact(gameObject);
            }
        }
    }
}

public interface IInteractable
{
    void Interact(GameObject player);
}
