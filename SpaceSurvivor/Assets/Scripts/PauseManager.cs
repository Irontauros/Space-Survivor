using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu; // Assign PauseMenu Panel in Inspector
    public static bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Toggle pause with ESC
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // Freeze game
        isPaused = true;
        pauseMenu.SetActive(true); // Show menu

        // Unlock cursor & make it visible for UI interactions
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // Resume game
        isPaused = false;
        pauseMenu.SetActive(false); // Hide menu

        // Lock cursor back if needed (FPS games)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1; // Reset time scale before switching scenes
        SceneManager.LoadScene("Menu"); // Load main menu
    }
}
