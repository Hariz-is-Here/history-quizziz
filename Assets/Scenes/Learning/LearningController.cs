using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class LearningController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public Button startQuizButton;
    public float minSeconds = 10f;
    public bool requireFullMediaToStartQuiz;
    public TMP_Text topicTitleText;
    public TMP_Text objectiveText;
    public TMP_Text keyPointsText;
    public Slider progressSlider;
    public TMP_Text progressText;
    public Button playPauseButton;
    public TMP_Text playPauseButtonText;
    public Button replayButton;

    float elapsed;
    bool unlocked;
    bool hadMedia;
    bool hasVideo;
    bool hasAudio;
    bool videoFinished;
    bool audioFinished;

    void Start()
    {
        if (GameSession.Instance == null || GameSession.Instance.selectedTopic == null)
        {
            Debug.LogError("No topic selected. Go through TopicSelect before Learning.", this);
            if (startQuizButton != null) startQuizButton.interactable = false;
            enabled = false;
            return;
        }

        var topic = GameSession.Instance.selectedTopic;
        if (topicTitleText != null) topicTitleText.text = topic.title;
        if (objectiveText != null) objectiveText.text = topic.learningObjective;
        if (keyPointsText != null) keyPointsText.text = topic.keyPoints;

        hadMedia = false;
        hasVideo = false;
        hasAudio = false;
        videoFinished = false;
        audioFinished = false;

        if (videoPlayer != null && topic.videoClip != null)
        {
            hadMedia = true;
            hasVideo = true;
            videoPlayer.clip = topic.videoClip;
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Prepare();
            videoPlayer.Play();
        }
        if (audioSource != null && topic.audioClip != null)
        {
            hadMedia = true;
            hasAudio = true;
            audioSource.clip = topic.audioClip;
            audioSource.Play();
        }

        if (startQuizButton != null) startQuizButton.interactable = false;
        if (playPauseButton != null)
        {
            playPauseButton.onClick.RemoveAllListeners();
            playPauseButton.onClick.AddListener(TogglePlayPause);
        }
        if (replayButton != null)
        {
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(Replay);
        }
        RefreshPlayPauseLabel();
        if (CanUnlock()) Unlock();
    }

    void Update()
    {
        if (unlocked) return;

        elapsed += Time.deltaTime;
        UpdateMediaFinished();
        UpdateProgressUI();

        if (CanUnlock()) Unlock();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true;
        Unlock();
    }

    void Unlock()
    {
        unlocked = true;
        if (startQuizButton != null) startQuizButton.interactable = true;
    }

    public void StartQuiz()
    {
        GameSession.Instance.currentQuestionIndex = 0;
        GameSession.Instance.LoadScene("Quiz");
    }

    public void SkipLearning()
    {
        if (GameSession.Instance == null || GameSession.Instance.selectedTopic == null)
        {
            if (GameSession.Instance != null) GameSession.Instance.LoadScene("TopicSelect");
            return;
        }

        StartQuiz();
    }

    void TogglePlayPause()
    {
        bool isPlaying = (hasVideo && videoPlayer != null && videoPlayer.isPlaying) || (hasAudio && audioSource != null && audioSource.isPlaying);

        if (isPlaying)
        {
            if (hasVideo && videoPlayer != null) videoPlayer.Pause();
            if (hasAudio && audioSource != null) audioSource.Pause();
        }
        else
        {
            if (hasVideo && videoPlayer != null) videoPlayer.Play();
            if (hasAudio && audioSource != null) audioSource.Play();
        }

        RefreshPlayPauseLabel();
    }

    void Replay()
    {
        elapsed = 0f;
        videoFinished = false;
        audioFinished = false;

        if (hasVideo && videoPlayer != null && videoPlayer.clip != null)
        {
            videoPlayer.time = 0;
            videoPlayer.Play();
        }
        if (hasAudio && audioSource != null && audioSource.clip != null)
        {
            audioSource.time = 0;
            audioSource.Play();
        }

        RefreshPlayPauseLabel();
        if (startQuizButton != null) startQuizButton.interactable = false;
        unlocked = false;
    }

    bool CanUnlock()
    {
        var topic = GameSession.Instance.selectedTopic;
        var requiredSeconds = topic.minLearningSecondsOverride > 0f ? topic.minLearningSecondsOverride : minSeconds;
        var enoughTime = requiredSeconds <= 0f || elapsed >= requiredSeconds;

        if (!requireFullMediaToStartQuiz) return enoughTime;
        if (!hadMedia) return enoughTime;
        bool mediaDone = (!hasVideo || videoFinished) && (!hasAudio || audioFinished);
        return enoughTime && mediaDone;
    }

    void UpdateMediaFinished()
    {
        if (hasVideo && !videoFinished && videoPlayer != null && videoPlayer.clip != null)
        {
            if (!videoPlayer.isPlaying && videoPlayer.isPrepared && videoPlayer.time > 0 && videoPlayer.time >= videoPlayer.length) videoFinished = true;
        }

        if (hasAudio && !audioFinished && audioSource != null && audioSource.clip != null)
        {
            if (!audioSource.isPlaying && audioSource.time > 0 && audioSource.time >= audioSource.clip.length) audioFinished = true;
        }
    }

    void UpdateProgressUI()
    {
        var topic = GameSession.Instance.selectedTopic;
        var requiredSeconds = topic.minLearningSecondsOverride > 0f ? topic.minLearningSecondsOverride : minSeconds;

        float current = elapsed;
        float total = requiredSeconds > 0f ? requiredSeconds : 0f;

        if (hasVideo && videoPlayer != null && videoPlayer.clip != null && videoPlayer.isPrepared)
        {
            current = (float)videoPlayer.time;
            total = (float)videoPlayer.length;
        }
        else if (hasAudio && audioSource != null && audioSource.clip != null)
        {
            current = audioSource.time;
            total = audioSource.clip.length;
        }

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = total > 0f ? total : 1f;
            progressSlider.value = Mathf.Clamp(current, 0f, progressSlider.maxValue);
        }

        if (progressText != null)
        {
            if (total > 0f) progressText.text = $"{Mathf.FloorToInt(current)}/{Mathf.CeilToInt(total)}s";
            else progressText.text = "";
        }
    }

    void RefreshPlayPauseLabel()
    {
        if (playPauseButtonText == null) return;

        if (hasVideo && videoPlayer != null && videoPlayer.clip != null)
        {
            bool isPlaying = (hasVideo && videoPlayer.isPlaying) || (hasAudio && audioSource != null && audioSource.isPlaying);
            playPauseButtonText.text = isPlaying ? "Pause" : "Play";
            return;
        }

        if (hasAudio && audioSource != null && audioSource.clip != null)
        {
            playPauseButtonText.text = audioSource.isPlaying ? "Pause" : "Play";
            return;
        }

        playPauseButtonText.text = "Play";
    }
}
