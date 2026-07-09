using UnityEngine;

public class StylizedCameraRig : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 1.15f, 0f);
    public float distance = 10.5f;
    public float height = 6.2f;
    public float yaw = 28f;
    public float positionSmooth = 7f;
    public float rotationSmooth = 10f;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 focus = target.position + targetOffset;
        Vector3 localOffset = new Vector3(0f, height, -distance);
        Vector3 desired = focus + Quaternion.Euler(0f, yaw, 0f) * localOffset;

        float smoothTime = 1f / Mathf.Max(0.01f, positionSmooth);
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref velocity,
            smoothTime
        );

        Vector3 lookDirection = focus - transform.position;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                1f - Mathf.Exp(-rotationSmooth * Time.deltaTime)
            );
        }
    }
}
