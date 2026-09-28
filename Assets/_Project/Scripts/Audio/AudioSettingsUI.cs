using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void Start()
    {
        musicToggle.isOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        sfxToggle.isOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
    }

    public void OnMusicChanged(bool isOn)
    {
        PlayerPrefs.SetInt("MusicOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnSFXChanged(bool isOn)
    {
        PlayerPrefs.SetInt("SFXOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}