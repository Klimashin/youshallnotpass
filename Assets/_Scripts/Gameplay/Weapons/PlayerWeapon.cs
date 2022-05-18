using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private float _missileSpeed;
    [SerializeField] private float _shotCooldown;
    [SerializeField] private float _missileLifetime;

    private float _currentShotCooldown;
    public virtual void WeaponUpdate(float deltaTime)
    {
        if (gameObject.activeSelf)
        {
            _currentShotCooldown -= deltaTime;
            if (_currentShotCooldown < 0f)
            {
                _currentShotCooldown = _shotCooldown;
                Shot();
            }
        }

        foreach (var missile in ActiveMissiles)
        {
            missile.Lifetime += deltaTime;
            missile.transform.Translate(missile.transform.up * (_missileSpeed * deltaTime), Space.World);
        }

        var expiredMissiles = ActiveMissiles.FindAll(missile => missile.Lifetime >= _missileLifetime);
        foreach (var missile in expiredMissiles)
        {
            UtilizeMissile(missile);
        }
    }

    public virtual void WeaponReset()
    {
        _currentShotCooldown = _shotCooldown;

        for (int i = ActiveMissiles.Count - 1; i >= 0; i--)
        {
            UtilizeMissile(ActiveMissiles[i]);
        }
    }

    protected virtual void EnemyHitHandler(object sender, Enemy e)
    {
        e.OnHit();
        UtilizeMissile(sender as Missile);
    }

    protected abstract void Shot();

    protected readonly List<Missile> ActiveMissiles = new();

    protected void UtilizeMissile(Missile missile)
    {
        missile.EnemyHitHandler -= EnemyHitHandler;
        ActiveMissiles.Remove(missile);
        Destroy(missile.gameObject); // @TODO: use object pool
    }
}
