using UnityEngine;

public class Day01HUD : MonoBehaviour
{
    public MissionSystem mission;
    public Day01CustomerDirector director;
    public Day01TapFlowController tapFlow;

    private GUIStyle panelStyle;
    private GUIStyle titleStyle;
    private GUIStyle labelStyle;
    private GUIStyle moneyStyle;
    private GUIStyle smallStyle;
    private GUIStyle centerTitleStyle;
    private GUIStyle centerLabelStyle;

    private Texture2D panelTexture;
    private Texture2D accentTexture;
    private Texture2D progressBackTexture;
    private Texture2D progressFillTexture;

    private Texture2D missionPanelArt;
    private Texture2D coinArt;
    private Texture2D coinStackArt;
    private Texture2D playerPortraitArt;
    private Texture2D productArt;
    private Texture2D shelfArt;

    void Awake()
    {
        BuildStyles();
        LoadDesignedArt();
    }

    void OnDestroy()
    {
        Destroy(panelTexture);
        Destroy(accentTexture);
        Destroy(progressBackTexture);
        Destroy(progressFillTexture);
    }

    void OnGUI()
    {
        if (panelStyle == null)
            BuildStyles();

        if (productArt == null && coinArt == null)
            LoadDesignedArt();

        float scale = Mathf.Clamp(Screen.height / 900f, 0.72f, 1.25f);
        Matrix4x4 previous = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);

        float width = Screen.width / scale;
        float height = Screen.height / scale;

        DrawTopHUD(width);
        DrawMissionCard();
        DrawTapHint(height);
        DrawCompletionBanner(width);

