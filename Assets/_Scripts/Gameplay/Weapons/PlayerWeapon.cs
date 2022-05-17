using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour
{
    public abstract void WeaponUpdate(float deltaTime);


    public abstract void EnemyHitHandler(object sender, Enemy e);

    public abstract void WeaponReset();
}
