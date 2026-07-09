using UnityEngine;

public class MobileInputController : MonoBehaviour
{
    public Vector2 joystickInput;
    public PlayerController player;

    public void SetMove(Vector2 value)
    {
        joystickInput = value;
    }

    void Update()
    {
        if(player == null) return;

        player.transform.position += new Vector3(
            joystickInput.x,
            0,
            joystickInput.y
        ) * player.moveSpeed * Time.deltaTime;
    }
}
