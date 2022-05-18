using UnityEngine;

public class SingleShotWeapon : PlayerWeapon
{
    [SerializeField] protected Missile _missilePrefab;
    [SerializeField] private Transform _missileLaunchPoint;

    protected override void Shot()
    {
        var missile = Instantiate(_missilePrefab, _missileLaunchPoint); // @TODO: use object pool
        missile.transform.SetParent(null);
        missile.EnemyHitHandler += EnemyHitHandler;
        ActiveMissiles.Add(missile);
    }
}
