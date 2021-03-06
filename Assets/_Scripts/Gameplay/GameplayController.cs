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

    public EventHandler<GameSessionData> NewGameSessionStarted;

    private GameSessionData _gameSessionData;
    private GameFSM _fsm;
    private InputActions _input;
    private GameSettings _settings;
    private PlayerWeapon[] _weapons = new PlayerWeapon[3];
    private int _currentWeaponIndex;

    [Inject]
    private void Construct(GameFSM fsm, InputActions input, GameSettings settings)
    {
        _fsm = fsm;
        _input = input;
        _settings = settings;
        
        _damageZone.EnemyEnteredDamageZone += OnEnemyEnteredDamageZone;

        for (int i = 0; i < _settings.Weapons.Length; i++)
        {
            _weapons[i] = Instantiate(_settings.Weapons[i], _playerCharacter.transform);
            _weapons[i].gameObject.SetActive(false);
        }

        var cam = Camera.main;
        _screenTopLeftWorld = cam.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f));
        _screenTopRightWorld = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        _screenBottomLeftWorld = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        _screenBottomRightWorld = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
    }

    private void OnEnemyEnteredDamageZone(object sender, Enemy enemy)
    {
        _gameSessionData.LoseHp(enemy.Damage);
        UtilizeEnemy(enemy);
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

    public void SetCurrentWeapon(int weaponIndex)
    {
        if (_currentWeaponIndex != weaponIndex)
        {
            _weapons[_currentWeaponIndex].gameObject.SetActive(false);
        }

        _currentWeaponIndex = weaponIndex;
        _weapons[_currentWeaponIndex].gameObject.SetActive(true);
    }

    private IEnumerator GameplayCoroutine()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();

        _playerCharacter.transform.rotation = Quaternion.identity;
        SetCurrentWeapon(0);
        _gameSessionData = new GameSessionData(_settings.InitialHp, _settings.WeaponUnlockLimits);

        NewGameSessionStarted?.Invoke(this, _gameSessionData);

        yield return new WaitForSeconds(1f);

        _input.Gameplay.Enable();

        while (_gameSessionData.CurrentHp > 0)
        {
            var deltaTime = Time.deltaTime;
            HandlePlayerInput(deltaTime);
            HandlePlayerWeapons(deltaTime);
            HandleEnemySpawn(deltaTime);
            HandleEnemyMove(deltaTime);

            yield return null;
        }

        UtilizeAllEnemies();
        ResetAllWeapons();

        _input.Gameplay.Disable();

        _fsm.OnGameOver(_gameSessionData);
    }

    private void HandlePlayerWeapons(float deltaTime)
    {
        foreach (var weapon in _weapons)
        {
            weapon.WeaponUpdate(deltaTime);
        }
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
            _currentSpawnTimeout = _settings.EnemySpawnTimeout;
            SpawnEnemy();
        }
    }

    private Vector3 _screenTopLeftWorld;
    private Vector3 _screenTopRightWorld;
    private Vector3 _screenBottomLeftWorld;
    private Vector3 _screenBottomRightWorld;
    private void SpawnEnemy()
    {
        var newEnemy = Instantiate(_settings.EnemyPrefab); // @TODO: use object pooling

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
        newEnemy.SetHp(UnityEngine.Random.Range(_settings.EnemyHpRange.x, _settings.EnemyHpRange.y));
        newEnemy.OnEliminatedHandler += OnEnemyEliminated;
        _gameSessionData.AddEnemy(newEnemy);
    }

    private void OnEnemyEliminated(object sender, EventArgs e)
    {
        var enemy = sender as Enemy;
        _gameSessionData.AddScore(enemy.ScorePoints);
        UtilizeEnemy(enemy);
    }

    private void UtilizeEnemy(Enemy enemy)
    {
        _gameSessionData.RemoveEnemy(enemy);
        Destroy(enemy.gameObject); // @TODO: use object pooling
    }

    private void ResetAllWeapons()
    {
        foreach (var weapon in _weapons)
        {
            weapon.WeaponReset();
        }
    }

    private void UtilizeAllEnemies()
    {
        var count = _gameSessionData.ActiveEnemies.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            UtilizeEnemy(_gameSessionData.ActiveEnemies[i]);
        }
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

    private readonly List<Enemy> _activeEnemies = new();
    public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;

    public EventHandler<int> HpChanged;
    public EventHandler<int> ScoreChanged;
    public EventHandler NextWeaponUnlocked;

    private int _currentUnlockLimitIndex = 0;
    private int[] _weaponsUnlockLimits;

    public GameSessionData(int initialHp, int[] weaponsUnlockLimits)
    {
        CurrentHp = initialHp;
        CurrentScore = 0;
        _weaponsUnlockLimits = weaponsUnlockLimits;
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
        CurrentHp = Mathf.Max(0, CurrentHp);
        HpChanged?.Invoke(this, CurrentHp);
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        ScoreChanged?.Invoke(this, CurrentScore);

        if (
            _currentUnlockLimitIndex < _weaponsUnlockLimits.Length
            && CurrentScore >= _weaponsUnlockLimits[_currentUnlockLimitIndex]
        ) {
            _currentUnlockLimitIndex++;
            NextWeaponUnlocked?.Invoke(this, EventArgs.Empty);
        }
    }
}
