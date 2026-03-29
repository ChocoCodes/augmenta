using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;

    void Start()
    {
        
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

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
