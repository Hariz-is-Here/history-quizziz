using UnityEngine;
using System.Collections;

public class BootFlow : MonoBehaviour
{
    public CanvasGroup splashGroup;
    public float fadeInSeconds = 0.35f;
    public float holdSeconds = 0.8f;
    public float fadeOutSeconds = 0.35f;
    public bool allowSkipWithClick = true;
    public RectTransform scrollingA;
    public RectTransform scrollingB;
    public Vector2 scrollSpeed = new Vector2(-40f, 0f);

    bool _scrollInitialized;
    bool _scrollHorizontal;
    float _scrollLoopSize;

    void Start()
    {
        TryInitScroll();

        if (splashGroup == null)
        {
            GameSession.Instance.LoadScene("MainMenu");
            return;
        }

        StartCoroutine(RunSplash());
    }

    void Update()
    {
        TickScroll();
    }

    IEnumerator RunSplash()
    {
        splashGroup.alpha = 0f;
        yield return FadeTo(1f, fadeInSeconds);
        yield return WaitHold(holdSeconds);
        yield return FadeTo(0f, fadeOutSeconds);
        GameSession.Instance.LoadScene("MainMenu");
    }

    IEnumerator WaitHold(float seconds)
    {
        if (seconds <= 0f) yield break;

        float t = 0f;
        while (t < seconds)
        {
            if (allowSkipWithClick && WasSkipPressed()) yield break;
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    IEnumerator FadeTo(float target, float duration)
    {
        if (duration <= 0f)
        {
            splashGroup.alpha = target;
            yield break;
        }

        float start = splashGroup.alpha;
        float t = 0f;
        while (t < duration)
        {
            if (allowSkipWithClick && WasSkipPressed())
            {
                splashGroup.alpha = target;
                yield break;
            }

            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            splashGroup.alpha = Mathf.Lerp(start, target, p);
            yield return null;
        }

        splashGroup.alpha = target;
    }

    void TryInitScroll()
    {
        if (scrollingA == null || scrollingB == null) return;

        _scrollHorizontal = Mathf.Abs(scrollSpeed.x) >= Mathf.Abs(scrollSpeed.y);
        _scrollLoopSize = GetLoopSize(scrollingA, _scrollHorizontal);
        if (_scrollLoopSize <= 0f) return;

        var offset = _scrollHorizontal ? new Vector2(_scrollLoopSize, 0f) : new Vector2(0f, _scrollLoopSize);
        scrollingB.anchoredPosition = scrollingA.anchoredPosition + offset;
        _scrollInitialized = true;
    }

    void TickScroll()
    {
        if (scrollingA == null || scrollingB == null) return;
        if (!_scrollInitialized) TryInitScroll();
        if (!_scrollInitialized) return;

        var delta = scrollSpeed * Time.unscaledDeltaTime;
        scrollingA.anchoredPosition += delta;
        scrollingB.anchoredPosition += delta;

        float axisSpeed = _scrollHorizontal ? scrollSpeed.x : scrollSpeed.y;
        if (Mathf.Approximately(axisSpeed, 0f)) return;

        float aPos = _scrollHorizontal ? scrollingA.anchoredPosition.x : scrollingA.anchoredPosition.y;
        float bPos = _scrollHorizontal ? scrollingB.anchoredPosition.x : scrollingB.anchoredPosition.y;

        if (axisSpeed < 0f)
        {
            if (aPos <= -_scrollLoopSize)
            {
                MoveTileAfter(scrollingA, scrollingB);
                SwapScrollTiles();
            }
            else if (bPos <= -_scrollLoopSize)
            {
                MoveTileAfter(scrollingB, scrollingA);
            }
        }
        else
        {
            if (aPos >= _scrollLoopSize)
            {
                MoveTileBefore(scrollingA, scrollingB);
                SwapScrollTiles();
            }
            else if (bPos >= _scrollLoopSize)
            {
                MoveTileBefore(scrollingB, scrollingA);
            }
        }
    }

    void MoveTileAfter(RectTransform tile, RectTransform other)
    {
        var offset = _scrollHorizontal ? new Vector2(_scrollLoopSize, 0f) : new Vector2(0f, _scrollLoopSize);
        tile.anchoredPosition = other.anchoredPosition + offset;
    }

    void MoveTileBefore(RectTransform tile, RectTransform other)
    {
        var offset = _scrollHorizontal ? new Vector2(_scrollLoopSize, 0f) : new Vector2(0f, _scrollLoopSize);
        tile.anchoredPosition = other.anchoredPosition - offset;
    }

    void SwapScrollTiles()
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

    static bool WasSkipPressed()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) return true;
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard != null && (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame)) return true;
        return false;
#else
        return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
#endif
    }
}
