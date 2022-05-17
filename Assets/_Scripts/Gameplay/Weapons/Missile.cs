using System;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public EventHandler<Enemy> EnemyHitHandler;

    public float Lifetime { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            EnemyHitHandler?.Invoke(this, enemy);
        }
    }
}
