using UnityEngine;

public class WorldSpriteBillboard : MonoBehaviour
{
    public bool keepVertical = true;
    private Camera cachedCamera;

    void LateUpdate()
    {
        if (cachedCamera == null)
            cachedCamera = Camera.main;

        if (cachedCamera == null) return;

        Vector3 direction = cachedCamera.transform.position - transform.position;
        if (keepVertical)
            direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.LookRotation(-direction.normalized, Vector3.up);
    }
}
