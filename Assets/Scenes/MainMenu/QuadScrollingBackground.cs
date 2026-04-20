using UnityEngine;

public class QuadScrollingBackground : MonoBehaviour
{
    public float speed = 0.2f;
    public Vector2 direction = Vector2.right;
    public Renderer targetRenderer;
    public Material materialOverride;
    public Texture textureOverride;
    public Vector2 tiling = new Vector2(2f, 1f);
    public bool applyTilingOnEnable = true;
    public bool wrapOffsetTo01 = true;
    public bool forceTextureWrapRepeat = true;
    public bool tryForceTilingCompatibleShader = true;
    public bool persistAcrossScenes = false;

    Material _material;
    int _texPropertyId;
    static QuadScrollingBackground _instance;

    void Awake()
    {
        if (persistAcrossScenes)
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
        if (targetRenderer == null) return;

        if (materialOverride != null) targetRenderer.sharedMaterial = materialOverride;

        _material = targetRenderer.material;
        if (_material == null) return;

        _texPropertyId = _material.HasProperty("_BaseMap") ? Shader.PropertyToID("_BaseMap") : Shader.PropertyToID("_MainTex");
        ApplyTextureIfNeeded();
        TryMakeMaterialTilingCompatible();
    }

    void OnEnable()
    {
        if (_material == null) return;
        ApplyTextureIfNeeded();
        TryMakeMaterialTilingCompatible();
        ApplyTilingIfNeeded();
    }

    void Update()
    {
        if (_material == null) return;
        if (direction.sqrMagnitude <= 0f) return;

        var offset = _material.GetTextureOffset(_texPropertyId);
        offset += direction.normalized * (speed * Time.deltaTime);

        if (wrapOffsetTo01)
        {
            offset.x = Mathf.Repeat(offset.x, 1f);
            offset.y = Mathf.Repeat(offset.y, 1f);
        }

        _material.SetTextureOffset(_texPropertyId, offset);
    }

    void ApplyTilingIfNeeded()
    {
        if (!applyTilingOnEnable) return;
        if (tiling.x <= 0f || tiling.y <= 0f) return;
        _material.SetTextureScale(_texPropertyId, tiling);
    }

    void ApplyTextureIfNeeded()
    {
        if (textureOverride == null) return;
        if (_material == null) return;
        _material.SetTexture(_texPropertyId, textureOverride);
    }

    void TryMakeMaterialTilingCompatible()
    {
        if (_material == null) return;

        if (tryForceTilingCompatibleShader)
        {
            var shaderName = _material.shader != null ? _material.shader.name : string.Empty;
            var looksLikeSpriteShader = shaderName.Contains("Sprite") || shaderName.Contains("2D");
            if (looksLikeSpriteShader)
            {
                var urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");
                var builtinUnlit = Shader.Find("Unlit/Texture");
                var chosen = urpUnlit != null ? urpUnlit : builtinUnlit;
                if (chosen != null) _material.shader = chosen;
            }
        }

        if (!forceTextureWrapRepeat) return;

        var tex = _material.GetTexture(_texPropertyId);
        if (tex == null) return;

        if (tex.wrapMode != TextureWrapMode.Repeat) tex.wrapMode = TextureWrapMode.Repeat;
        if (tex.wrapModeU != TextureWrapMode.Repeat) tex.wrapModeU = TextureWrapMode.Repeat;
        if (tex.wrapModeV != TextureWrapMode.Repeat) tex.wrapModeV = TextureWrapMode.Repeat;
    }
}
