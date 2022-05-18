using UnityEngine;

[CreateAssetMenu(menuName = "Custom/GameSettings")]
public class GameSettings : ScriptableObject
{
    public PlayerWeapon[] Weapons;
    public int[] WeaponUnlockLimits;
    public Enemy EnemyPrefab;
    public float RotationSpeed = 50f;
    public float RotationConstraint = 60f;
    public float EnemySpawnTimeout = 1f;
    public int InitialHp = 3;
}
