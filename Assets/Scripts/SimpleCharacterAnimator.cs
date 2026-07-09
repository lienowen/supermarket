using UnityEngine;

public class SimpleCharacterAnimator : MonoBehaviour
{
    public Transform leftArm;
    public Transform rightArm;
    public Transform leftLeg;
    public Transform rightLeg;
    public Transform body;

    public float walkSwing = 28f;
    public float walkFrequency = 8f;
    public float movementThreshold = 0.02f;
    public float idleBob = 0.025f;

    private Vector3 lastWorldPosition;
    private Vector3 baseLocalPosition;
    private Quaternion leftArmBase;
    private Quaternion rightArmBase;
    private Quaternion leftLegBase;
    private Quaternion rightLegBase;

    void Start()
    {
        lastWorldPosition = transform.position;
        baseLocalPosition = transform.localPosition;

        if (leftArm != null) leftArmBase = leftArm.localRotation;
        if (rightArm != null) rightArmBase = rightArm.localRotation;
        if (leftLeg != null) leftLegBase = leftLeg.localRotation;
        if (rightLeg != null) rightLegBase = rightLeg.localRotation;
    }

    void LateUpdate()
    {
        float moved = (transform.position - lastWorldPosition).magnitude;
        bool walking = moved > movementThreshold * Time.deltaTime;
        lastWorldPosition = transform.position;

        float phase = Time.time * walkFrequency;
        float swing = walking ? Mathf.Sin(phase) * walkSwing : 0f;

        if (leftArm != null)
            leftArm.localRotation = leftArmBase * Quaternion.Euler(swing, 0f, 0f);

        if (rightArm != null)
            rightArm.localRotation = rightArmBase * Quaternion.Euler(-swing, 0f, 0f);

        if (leftLeg != null)
            leftLeg.localRotation = leftLegBase * Quaternion.Euler(-swing, 0f, 0f);

        if (rightLeg != null)
            rightLeg.localRotation = rightLegBase * Quaternion.Euler(swing, 0f, 0f);

        float bob = walking
            ? Mathf.Abs(Mathf.Sin(phase)) * idleBob * 1.8f
            : Mathf.Sin(Time.time * 2f) * idleBob;

        transform.localPosition = baseLocalPosition + Vector3.up * bob;
    }
}
