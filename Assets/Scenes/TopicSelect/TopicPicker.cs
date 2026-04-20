using UnityEngine;

public class TopicPicker : MonoBehaviour
{
    public TopicData topic;
    public void OnPick()
    {
        GameSession.Instance.SelectTopic(topic);
        GameSession.Instance.LoadScene("Learning");
    }

    public void BackToMainMenu()
    {
        GameSession.Instance.LoadScene("MainMenu");
    }
}
