using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WeaponSelectWidget : UIElement
{
    [SerializeField] private Button[] _weaponButtons;

    private GameplayController _gameplayController;

    [Inject]
    private void Construct(GameplayController gameplayController)
    {
        _gameplayController = gameplayController;
        _gameplayController.NewGameSessionStarted += OnNewGameSessionStarted;
    }

    private void Awake()
    {
        for (int i = 0; i < _weaponButtons.Length; i++)
        {
            var iScoped = i;
            _weaponButtons[i].onClick.AddListener(() => SelectWeapon(iScoped));
        }
    }

    private void SelectWeapon(int i)
    {
        _gameplayController.SetCurrentWeapon(i);
    }

    private void OnNewGameSessionStarted(object sender, GameSessionData gameSession)
    {
        _lastUnlockedWeaponIndex = -1;

        UnlockNextWeapon();

        for (int i = 1; i < _weaponButtons.Length; i++)
        {
            _weaponButtons[i].enabled = false;
            _weaponButtons[i].GetComponent<Image>().color = Color.red;
        }

        gameSession.NextWeaponUnlocked += OnNextWeaponUnlocked;
    }

    private void OnNextWeaponUnlocked(object sender, System.EventArgs e)
    {
        UnlockNextWeapon();
    }

    private int _lastUnlockedWeaponIndex = 0;
    private void UnlockNextWeapon()
    {
        _lastUnlockedWeaponIndex++;
        if (_weaponButtons.Length > _lastUnlockedWeaponIndex)
        {
            _weaponButtons[_lastUnlockedWeaponIndex].enabled = true;
            _weaponButtons[_lastUnlockedWeaponIndex].GetComponent<Image>().color = Color.green;
        }
    }
}
