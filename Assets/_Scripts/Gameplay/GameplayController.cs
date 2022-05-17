using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private DamageZone _damageZone;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationConstraint = 60f;

    private GameFSM _fsm;
    private InputActions _input;

    [Inject]
    private void Construct(GameFSM fsm, InputActions input)
    {
        _fsm = fsm;
        _input = input;

        //input.Gameplay.TouchPos.performed += TouchPos_performed;
    }

    private void TouchPos_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        var touchScreenPos = context.ReadValue<Vector2>();
        var touchSideLeft = Screen.width / 2 > touchScreenPos.x;

        var rotationSign = touchSideLeft ? 1 :-1;
        var rotation = _rotationSpeed * rotationSign * Time.deltaTime;
        _playerCharacter.transform.Rotate(new Vector3(0f, 0f, rotation), Space.World);
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
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();

        yield return new WaitForSeconds(1f);

        _input.Gameplay.Enable();

        while (true)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.isInProgress)
            {
                var touchSideLeft = Screen.width / 2 > touch.position.ReadValue().x;
                var rotationSign = touchSideLeft ? 1 : -1;
                var rotationDelta = _rotationSpeed * rotationSign * Time.deltaTime;
                _playerCharacter.transform.Rotate(new Vector3(0f, 0f, rotationDelta), Space.World);

                ApplyRotationConstraint();
            }

            yield return null;
        }

        _input.Gameplay.Disable();

        _fsm.OnGameOver();
    }

    private void ApplyRotationConstraint()
    {
        var currentRotation = _playerCharacter.transform.rotation.eulerAngles.z;

        if (currentRotation > 180 && 360 - currentRotation > _rotationConstraint)
        {
            _playerCharacter.transform.rotation = Quaternion.Euler(0f, 0f, 360f - _rotationConstraint);
        }
        else if (currentRotation < 180 && currentRotation > _rotationConstraint)
        {
            _playerCharacter.transform.rotation = Quaternion.Euler(0f, 0f, _rotationConstraint);
        }
    }
}
