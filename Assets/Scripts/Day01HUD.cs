using UnityEngine;

public class Day01HUD : MonoBehaviour
{
    public MissionSystem mission;
    public Day01CustomerDirector director;

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

        float scale = Mathf.Clamp(Screen.height / 900f, 0.75f, 1.25f);
        Matrix4x4 previous = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);

        float width = Screen.width / scale;
        float height = Screen.height / scale;

        DrawTopHUD(width);
        DrawMissionCard();
        DrawControlHint(height);
        DrawCompletionBanner(width);

        GUI.matrix = previous;
    }

    private void DrawTopHUD(float screenWidth)
    {
        GUI.Box(new Rect(24f, 20f, 150f, 42f), GUIContent.none, panelStyle);
        GUI.Label(new Rect(38f, 27f, 130f, 28f), "DAY 1", titleStyle);

        if (playerPortraitArt != null)
        {
            GUI.Box(new Rect(184f, 20f, 42f, 42f), GUIContent.none, panelStyle);
            GUI.DrawTexture(
                new Rect(190f, 23f, 30f, 36f),
                playerPortraitArt,
                ScaleMode.ScaleToFit,
                true
            );
        }

        int money = EconomySystem.Instance != null ? EconomySystem.Instance.money : 0;
        GUI.Box(new Rect(screenWidth - 190f, 20f, 166f, 42f), GUIContent.none, panelStyle);

        if (coinArt != null)
        {
            GUI.DrawTexture(
                new Rect(screenWidth - 178f, 26f, 30f, 30f),
                coinArt,
                ScaleMode.ScaleToFit,
                true
            );
            GUI.Label(new Rect(screenWidth - 146f, 27f, 110f, 28f), money.ToString(), moneyStyle);
        }
        else
        {
            GUI.Label(new Rect(screenWidth - 174f, 27f, 140f, 28f), "$ " + money, moneyStyle);
        }
    }

    private void DrawMissionCard()
    {
        Rect card = new Rect(24f, 78f, 420f, 142f);

        if (missionPanelArt != null)
            GUI.DrawTexture(card, missionPanelArt, ScaleMode.StretchToFill, true);
        else
            GUI.Box(card, GUIContent.none, panelStyle);

        Rect artFrame = new Rect(38f, 91f, 86f, 86f);
        GUI.Box(artFrame, GUIContent.none, panelStyle);

        Texture2D preview = productArt != null ? productArt : shelfArt;
        if (preview != null)
        {
            GUI.DrawTexture(
                new Rect(43f, 96f, 76f, 76f),
                preview,
                ScaleMode.ScaleToFit,
                true
            );
        }

        GUI.Label(new Rect(138f, 91f, 260f, 28f), "MORNING SHIFT", titleStyle);
        GUI.Label(new Rect(138f, 123f, 260f, 24f), "Restock the drink shelf", labelStyle);

        int current = mission != null ? mission.currentAmount : 0;
        int target = mission != null ? Mathf.Max(1, mission.targetAmount) : 10;
        float progress = Mathf.Clamp01((float)current / target);

        GUI.DrawTexture(new Rect(138f, 158f, 205f, 14f), progressBackTexture);
        GUI.DrawTexture(new Rect(138f, 158f, 205f * progress, 14f), progressFillTexture);
        GUI.Label(new Rect(350f, 150f, 52f, 28f), current + "/" + target, smallStyle);

        if (mission != null && mission.reward > 0)
            GUI.Label(new Rect(138f, 181f, 250f, 20f), "Reward  +$" + mission.reward, smallStyle);
    }

    private void DrawControlHint(float screenHeight)
    {
        GUI.Box(new Rect(24f, screenHeight - 64f, 350f, 40f), GUIContent.none, panelStyle);
        GUI.Label(new Rect(40f, screenHeight - 56f, 330f, 26f), "WASD Move   •   E Interact   •   Shift Run", smallStyle);
    }

    private void DrawCompletionBanner(float screenWidth)
    {
        if (mission == null || !mission.completed) return;

        bool finished = director != null && director.IsWaveFinished();
        float bannerWidth = 420f;
        float x = screenWidth * 0.5f - bannerWidth * 0.5f;

        GUI.Box(new Rect(x, 24f, bannerWidth, 86f), GUIContent.none, panelStyle);
        GUI.DrawTexture(new Rect(x, 24f, 8f, 86f), accentTexture);

        if (finished)
        {
            GUI.Label(new Rect(x + 28f, 36f, bannerWidth - 50f, 28f), "DAY COMPLETE", centerTitleStyle);
            GUI.Label(new Rect(x + 28f, 68f, bannerWidth - 50f, 24f), "All customers checked out", centerLabelStyle);
            return;
        }

        int served = director != null ? director.GetCompletedCustomers() : 0;
        int total = director != null ? director.totalCustomers : 0;
        GUI.Label(new Rect(x + 28f, 36f, bannerWidth - 50f, 28f), "STORE OPEN", centerTitleStyle);
        GUI.Label(new Rect(x + 28f, 68f, bannerWidth - 50f, 24f), "Checkout progress  " + served + "/" + total, centerLabelStyle);
    }

    private void LoadDesignedArt()
    {
        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null) return;

        missionPanelArt = DesignedArtIntegration.GetTexture(catalog.missionPanel);
        coinArt = DesignedArtIntegration.GetTexture(catalog.coinIcon);
        playerPortraitArt = DesignedArtIntegration.GetTexture(
            catalog.playerIdle != null ? catalog.playerIdle : catalog.player
        );
        productArt = DesignedArtIntegration.GetTexture(
            catalog.colaBox != null ? catalog.colaBox : catalog.drinkBox
        );
        shelfArt = DesignedArtIntegration.GetTexture(catalog.drinkShelf);
    }

    private void BuildStyles()
    {
        panelTexture = SolidTexture(new Color(0.06f, 0.09f, 0.12f, 0.88f));
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
        smallStyle.fontSize = 14;
        smallStyle.normal.textColor = new Color(0.78f, 0.84f, 0.88f);

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
