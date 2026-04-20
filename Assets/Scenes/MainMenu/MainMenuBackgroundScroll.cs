using UnityEngine;

public class MainMenuBackgroundScroll : MonoBehaviour
{
    public RectTransform scrollingA;
    public RectTransform scrollingB;
    public Vector2 scrollSpeed = new Vector2(-20f, 0f);

    bool _initialized;
    bool _horizontal;
    float _loopSize;

    void Start()
    {
        TryInit();
    }

    void Update()
    {
        Tick();
    }

    void TryInit()
    {
        if (scrollingA == null || scrollingB == null) return;

        _horizontal = Mathf.Abs(scrollSpeed.x) >= Mathf.Abs(scrollSpeed.y);
        _loopSize = GetLoopSize(scrollingA, _horizontal);
        if (_loopSize <= 0f) return;

        var offset = _horizontal ? new Vector2(_loopSize, 0f) : new Vector2(0f, _loopSize);
        scrollingB.anchoredPosition = scrollingA.anchoredPosition + offset;
        _initialized = true;
    }

    void Tick()
    {
        if (scrollingA == null || scrollingB == null) return;
        if (!_initialized) TryInit();
        if (!_initialized) return;

        var delta = scrollSpeed * Time.unscaledDeltaTime;
        scrollingA.anchoredPosition += delta;
        scrollingB.anchoredPosition += delta;

        float axisSpeed = _horizontal ? scrollSpeed.x : scrollSpeed.y;
        if (Mathf.Approximately(axisSpeed, 0f)) return;

        float aPos = _horizontal ? scrollingA.anchoredPosition.x : scrollingA.anchoredPosition.y;
        float bPos = _horizontal ? scrollingB.anchoredPosition.x : scrollingB.anchoredPosition.y;

        if (axisSpeed < 0f)
        {
            if (aPos <= -_loopSize)
            {
                MoveTileAfter(scrollingA, scrollingB);
                SwapTiles();
            }
            else if (bPos <= -_loopSize)
            {
                MoveTileAfter(scrollingB, scrollingA);
            }
        }
        else
        {
            if (aPos >= _loopSize)
            {
                MoveTileBefore(scrollingA, scrollingB);
                SwapTiles();
            }
            else if (bPos >= _loopSize)
            {
                MoveTileBefore(scrollingB, scrollingA);
            }
        }
    }

    void MoveTileAfter(RectTransform tile, RectTransform other)
    {
        var offset = _horizontal ? new Vector2(_loopSize, 0f) : new Vector2(0f, _loopSize);
        tile.anchoredPosition = other.anchoredPosition + offset;
    }

    void MoveTileBefore(RectTransform tile, RectTransform other)
    {
        var offset = _horizontal ? new Vector2(_loopSize, 0f) : new Vector2(0f, _loopSize);
        tile.anchoredPosition = other.anchoredPosition - offset;
    }

    void SwapTiles()
    {
        var tmp = scrollingA;
        scrollingA = scrollingB;
        scrollingB = tmp;
    }

    static float GetLoopSize(RectTransform rt, bool horizontal)
    {
        if (rt == null) return 0f;
        var size = rt.rect.size;
        return horizontal ? size.x : size.y;
    }
}
