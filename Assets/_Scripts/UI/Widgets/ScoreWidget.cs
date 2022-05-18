using UnityEngine;
using TMPro;
using Zenject;

public class ScoreWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private GameplayController _gameplayController;

    [Inject]
    private void Construct(GameplayController gameplayController)
    {
        _gameplayController = gameplayController;
        _gameplayController.NewGameSessionStarted += OnNewGameSessionStarted;
    }

    private void OnNewGameSessionStarted(object sender, GameSessionData gameSessionData)
    {
        gameSessionData.ScoreChanged += OnScoreChanged;
        SetScore(gameSessionData.CurrentScore);
    }

    private void OnScoreChanged(object sender, int score)
    {
        SetScore(score);
    }

    private void SetScore(int score)
    {
        _scoreText.text = $"Score: {score}";
    }
}
