using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    const string MasterVolumeKey = "settings.masterVolume";

    public Slider masterVolumeSlider;

    void Start()
    {
        var saved = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        ApplyVolume(saved);

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.SetValueWithoutNotify(saved);
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }
    }

    public void OnMasterVolumeChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    static void ApplyVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
    }
}
