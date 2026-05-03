using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject opponentSelectPanel;

    [Header("Opponent Select Buttons")]
    [SerializeField] private Button midasButton;
    [SerializeField] private Button metroButton;
    [SerializeField] private Button zeusButton;
    [SerializeField] private Button twinButton;

    [Header("Opponent Select Images")]
    [SerializeField] private Image midasImage;
    [SerializeField] private Image metroImage;
    [SerializeField] private Image zeusImage;
    [SerializeField] private Image twinImage;

    [Header("Opponent Sprites")]
    [SerializeField] private Sprite midasOpenSprite;
    [SerializeField] private Sprite midasDefeatedSprite;
    [SerializeField] private Sprite metroLockedSprite;
    [SerializeField] private Sprite metroOpenSprite;
    [SerializeField] private Sprite metroDefeatedSprite;
    [SerializeField] private Sprite twinLockedSprite;
    [SerializeField] private Sprite twinOpenSprite;
    [SerializeField] private Sprite twinDefeatedSprite;
    [SerializeField] private Sprite zeusLockedSprite;
    [SerializeField] private Sprite zeusOpenSprite;
    [SerializeField] private Sprite zeusDefeatedSprite;

    [Header("SFX")]
    [SerializeField] private AudioClip bgm;

    public AudioManager GetAudioManager() => AudioManager.GetInstance();

    private void PlayButtonSfx()
    {
        GetAudioManager().PlayBtnSfx();
    }

    void Start()
    {
        Time.timeScale = 1f;

        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (opponentSelectPanel != null)
            opponentSelectPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        GetAudioManager().PlayAudio(bgm, true);

        RefreshOpponentSelectState();
    }

    public void StartGame(GameObject panelToClose, GameObject panelToOpen)
    {
        PlayButtonSfx();

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
        PlayButtonSfx();

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (opponentSelectPanel != null)
            opponentSelectPanel.SetActive(true);

        RefreshOpponentSelectState();
    }

    public void ExitGame()
    {
        PlayButtonSfx();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void NextScene(string sceneName)
    {
        PlayButtonSfx();

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void RefreshOpponentSelectState()
    {
        int unlockedIndex = CareerProgression.GetUnlockedIndex();

        ApplyOpponentVisuals(
            0,
            midasButton,
            midasImage,
            null,
            midasOpenSprite,
            midasDefeatedSprite,
            unlockedIndex);

        ApplyOpponentVisuals(
            1,
            metroButton,
            metroImage,
            metroLockedSprite,
            metroOpenSprite,
            metroDefeatedSprite,
            unlockedIndex);

        ApplyOpponentVisuals(
            2,
            twinButton,
            twinImage,
            twinLockedSprite,
            twinOpenSprite,
            twinDefeatedSprite,
            unlockedIndex);

        ApplyOpponentVisuals(
            3,
            zeusButton,
            zeusImage,
            zeusLockedSprite,
            zeusOpenSprite,
            zeusDefeatedSprite,
            unlockedIndex);
    }

    private static void ApplyOpponentVisuals(
        int index,
        Button button,
        Image image,
        Sprite lockedSprite,
        Sprite openSprite,
        Sprite defeatedSprite,
        int unlockedIndex)
    {
        bool isUnlocked = index <= unlockedIndex;
        bool isDefeated = CareerProgression.IsOpponentDefeated(index);

        if (button != null)
        {
            button.interactable = isUnlocked;

            ColorBlock colors = button.colors;
            Color disabled = colors.disabledColor;
            disabled.a = 1f;
            colors.disabledColor = disabled;
            button.colors = colors;
        }

        if (image == null) return;

        if (isDefeated && defeatedSprite != null)
        {
            image.sprite = defeatedSprite;
            return;
        }

        if (isUnlocked && openSprite != null)
        {
            image.sprite = openSprite;
            return;
        }

        if (!isUnlocked && lockedSprite != null)
        {
            image.sprite = lockedSprite;
            return;
        }

        if (openSprite != null)
        {
            image.sprite = openSprite;
        }
    }
}