        GUI.matrix = previous;
    }

    private void DrawTopHUD(float screenWidth)
    {
        GUI.Box(new Rect(24f, 20f, 210f, 50f), GUIContent.none, panelStyle);
        GUI.Label(new Rect(38f, 27f, 182f, 30f), "☀  MORNING SHIFT", titleStyle);

        if (playerPortraitArt != null)
        {
            GUI.Box(new Rect(244f, 20f, 50f, 50f), GUIContent.none, panelStyle);
            GUI.DrawTexture(new Rect(250f, 24f, 38f, 42f), playerPortraitArt, ScaleMode.ScaleToFit, true);
        }

        int money = EconomySystem.Instance != null ? EconomySystem.Instance.money : 0;
        GUI.Box(new Rect(screenWidth - 190f, 20f, 166f, 50f), GUIContent.none, panelStyle);

        if (coinArt != null)
        {
            GUI.DrawTexture(new Rect(screenWidth - 178f, 29f, 32f, 32f), coinArt, ScaleMode.ScaleToFit, true);
            GUI.Label(new Rect(screenWidth - 140f, 29f, 104f, 30f), money.ToString(), moneyStyle);
        }
        else
        {
            GUI.Label(new Rect(screenWidth - 174f, 29f, 140f, 30f), "$ " + money, moneyStyle);
        }
    }

    private void DrawMissionCard()
    {
        Rect card = new Rect(24f, 82f, 430f, 150f);

        if (missionPanelArt != null)
            GUI.DrawTexture(card, missionPanelArt, ScaleMode.StretchToFill, true);
        else
            GUI.Box(card, GUIContent.none, panelStyle);

        Rect artFrame = new Rect(38f, 96f, 88f, 88f);
        GUI.Box(artFrame, GUIContent.none, panelStyle);

        Texture2D preview = productArt != null ? productArt : shelfArt;
        if (preview != null)
            GUI.DrawTexture(new Rect(44f, 102f, 76f, 76f), preview, ScaleMode.ScaleToFit, true);

        GUI.Label(new Rect(140f, 96f, 270f, 30f), "RESTOCK DRINKS", titleStyle);
        GUI.Label(new Rect(140f, 128f, 270f, 24f), "Tap boxes → cart → shelf", labelStyle);

        int current = mission != null ? mission.currentAmount : 0;
        int target = mission != null ? Mathf.Max(1, mission.targetAmount) : 6;
        float progress = Mathf.Clamp01((float)current / target);

        GUI.DrawTexture(new Rect(140f, 164f, 210f, 14f), progressBackTexture);
        GUI.DrawTexture(new Rect(140f, 164f, 210f * progress, 14f), progressFillTexture);
        GUI.Label(new Rect(360f, 156f, 58f, 28f), current + "/" + target, smallStyle);

        if (mission != null && mission.reward > 0)
            GUI.Label(new Rect(140f, 188f, 250f, 22f), "Reward  +$" + mission.reward, smallStyle);
    }

    private void DrawTapHint(float screenHeight)
    {
        string hint = tapFlow != null ? tapFlow.CurrentHint : "Tap objects to interact";
        GUI.Box(new Rect(24f, screenHeight - 70f, 520f, 46f), GUIContent.none, panelStyle);
        GUI.DrawTexture(new Rect(24f, screenHeight - 70f, 7f, 46f), accentTexture);
        GUI.Label(new Rect(42f, screenHeight - 61f, 490f, 28f), hint, smallStyle);
    }

    private void DrawCompletionBanner(float screenWidth)
    {
        if (mission == null || !mission.completed) return;

        bool finished = director != null && director.IsWaveFinished();
        float bannerWidth = finished && coinStackArt != null ? 500f : 440f;
        float x = screenWidth * 0.5f - bannerWidth * 0.5f;

        GUI.Box(new Rect(x, 24f, bannerWidth, 92f), GUIContent.none, panelStyle);
        GUI.DrawTexture(new Rect(x, 24f, 8f, 92f), accentTexture);

        float textOffset = 28f;
        if (finished && coinStackArt != null)
        {
            GUI.DrawTexture(new Rect(x + 22f, 31f, 78f, 72f), coinStackArt, ScaleMode.ScaleToFit, true);
            textOffset = 104f;
        }

        if (finished)
        {
            GUI.Label(new Rect(x + textOffset, 36f, bannerWidth - textOffset - 22f, 28f), "DAY COMPLETE", centerTitleStyle);
            GUI.Label(new Rect(x + textOffset, 70f, bannerWidth - textOffset - 22f, 24f), "All customers checked out", centerLabelStyle);
            return;
        }

        int served = director != null ? director.GetCompletedCustomers() : 0;
        int total = director != null ? director.totalCustomers : 0;
        GUI.Label(new Rect(x + 28f, 36f, bannerWidth - 50f, 28f), "STORE OPEN", centerTitleStyle);
        GUI.Label(new Rect(x + 28f, 70f, bannerWidth - 50f, 24f), "Checkout progress  " + served + "/" + total, centerLabelStyle);
    }

    private void LoadDesignedArt()
    {
        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null) return;

        missionPanelArt = DesignedArtIntegration.GetTexture(catalog.missionPanel);
        coinArt = DesignedArtIntegration.GetTexture(catalog.coinIcon);
        coinStackArt = DesignedArtIntegration.GetTexture(catalog.coinStack);
        playerPortraitArt = DesignedArtIntegration.GetTexture(catalog.playerIdle != null ? catalog.playerIdle : catalog.player);
        productArt = DesignedArtIntegration.GetTexture(catalog.colaBox != null ? catalog.colaBox : catalog.drinkBox);
        shelfArt = DesignedArtIntegration.GetTexture(catalog.drinkShelf);
    }

    private void BuildStyles()
    {
        panelTexture = SolidTexture(new Color(0.06f, 0.09f, 0.12f, 0.90f));
        accentTexture = SolidTexture(new Color(0.98f, 0.68f, 0.12f, 1f));
        progressBackTexture = SolidTexture(new Color(1f, 1f, 1f, 0.16f));
        progressFillTexture = SolidTexture(new Color(0.18f, 0.75f, 0.36f, 1f));

        panelStyle = new GUIStyle(GUI.skin.box);
        panelStyle.normal.background = panelTexture;
        panelStyle.border = new RectOffset(10, 10, 10, 10);

        titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 19;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;

        labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 17;
        labelStyle.normal.textColor = new Color(0.9f, 0.93f, 0.95f);

        moneyStyle = new GUIStyle(titleStyle);
        moneyStyle.alignment = TextAnchor.MiddleRight;

        smallStyle = new GUIStyle(GUI.skin.label);
        smallStyle.fontSize = 15;
        smallStyle.normal.textColor = new Color(0.84f, 0.89f, 0.92f);

        centerTitleStyle = new GUIStyle(titleStyle);
        centerTitleStyle.alignment = TextAnchor.MiddleCenter;
        centerTitleStyle.fontSize = 21;

        centerLabelStyle = new GUIStyle(labelStyle);
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;
        centerLabelStyle.fontSize = 15;
    }

    private Texture2D SolidTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}