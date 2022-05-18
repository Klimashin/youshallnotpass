using UnityEngine;

public class ShotgunWeapon : PlayerWeapon
{
    [SerializeField] protected Missile _missilePrefab;
    [SerializeField] private int _shotMissilesCount;
    [SerializeField] private float _shotAngle;
    [SerializeField] private Transform _missileLaunchPoint;

    protected override void Shot()
    {
        var missileRotation = _shotAngle / 2f;
        var rotationStep = -_shotAngle / _shotMissilesCount;

        for (int i = 0; i < _shotMissilesCount; i++)
        {
            var missile = Instantiate(_missilePrefab, _missileLaunchPoint); // @TODO: use object pool
            missile.transform.Rotate(new Vector3(0f, 0f, missileRotation));
            missile.transform.SetParent(null);
            missile.EnemyHitHandler += EnemyHitHandler;
            ActiveMissiles.Add(missile);

            missileRotation += rotationStep;
        }
    }
}
