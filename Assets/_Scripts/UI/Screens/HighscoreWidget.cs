using UnityEngine;
using TMPro;
using Zenject;

public class HighscoreWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highscoreText;

    private PersistentStorage _storage;

    [Inject]
    private void Construct(PersistentStorage storage)
    {
        _storage = storage;
    }

    private void OnEnable()
    {
        _highscoreText.text = $"Highscore: {_storage.GetHighscore()}";
    }
}
