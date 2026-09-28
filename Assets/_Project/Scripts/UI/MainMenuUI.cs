using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenuContent;
    public GameObject settingsPanel;
    public GameObject howToPlayPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("03_Gameplay");
    }

    public void OpenSettings()
    {
        mainMenuContent.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuContent.SetActive(true);
    }

    public void OpenHowToPlay()
    {
        mainMenuContent.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
        mainMenuContent.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}