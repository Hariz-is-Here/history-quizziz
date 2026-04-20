using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup transitionGroup;
    public float transitionSeconds = 0.35f;

    bool _isTransitioning;

    void Awake()
    {
        if (transitionGroup != null)
        {
            transitionGroup.alpha = 0f;
            transitionGroup.blocksRaycasts = false;
            transitionGroup.interactable = false;
        }
    }

    public void Play()
    {
        StartTransitionTo("TopicSelect");
    }

    public void OpenTopics()
    {
        StartTransitionTo("TopicSelect");
    }

    public void OpenSettings()
    {
        StartTransitionTo("SettingsMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    void StartTransitionTo(string sceneName)
    {
        if (_isTransitioning) return;
        if (transitionGroup == null)
        {
            GameSession.Instance.LoadScene(sceneName);
            return;
        }

        StartCoroutine(TransitionAndLoad(sceneName));
    }

    IEnumerator TransitionAndLoad(string sceneName)
    {
        _isTransitioning = true;

        transitionGroup.blocksRaycasts = true;
        transitionGroup.interactable = true;

        float start = transitionGroup.alpha;
        float t = 0f;
        float duration = Mathf.Max(0.01f, transitionSeconds);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            transitionGroup.alpha = Mathf.Lerp(start, 1f, p);
            yield return null;
        }

        transitionGroup.alpha = 1f;
        GameSession.Instance.LoadScene(sceneName);
    }
}
