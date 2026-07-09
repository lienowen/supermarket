using UnityEngine;

/// <summary>
/// Lightweight sale feedback for WebGL/mobile. Watches the existing economy
/// totals and shows a floating income popup near the checkout.
/// </summary>
public class Day01SaleFeedback : MonoBehaviour
{
    public Camera gameplayCamera;
    public Transform checkoutAnchor;
    public float popupSeconds = 1.15f;

    private int lastIncome;
    private int popupAmount;
    private float popupTimer;
    private GUIStyle popupStyle;

    void Start()
    {
        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        if (EconomySystem.Instance != null)
            lastIncome = EconomySystem.Instance.totalIncome;

        BuildStyle();
    }

    void Update()
    {
        if (EconomySystem.Instance == null)
            return;

        int income = EconomySystem.Instance.totalIncome;
        if (income > lastIncome)
        {
            popupAmount = income - lastIncome;
            popupTimer = popupSeconds;
        }

        lastIncome = income;

        if (popupTimer > 0f)
            popupTimer -= Time.deltaTime;
    }

    void OnGUI()
    {
        if (popupTimer <= 0f || popupAmount <= 0 || gameplayCamera == null || checkoutAnchor == null)
            return;

        if (popupStyle == null)
            BuildStyle();

        Vector3 world = checkoutAnchor.position + new Vector3(0f, 2.8f, 0f);
        Vector3 screen = gameplayCamera.WorldToScreenPoint(world);
        if (screen.z <= 0f) return;

        float progress = 1f - Mathf.Clamp01(popupTimer / Mathf.Max(0.01f, popupSeconds));
        float yLift = progress * 46f;
        float alpha = 1f - Mathf.Clamp01((progress - 0.55f) / 0.45f);

        Color old = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, alpha);

        Rect rect = new Rect(
            screen.x - 80f,
            Screen.height - screen.y - 28f - yLift,
            160f,
            48f
        );
        GUI.Label(rect, "+$" + popupAmount, popupStyle);
        GUI.color = old;
    }

    void BuildStyle()
    {
        popupStyle = new GUIStyle(GUI.skin.label);
        popupStyle.alignment = TextAnchor.MiddleCenter;
        popupStyle.fontSize = 30;
        popupStyle.fontStyle = FontStyle.Bold;
        popupStyle.normal.textColor = new Color(0.18f, 0.95f, 0.42f);
    }
}