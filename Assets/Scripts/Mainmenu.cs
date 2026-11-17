using UnityEngine;
using UnityEngine.SceneManagement;

// Lives in the MENU scene. Handles the main menu + instructions panels.
public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;

    [Header("Scene to load on Play")]
    public string gameSceneName = "SampleScene";

    void Start()
    {
        Time.timeScale = 1f;   // reset in case we returned from a paused game-over
        ShowMenu();
    }

    public void Play()                 // Play button
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowInstructions()     // Instructions button
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void ShowMenu()             // Back button
    {
        instructionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}