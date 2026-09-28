using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootLoader : MonoBehaviour
{
    public Image loadingBarFill;
    public float loadDuration = 2f;

    private void Start()
    {
        StartCoroutine(LoadMainMenu());
    }

    private IEnumerator LoadMainMenu()
    {
        float timer = 0f;

        while (timer < loadDuration)
        {
            timer += Time.deltaTime;

            float progress = timer / loadDuration;

            if (loadingBarFill != null)
            {
                loadingBarFill.fillAmount = progress;
            }

            yield return null;
        }

        SceneManager.LoadScene("02_MainMenu");
    }
}