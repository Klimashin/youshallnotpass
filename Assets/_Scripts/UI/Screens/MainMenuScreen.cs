using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class MainMenuScreen : UIElement
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _quitGameButton;
    [SerializeField] private TextMeshProUGUI _highscoreText;

    private GameFSM _fsm;

    [Inject]
    private void Construct(GameFSM fsm)
    {
        _fsm = fsm;

        _startGameButton.onClick.AddListener(_fsm.OnStartGameButtonPressed);
        _quitGameButton.onClick.AddListener(OnQuitGameButtonClick);
    }

    private void OnQuitGameButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
