using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HpWidget : MonoBehaviour
{
    [SerializeField] private Image _hpIcon;

    private GameplayController _gameplayController;

    [Inject]
    private void Construct(GameplayController gameplayController)
    {
        _gameplayController = gameplayController;
        _gameplayController.NewGameSessionStarted += OnNewGameSessionStarted;
    }

    private void OnNewGameSessionStarted(object sender, GameSessionData gameSessionData)
    {
        gameSessionData.HpChanged += OnHpChanged;
        SetHp(gameSessionData.CurrentHp);
    }

    private void OnHpChanged(object sender, int hp)
    {
        SetHp(hp);
    }

    private void SetHp(int currentHp)
    {
        Debug.Log(currentHp);

        if (transform.childCount < currentHp)
        {
            var childCount = transform.childCount;
            for (var i = 0; i < currentHp - childCount; i++)
            {
                Instantiate(_hpIcon, transform);
            }
        }

        for (var j = 0; j < transform.childCount; j++)
        {
            transform.GetChild(j).gameObject.SetActive(currentHp > j);
        }
    }
}
