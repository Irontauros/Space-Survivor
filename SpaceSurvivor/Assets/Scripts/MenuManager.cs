using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;
    public GameObject title1;
    public GameObject title2;
    public AudioClip backgroundMusic;  
    public AudioClip buttonSound;     

    private AudioSource musicSource;
    private AudioSource buttonSource;
    private ColorBlock originalStartButtonColors;
    private ColorBlock originalQuitButtonColors;

    private void Start()
    {

        originalStartButtonColors = startButton.colors;
        originalQuitButtonColors = quitButton.colors;

        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);

        StartCoroutine(SwitchTitles());

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = true;
        musicSource.volume = 0.5f; 
        musicSource.Play();

        buttonSource = gameObject.AddComponent<AudioSource>();
        buttonSource.clip = buttonSound;
        buttonSource.playOnAwake = false;
        buttonSource.volume = 1f;
    }

    public void StartGame()
    {
        PlayButtonSound();
        ChangeButtonColor(startButton, Color.gray);

        StartCoroutine(LoadSceneAfterDelay("Main", startButton));
    }

    public void QuitGame()
    {
        PlayButtonSound();
        ChangeButtonColor(quitButton, Color.gray);

        StartCoroutine(QuitAfterDelay(quitButton));
    }

    private void PlayButtonSound()
    {
        if (buttonSource != null && buttonSound != null)
        {
            buttonSource.Play();
        }
    }

    private void ChangeButtonColor(Button button, Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        button.colors = cb;
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, Button button)
    {
        yield return new WaitForSeconds(0.5f); 
        SceneManager.LoadScene(sceneName);

        button.colors = originalStartButtonColors;
    }

    private IEnumerator QuitAfterDelay(Button button)
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        button.colors = originalQuitButtonColors;
    }

    private IEnumerator SwitchTitles()
    {
        while (true) 
        {
            title1.SetActive(true);
            title2.SetActive(false);
            yield return new WaitForSeconds(1f);

            title1.SetActive(false);
            title2.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
    }
}
