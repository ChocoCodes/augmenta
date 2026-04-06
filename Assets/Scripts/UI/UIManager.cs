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

    void Start()
    {
        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (opponentSelectPanel != null)
            opponentSelectPanel.SetActive(false);

        if (inGamePanel != null)
            inGamePanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
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
        if (opponentSelectPanel != null)
            opponentSelectPanel.SetActive(false);

        if (inGamePanel != null)
            inGamePanel.SetActive(true);
    }

    public void ShowPausePanel()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void HidePausePanel()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
