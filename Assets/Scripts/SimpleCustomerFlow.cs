using UnityEngine;

public class SimpleCustomerFlow : MonoBehaviour
{
    public Transform shelfPoint;
    public Transform checkoutPoint;
    public Transform exitPoint;
    public float speed = 2.2f;
    public int purchaseValue = 35;

    private int stage;
    private float waitTimer;
    private bool paid;

    void Update()
    {
        switch (stage)
        {
            case 0:
                MoveTo(shelfPoint, 1);
                break;
            case 1:
                waitTimer += Time.deltaTime;
                if (waitTimer >= 1.2f)
                    stage = 2;
                break;
            case 2:
                MoveTo(checkoutPoint, 3);
                break;
            case 3:
                if (!paid)
                {
                    paid = true;
                    if (EconomySystem.Instance != null)
                        EconomySystem.Instance.AddIncome(purchaseValue);

                    AchievementSystem achievements = FindObjectOfType<AchievementSystem>();
                    if (achievements != null)
                        achievements.AddProgress("first_sale");
                }
                stage = 4;
                break;
            case 4:
                MoveTo(exitPoint, 5);
                break;
            case 5:
                Destroy(gameObject);
                break;
        }
    }

    void MoveTo(Transform target, int nextStage)
    {
        if (target == null)
        {
            stage = nextStage;
            return;
        }

        Vector3 delta = target.position - transform.position;
        delta.y = 0f;

        if (delta.magnitude <= 0.25f)
        {
            stage = nextStage;
            return;
        }

        Vector3 direction = delta.normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.forward = direction;
    }
}
