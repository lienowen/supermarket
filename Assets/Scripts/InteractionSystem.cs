using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public float interactDistance = 2.5f;
    public KeyCode interactKey = KeyCode.E;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
            TryInteract();
    }

    public bool TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactDistance);
        IInteractable bestTarget = null;
        float bestScore = float.MaxValue;

        foreach (Collider hit in hits)
        {
            IInteractable target = hit.GetComponent<IInteractable>();
            if (target == null)
                target = hit.GetComponentInParent<IInteractable>();

            if (target == null) continue;

            Vector3 toTarget = hit.transform.position - transform.position;
            float distance = toTarget.magnitude;
            float facingPenalty = Vector3.Dot(transform.forward, toTarget.normalized) < -0.2f ? 2f : 0f;
            float score = distance + facingPenalty;

            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = target;
            }
        }

        if (bestTarget == null)
            return false;

        bestTarget.Interact(gameObject);
        return true;
    }
}

public interface IInteractable
{
    void Interact(GameObject player);
}
