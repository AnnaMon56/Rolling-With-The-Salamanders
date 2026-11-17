using UnityEngine;
using UnityEngine.SceneManagement;

// Lives in the GAME scene. Handles the game-over screen + restart.
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject hudPanel;          // optional

    [Header("Menu scene to return to")]
    public string menuSceneName = "Menu";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        Debug.Log("ShowGameOver running, panel = " + (gameOverPanel != null ? gameOverPanel.name : "NULL"));
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (hudPanel != null) hudPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}