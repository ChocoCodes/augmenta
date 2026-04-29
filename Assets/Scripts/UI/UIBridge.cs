using UnityEngine;

public class UIBridge : MonoBehaviour
{
    void Start()
    {
        if (UIManager.Instance == null) return;

        // The name check ensures the right prefab fills the right slot
        if (gameObject.name.Contains("MainMenu"))
        {
            UIManager.Instance.RegisterMainPanel(this.gameObject);
        }
        else if (gameObject.name.Contains("GameModeSelect"))
        {
            UIManager.Instance.RegisterGamePanel(this.gameObject);
        }
        else if (gameObject.name.Contains("OpponentSelect"))
        {
            UIManager.Instance.RegisterOpponentPanel(this.gameObject);
        }

    }

    public void OpenOpponents() => UIManager.Instance.OpenOpponentSelect();
    public void BackToMain() => UIManager.Instance.ReturnToMainMenu();
    public void StartGameScene(string sceneName) => UIManager.Instance.NextScene(sceneName);
    public void Exit() => UIManager.Instance.ExitGame();
    public void StartGame() => UIManager.Instance.StartGame();
}
