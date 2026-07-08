using UnityEngine;

public class CartController : MonoBehaviour
{
    public float pushSpeed = 3f;
    public int capacity = 10;
    public int loadCount = 0;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v);

        if(direction.magnitude > 0)
        {
            rb.MovePosition(
                transform.position + direction.normalized * pushSpeed * Time.fixedDeltaTime
            );
        }
    }

    public bool AddBox()
    {
        if(loadCount >= capacity)
            return false;

        loadCount++;
        return true;
    }
}
