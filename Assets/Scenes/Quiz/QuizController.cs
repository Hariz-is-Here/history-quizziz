using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizController : MonoBehaviour
{
    public TMP_Text questionText;
    public Text questionTextLegacy;
    public TMP_Text[] optionTexts;
    public Text[] optionTextsLegacy;
    public Button[] optionButtons;
    public Button submitButton;
    public Button nextButton;
    public TMP_Text feedbackText;
    public Text feedbackTextLegacy;
    public GameObject explanationPanel;
    public TMP_Text explanationText;
    public Text explanationTextLegacy;
    public TMP_Text progressText;
    public Text progressTextLegacy;

    int selectedIndex = -1;
    int wrongAttemptsThisQuestion;
    public int maxWrongAttemptsPerQuestion = 3;

    QuestionData Current => GameSession.Instance.selectedTopic.questions[GameSession.Instance.currentQuestionIndex];

    void Awake()
    {
        if ((questionText == null && questionTextLegacy == null)
            || (optionTexts == null && optionTextsLegacy == null)
            || optionButtons == null
            || ((optionTexts != null ? optionTexts.Length : 0) != 4 && (optionTextsLegacy != null ? optionTextsLegacy.Length : 0) != 4)
            || optionButtons.Length != 4)
        {
            Debug.LogError("QuizController is missing required UI references. Assign (questionText or questionTextLegacy), (optionTexts[4] or optionTextsLegacy[4]), and optionButtons[4].", this);
            enabled = false;
        }
    }

    void Start()
    {
        LoadQuestion();
    }

    void LoadQuestion()
    {
        if (GameSession.Instance == null || GameSession.Instance.selectedTopic == null || GameSession.Instance.selectedTopic.questions == null || GameSession.Instance.selectedTopic.questions.Length == 0)
        {
            Debug.LogError("No topic/questions selected. Assign TopicData.questions and make sure TopicPicker.OnPick() runs before loading Quiz.", this);
            enabled = false;
            return;
        }

        selectedIndex = -1;
        wrongAttemptsThisQuestion = 0;
        if (nextButton != null) nextButton.interactable = false;
        SetFeedback("");
        if (explanationPanel != null) explanationPanel.SetActive(false);

        SetQuestion(Current.question);
        for (int i = 0; i < 4; i++)
        {
            SetOption(i, Current.options[i]);
            int idx = i;
            optionButtons[i].interactable = true;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnSelect(idx));
        }

        SetProgress((GameSession.Instance.currentQuestionIndex + 1) + "/" + GameSession.Instance.selectedTopic.questions.Length);
    }

    void OnSelect(int index)
    {
        selectedIndex = index;
        if (submitButton == null) OnSubmit();
    }

    public void OnSubmit()
    {
        if (selectedIndex < 0) return;
        bool correct = selectedIndex == Current.correctIndex;
        if (correct)
        {
            SetFeedback("Correct");
            GameSession.Instance.score += 100;
            if (nextButton != null) nextButton.interactable = true;
            DisableOptions();
            if (nextButton == null) OnNext();
        }
        else
        {
            GameSession.Instance.score = Mathf.Max(GameSession.Instance.score - 25, 0);
            wrongAttemptsThisQuestion++;
            if (wrongAttemptsThisQuestion == 1 && !string.IsNullOrWhiteSpace(Current.hint1)) SetFeedback(Current.hint1);
            else if (wrongAttemptsThisQuestion == 2 && !string.IsNullOrWhiteSpace(Current.hint2)) SetFeedback(Current.hint2);
            else SetFeedback("Incorrect");
            if (selectedIndex >= 0 && selectedIndex < optionButtons.Length && optionButtons[selectedIndex] != null)
            {
                optionButtons[selectedIndex].interactable = false;
            }

            selectedIndex = -1;
            if (explanationPanel != null) explanationPanel.SetActive(false);
            if (maxWrongAttemptsPerQuestion > 0 && wrongAttemptsThisQuestion >= maxWrongAttemptsPerQuestion)
            {
                GameSession.Instance.LoadScene("Results");
            }
        }
    }

    void DisableOptions()
    {
        for (int i = 0; i < optionButtons.Length; i++) optionButtons[i].interactable = false;
    }

    public void OnNext()
    {
        GameSession.Instance.currentQuestionIndex++;
        if (GameSession.Instance.currentQuestionIndex >= GameSession.Instance.selectedTopic.questions.Length)
        {
            GameSession.Instance.LoadScene("Results");
        }
        else
        {
            LoadQuestion();
        }
    }

    public void QuitToMainMenu()
    {
        if (GameSession.Instance != null) GameSession.Instance.LoadScene("MainMenu");
    }

    public void QuitToResults()
    {
        if (GameSession.Instance != null) GameSession.Instance.LoadScene("Results");
    }

    void SetQuestion(string value)
    {
        if (questionText != null) questionText.text = value;
        if (questionTextLegacy != null) questionTextLegacy.text = value;
    }

    void SetOption(int index, string value)
    {
        if (optionTexts != null && index >= 0 && index < optionTexts.Length && optionTexts[index] != null) optionTexts[index].text = value;
        if (optionTextsLegacy != null && index >= 0 && index < optionTextsLegacy.Length && optionTextsLegacy[index] != null) optionTextsLegacy[index].text = value;
    }

    void SetFeedback(string value)
    {
        if (feedbackText != null) feedbackText.text = value;
        if (feedbackTextLegacy != null) feedbackTextLegacy.text = value;
    }

    void SetProgress(string value)
    {
        if (progressText != null) progressText.text = value;
        if (progressTextLegacy != null) progressTextLegacy.text = value;
    }

    void SetExplanation(string value)
    {
        if (explanationText != null) explanationText.text = value;
        if (explanationTextLegacy != null) explanationTextLegacy.text = value;
    }
}
