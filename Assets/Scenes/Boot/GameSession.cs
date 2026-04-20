using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    static GameSession _instance;
    public static GameSession Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindFirstObjectByType<GameSession>();
            if (_instance != null) return _instance;

            var go = new GameObject(nameof(GameSession));
            _instance = go.AddComponent<GameSession>();
            return _instance;
        }
    }

    public TopicData selectedTopic;
    public int currentQuestionIndex;
    public int score;

    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SelectTopic(TopicData topic)
    {
        selectedTopic = topic;
        currentQuestionIndex = 0;
        score = 0;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
