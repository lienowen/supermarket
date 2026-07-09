using UnityEngine;
using UnityEngine.Rendering;

public class DesignedCharacterVisual : MonoBehaviour
{
    public enum CharacterRole
    {
        Player,
        Customer
    }

    [Header("Character")]
    public CharacterRole role = CharacterRole.Player;
    public int customerIndex;
    public float worldHeight = 2.35f;
    public float groundOffset = 0.02f;

    [Header("Motion")]
    public float walkBobAmplitude = 0.045f;
    public float walkBobSpeed = 9f;
    public float movementThreshold = 0.001f;

    private const string VisualRootName = "DesignedCharacterVisual";
    private const string ProceduralRootName = "ProceduralVisual";

    private Transform visualRoot;
    private Renderer visualRenderer;
    private Material visualMaterial;
    private CarrySystem carrySystem;
    private Sprite idleSprite;
    private Sprite carrySprite;
    private Sprite currentSprite;
    private Camera cachedCamera;
    private Vector3 previousPosition;
    private float baseLocalY;
    private float bobTime;

    public static bool ApplyPlayer(GameObject target)
    {
        if (target == null) return false;

        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null) return false;

        Sprite sprite = catalog.playerIdle != null ? catalog.playerIdle : catalog.player;
        if (sprite == null) return false;

        DesignedCharacterVisual visual = target.GetComponent<DesignedCharacterVisual>();
        if (visual == null)
            visual = target.AddComponent<DesignedCharacterVisual>();

        visual.role = CharacterRole.Player;
        visual.worldHeight = 2.35f;
        visual.ConfigureNow();
        return true;
    }

    public static bool ApplyCustomer(GameObject target, int index)
    {
        if (target == null) return false;

        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null || catalog.GetCustomer(index) == null) return false;

        DesignedCharacterVisual visual = target.GetComponent<DesignedCharacterVisual>();
        if (visual == null)
            visual = target.AddComponent<DesignedCharacterVisual>();

        visual.role = CharacterRole.Customer;
        visual.customerIndex = index;
        visual.worldHeight = 2.2f;
        visual.ConfigureNow();
        return true;
    }

    void Awake()
    {
        previousPosition = transform.position;
    }

    void Start()
    {
        ConfigureNow();
    }

    void Update()
    {
        if (visualRenderer == null) return;

        if (role == CharacterRole.Player)
            UpdatePlayerState();

        UpdateWalkBob();
    }

    void LateUpdate()
    {
        FaceCamera();
    }

    void OnDestroy()
    {
        if (visualMaterial != null)
            Destroy(visualMaterial);
    }

    public void ConfigureNow()
    {
        ArtRuntimeCatalog catalog = DesignedArtIntegration.Catalog;
        if (catalog == null) return;

        if (role == CharacterRole.Player)
        {
            idleSprite = catalog.playerIdle != null ? catalog.playerIdle : catalog.player;
            carrySprite = catalog.playerCarry;
            carrySystem = GetComponent<CarrySystem>();
        }
        else
        {
            idleSprite = catalog.GetCustomer(customerIndex);
            carrySprite = null;
        }

        if (idleSprite == null) return;

        EnsureVisualRoot();
        HideProceduralVisual();
        ApplySprite(idleSprite);
        previousPosition = transform.position;
    }

    private void EnsureVisualRoot()
    {
        Transform existing = transform.Find(VisualRootName);
        if (existing != null)
        {
            visualRoot = existing;
            visualRenderer = existing.GetComponent<Renderer>();
            if (visualRenderer != null)
                return;

            Destroy(existing.gameObject);
        }

        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = VisualRootName;
        quad.transform.SetParent(transform, false);
        visualRoot = quad.transform;

        Collider quadCollider = quad.GetComponent<Collider>();
        if (quadCollider != null)
            Destroy(quadCollider);

        visualRenderer = quad.GetComponent<Renderer>();
        if (visualRenderer != null)
        {
            visualRenderer.shadowCastingMode = ShadowCastingMode.Off;
            visualRenderer.receiveShadows = false;
            visualRenderer.sortingOrder = 40;
        }
    }

    private void ApplySprite(Sprite sprite)
    {
        if (sprite == null || visualRoot == null || visualRenderer == null) return;
        if (currentSprite == sprite && visualMaterial != null) return;

        currentSprite = sprite;

        if (visualMaterial != null)
            Destroy(visualMaterial);

        Shader shader = Shader.Find("Supermarket/CheckerboardCutout");
        if (shader == null) shader = Shader.Find("Unlit/Transparent");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Standard");

        visualMaterial = new Material(shader);
        visualMaterial.name = "Character_" + sprite.name;
        visualMaterial.mainTexture = sprite.texture;
        visualMaterial.color = Color.white;

        if (visualMaterial.HasProperty("_Cutoff"))
            visualMaterial.SetFloat("_Cutoff", 0.06f);
        if (visualMaterial.HasProperty("_NeutralTolerance"))
            visualMaterial.SetFloat("_NeutralTolerance", 0.045f);
        if (visualMaterial.HasProperty("_CheckerTolerance"))
            visualMaterial.SetFloat("_CheckerTolerance", 0.025f);

        visualRenderer.sharedMaterial = visualMaterial;

        float aspect = sprite.rect.height > 0f
            ? sprite.rect.width / sprite.rect.height
            : 1f;
        float width = worldHeight * Mathf.Max(0.25f, aspect);

        visualRoot.localScale = new Vector3(width, worldHeight, 1f);
        baseLocalY = groundOffset + worldHeight * 0.5f;
        visualRoot.localPosition = new Vector3(0f, baseLocalY, 0f);
    }

    private void UpdatePlayerState()
    {
        if (carrySystem == null)
            carrySystem = GetComponent<CarrySystem>();

        bool carrying = carrySystem != null && carrySystem.HasItem();
        Sprite desired = carrying && carrySprite != null ? carrySprite : idleSprite;
        if (desired != null && desired != currentSprite)
            ApplySprite(desired);
    }

    private void UpdateWalkBob()
    {
        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - previousPosition;
        delta.y = 0f;

        bool moving = delta.sqrMagnitude > movementThreshold * movementThreshold;
        previousPosition = currentPosition;

        if (moving)
        {
            bobTime += Time.deltaTime * walkBobSpeed;
            float bob = Mathf.Abs(Mathf.Sin(bobTime)) * walkBobAmplitude;
            visualRoot.localPosition = new Vector3(0f, baseLocalY + bob, 0f);
        }
        else
        {
            bobTime = 0f;
            visualRoot.localPosition = Vector3.Lerp(
                visualRoot.localPosition,
                new Vector3(0f, baseLocalY, 0f),
                1f - Mathf.Exp(-12f * Time.deltaTime)
            );
        }
    }

    private void FaceCamera()
    {
        if (visualRoot == null) return;

        if (cachedCamera == null)
            cachedCamera = Camera.main;
        if (cachedCamera == null) return;

        Vector3 direction = cachedCamera.transform.position - visualRoot.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        visualRoot.rotation = Quaternion.LookRotation(-direction.normalized, Vector3.up);
    }

    private void HideProceduralVisual()
    {
        Transform procedural = transform.Find(ProceduralRootName);
        if (procedural != null)
            procedural.gameObject.SetActive(false);
    }
}
