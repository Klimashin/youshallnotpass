using System.Collections.Generic;
using UnityEngine;

public class SingleShotWeapon : PlayerWeapon
{
    [SerializeField] private Missile _missile;
    [SerializeField] private float _missileSpeed;
    [SerializeField] private float _shotCooldown;
    [SerializeField] private float _missileLifetime;
    [SerializeField] private Transform _missileLaunchPoint;

    private readonly List<Missile> _activeMissiles = new();
    private float _currentShotCooldown;
    
    public override void WeaponUpdate(float deltaTime)
    {
        _currentShotCooldown -= deltaTime;
        if (_currentShotCooldown < 0f)
        {
            _currentShotCooldown = _shotCooldown;
            ShotMissile();
        }
        
        foreach (var missile in _activeMissiles)
        {
            missile.Lifetime += deltaTime;
            missile.transform.Translate(missile.transform.up * (_missileSpeed * deltaTime));
        }

        var expiredMissiles = _activeMissiles.FindAll(missile => missile.Lifetime >= _missileLifetime);
        foreach (var missile in expiredMissiles)
        {
            UtilizeMissile(missile);
        }
    }

    public override void EnemyHitHandler(object sender, Enemy e)
    {
        e.OnHit();
        UtilizeMissile(sender as Missile);
    }

    public override void WeaponReset()
    {
        _currentShotCooldown = _shotCooldown;

        foreach (var missile in _activeMissiles)
        {
            UtilizeMissile(missile);
        }
        
        _activeMissiles.Clear();
    }
    
    private void ShotMissile()
    {
        var missile = Instantiate(_missile, _missileLaunchPoint);
        missile.transform.SetParent(null);
        missile.EnemyHitHandler += EnemyHitHandler;
        _activeMissiles.Add(missile);
    }

    private void UtilizeMissile(Missile missile)
    {
        missile.EnemyHitHandler -= EnemyHitHandler;
        _activeMissiles.Remove(missile);
        Destroy(missile.gameObject); // @TODO: use object pool
    }
}
