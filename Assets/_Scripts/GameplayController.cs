using System.Collections;
using UnityEngine;
using Zenject;

public class GameplayController : MonoBehaviour
{
    private GameFSM _fsm;

    [Inject]
    private void Construct(GameFSM fsm)
    {
        _fsm = fsm;
    }

    private Coroutine _gameplayCoroutine;
    public void StartGameplaySequence()
    {
        if (_gameplayCoroutine != null)
        {
            StopCoroutine(_gameplayCoroutine);
        }

        _gameplayCoroutine = StartCoroutine(GameplayCoroutine());
    }

    private IEnumerator GameplayCoroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.Log(i+1);
            yield return new WaitForSeconds(1f);
        }

        _fsm.OnGameOver();
    }
}
