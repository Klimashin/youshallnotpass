using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private DamageZone _damageZone;
    [SerializeField] private float _generationZoneOffsetHorizontal = 1f;
    [SerializeField] private float _generationZoneOffsetVertical = 2f;

    private GameFSM _fsm;
    private InputActions _input;
    private GameSessionData _gameSessionData;
    private GameSettings _settings;

    private PlayerWeapon[] _weapons = new PlayerWeapon[3];

    [Inject]
    private void Construct(GameFSM fsm, InputActions input, GameSettings settings)
    {
        _fsm = fsm;
        _input = input;
        _settings = settings;
        
        _damageZone.EnemyEnteredDamageZone += OnEnemyEnteredDamageZone;

        _weapons[0] = Instantiate(_settings.StartWeapon, _playerCharacter.transform);

        var cam = Camera.main;
        _screenTopLeftWorld = cam.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f));
        _screenTopRightWorld = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        _screenBottomLeftWorld = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        _screenBottomRightWorld = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
    }

    private void OnEnemyEnteredDamageZone(object sender, Enemy e)
    {
        _gameSessionData.LoseHp(e.Damage);
        _gameSessionData.RemoveEnemy(e);
        Destroy(e.gameObject); // @TODO: use object pooling
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

        _gameSessionData = new GameSessionData(3, _weapons[0]);

        while (true)
        {
            var deltaTime = Time.deltaTime;
            HandlePlayerInput(deltaTime);
            HandlePlayerWeapon(deltaTime);
            HandleEnemySpawn(deltaTime);
            HandleEnemyMove(deltaTime);

            yield return null;
        }

        _input.Gameplay.Disable();

        _fsm.OnGameOver();
    }

    private void HandlePlayerWeapon(float deltaTime)
    {
        _gameSessionData.CurrentWeapon.WeaponUpdate(deltaTime);
    }

    private void HandleEnemyMove(float deltaTime)
    {
        foreach (var activeEnemy in _gameSessionData.ActiveEnemies)
        {
            activeEnemy.Move(deltaTime);
        }
    }

    private void HandlePlayerInput(float deltaTime)
    {
        var touch = Touchscreen.current.primaryTouch;
        if (touch.isInProgress)
        {
            var touchSideLeft = Screen.width / 2 > touch.position.ReadValue().x;
            var rotationSign = touchSideLeft ? 1 : -1;
            var rotationDelta = _settings.RotationSpeed * rotationSign * deltaTime;
            _playerCharacter.transform.Rotate(new Vector3(0f, 0f, rotationDelta), Space.World);

            ApplyRotationConstraint();
        }
    }

    private float _currentSpawnTimeout;
    private void HandleEnemySpawn(float deltaTime)
    {
        _currentSpawnTimeout -= deltaTime;
        if (_currentSpawnTimeout <= 0f)
        {
            _currentSpawnTimeout = _settings.SpawnTimeout;
            SpawnEnemy();
        }
    }

    private Vector3 _screenTopLeftWorld;
    private Vector3 _screenTopRightWorld;
    private Vector3 _screenBottomLeftWorld;
    private Vector3 _screenBottomRightWorld;
    private void SpawnEnemy()
    {
        var newEnemy = Instantiate(_settings.EnemyPrefab);

        var initialPosX = UnityEngine.Random.Range(
            _screenTopLeftWorld.x - _generationZoneOffsetHorizontal,
            _screenTopRightWorld.x + _generationZoneOffsetHorizontal
        );
        var initialPosY = _screenTopLeftWorld.y + _generationZoneOffsetVertical;
        var enemyInitialPos = new Vector3(initialPosX, initialPosY, 0f);
        newEnemy.transform.position = enemyInitialPos;

        var enemyDirectionPointX = UnityEngine.Random.Range(_screenBottomLeftWorld.x, _screenBottomRightWorld.x);
        var enemyDirectionPointY = _screenBottomLeftWorld.y;
        var enemyDirectionPoint = new Vector3(enemyDirectionPointX, enemyDirectionPointY, 0f);
        var enemyMoveDirection = (enemyDirectionPoint - enemyInitialPos).normalized;
        
        newEnemy.SetDirection(enemyMoveDirection);
        newEnemy.OnEliminatedHandler += OnEnemyEliminated;
        _gameSessionData.AddEnemy(newEnemy);
    }

    private void OnEnemyEliminated(object sender, EventArgs e)
    {
        _gameSessionData.AddScore(1);
        _gameSessionData.RemoveEnemy(sender as Enemy);
        Destroy((sender as Enemy).gameObject); // @TODO: use object pooling
    }

    private void ApplyRotationConstraint()
    {
        var currentRotation = _playerCharacter.transform.rotation.eulerAngles.z;
        var constraint = _settings.RotationConstraint;
        if (currentRotation > 180 && 360 - currentRotation > constraint)
        {
            _playerCharacter.transform.rotation = Quaternion.Euler(0f, 0f, 360f - constraint);
        }
        else if (currentRotation < 180 && currentRotation > constraint)
        {
            _playerCharacter.transform.rotation = Quaternion.Euler(0f, 0f, constraint);
        }
    }
}

public class GameSessionData
{
    public int CurrentHp { get; private set; }
    public int CurrentScore { get; private set; }
    public PlayerWeapon CurrentWeapon { get; private set; }

    private readonly List<Enemy> _activeEnemies = new();
    public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;

    public EventHandler<int> HpChanged;
    public EventHandler<int> ScoreChanged;

    public GameSessionData(int initialHp, PlayerWeapon startWeapon)
    {
        CurrentHp = initialHp;
        CurrentScore = 0;
        CurrentWeapon = startWeapon;
        CurrentWeapon.WeaponReset();
    }

    public void SetWeapon(PlayerWeapon weapon)
    {
        if (CurrentWeapon != null && CurrentWeapon != weapon)
        {
            weapon.WeaponReset();
        }

        CurrentWeapon = weapon;
    }

    public void AddEnemy(Enemy enemy)
    {
        _activeEnemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemy.OnEliminatedHandler = null;
        _activeEnemies.Remove(enemy);
    }

    public void LoseHp(int amount)
    {
        CurrentHp -= amount;
        HpChanged?.Invoke(this, amount);
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        ScoreChanged?.Invoke(this, CurrentScore);
    }
}
