using UnityEngine;

[CreateAssetMenu(menuName = "HistoryQuizziz/Question")]
public class QuestionData : ScriptableObject
{
    [TextArea] public string question;
    public string[] options = new string[4];
    public int correctIndex;
    [TextArea] public string hint1;
    [TextArea] public string hint2;
    [TextArea] public string explanation;
}
