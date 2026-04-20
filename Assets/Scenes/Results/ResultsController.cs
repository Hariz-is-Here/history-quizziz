using UnityEngine;
using TMPro;

public class ResultsController : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
    {
        scoreText.text = "Score: " + GameSession.Instance.score;
    }

    public void Retry()
    {
        GameSession.Instance.LoadScene("Learning");
    }

    public void NextTopic()
    {
        GameSession.Instance.LoadScene("TopicSelect");
    }

    public void MainMenu()
    {
        GameSession.Instance.LoadScene("MainMenu");
    }
}