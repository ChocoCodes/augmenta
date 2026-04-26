using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject opponentSelectPanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject joystickPanel;

    private bool matchEnded = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (opponentSelectPanel != null)
            opponentSelectPanel.SetActive(false);

        if (inGamePanel != null)
            inGamePanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        UpdateJoystickVisibility();
    }

    void Update()
    {
        
    }

    public void StartGame(GameObject panelToClose, GameObject panelToOpen)
    {
        if (panelToClose != null)
            panelToClose.SetActive(false);

        if (panelToOpen != null)
            panelToOpen.SetActive(true);
    }

    public void StartGame()
    {
        StartGame(mainMenuPanel, gamePanel);
    }

    public void OpenOpponentSelect()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (opponentSelectPanel != null)
            opponentSelectPanel.SetActive(true);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    //NOT FINAL. IN GAME SHOULD BE IN ANOTHER SCENE. FOR DEMO PURPOSES
    public void OpenInGamePanel()
    {
        SceneManager.LoadScene("vsMidas");

        UpdateJoystickVisibility();
    }

    public void ShowPausePanel()
    {
        if (matchEnded) return;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
        UpdateJoystickVisibility();
    }

    public void HidePausePanel()
    {
        if (matchEnded) return;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        UpdateJoystickVisibility();
    }

    public void ShowWinPanel()
    {
        if (matchEnded) return;

        matchEnded = true;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(true);

        if (losePanel != null)
            losePanel.SetActive(false);

        UpdateJoystickVisibility();
    }

    public void ShowLosePanel()
    {
        if (matchEnded) return;

        matchEnded = true;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(true);

        if (winPanel != null)
            winPanel.SetActive(false);

        UpdateJoystickVisibility();
    }

    public void ReturnToMainMenu()
    {
        matchEnded = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        SceneManager.LoadScene("MainMenu");

        if (inGamePanel != null)
            inGamePanel.SetActive(false);

        if (opponentSelectPanel != null)
            opponentSelectPanel.SetActive(false);

        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        UpdateJoystickVisibility();
    }

    public void ReloadScene()
    {
        matchEnded = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //SceneManager.LoadScene("vsMetro");
    }

    public void NextScene(string sceneName)
    {
        matchEnded = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void UpdateJoystickVisibility()
    {
        if (joystickPanel == null) return;

        bool isOverlayVisible =
            (pausePanel != null && pausePanel.activeInHierarchy) ||
            (winPanel != null && winPanel.activeInHierarchy) ||
            (losePanel != null && losePanel.activeInHierarchy);

        joystickPanel.SetActive(!isOverlayVisible);
    }
}
