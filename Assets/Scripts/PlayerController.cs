using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        controller.Move(move.normalized * speed * Time.deltaTime);

        if(move.magnitude > 0)
            transform.forward = move;
    }
}
