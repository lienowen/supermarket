using UnityEngine;

public class CustomerNavigation : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;

    void Update()
    {
        if(target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        if(dir.magnitude > 0.1f)
        {
            transform.position += dir.normalized * speed * Time.deltaTime;
            transform.forward = dir.normalized;
        }
    }
}
