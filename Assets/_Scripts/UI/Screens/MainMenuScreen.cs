using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : UIScreen
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _quitGameButton;

    protected override void OnPostShow()
    {
        _startGameButton.onClick.AddListener(OnStartGameButtonClick);
        _quitGameButton.onClick.AddListener(OnQuitGameButtonClick);
    }
    
    protected override void OnPreHide()
    {
        _startGameButton.onClick.RemoveAllListeners();
        _quitGameButton.onClick.RemoveAllListeners();
    }

    private void OnQuitGameButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void OnStartGameButtonClick()
    {
        Debug.Log("START_GAME");
    }
}
