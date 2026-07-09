using UnityEngine;

/// <summary>
/// Lightweight sale feedback for WebGL/mobile. Watches existing economy totals
/// and shows a floating income popup in the visible checkout area.
/// </summary>
public class Day01SaleFeedback : MonoBehaviour
{
    public Camera gameplayCamera;
    public Transform checkoutAnchor;
    public Day01ScreenPresentation presentation;
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
        if (popupTimer <= 0f || popupAmount <= 0)
            return;

        if (popupStyle == null)
            BuildStyle();

        Vector2 anchor;
        if (presentation != null)
        {
            Rect play = presentation.PlayRect;
            anchor = new Vector2(
                play.x + play.width * 0.84f,
                play.y + play.height * 0.58f
            );
        }
        else
        {
            if (gameplayCamera == null || checkoutAnchor == null) return;
            Vector3 screen = gameplayCamera.WorldToScreenPoint(checkoutAnchor.position + new Vector3(0f, 2.8f, 0f));
            if (screen.z <= 0f) return;
            anchor = new Vector2(screen.x, Screen.height - screen.y);
        }

        float progress = 1f - Mathf.Clamp01(popupTimer / Mathf.Max(0.01f, popupSeconds));
        float yLift = progress * 52f;
        float alpha = 1f - Mathf.Clamp01((progress - 0.55f) / 0.45f);

        Color old = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, alpha);

        Rect rect = new Rect(anchor.x - 80f, anchor.y - 28f - yLift, 160f, 48f);
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