using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject joystickPanel;

    [Header("SFX")]
    [SerializeField] private AudioClip gameStartSfx; // Bell Ring
    [SerializeField] private AudioClip gameBgm; // Crowd Cheer

    private bool matchEnded = false;

    public AudioManager GetAudioManager() => AudioManager.GetInstance();
    public AudioClip GetGameStartSfx() => gameStartSfx;

    private void PlayButtonSfx()
    {
        GetAudioManager().PlayBtnSfx();
    }

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        GetAudioManager().PlayAudio(gameBgm, true);

        UpdateJoystickVisibility();
    }

    public void ShowPausePanel()
    {
        if (matchEnded) return;

        PlayButtonSfx();

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
        UpdateJoystickVisibility();
    }

    public void HidePausePanel()
    {
        if (matchEnded) return;

        PlayButtonSfx();

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        UpdateJoystickVisibility();
    }

    public void ShowWinPanel()
    {
        if (matchEnded) return;

        CareerProgression.MarkWinForScene(SceneManager.GetActiveScene().name);

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
        PlayButtonSfx();

        matchEnded = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        UpdateJoystickVisibility();
        SceneManager.LoadScene("MainMenu");
    }

    public void ReloadScene()
    {
        PlayButtonSfx();

        matchEnded = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextScene(string sceneName)
    {
        PlayButtonSfx();

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
