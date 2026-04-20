using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "HistoryQuizziz/Topic")]
public class TopicData : ScriptableObject
{
    public string id;
    public string title;
    [TextArea] public string description;
    [TextArea] public string learningObjective;
    [TextArea] public string keyPoints;
    public VideoClip videoClip;
    public AudioClip audioClip;
    public float minLearningSecondsOverride = -1f;
    public QuestionData[] questions;
}
