using System;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public EventHandler<Enemy> EnemyEnteredDamageZone;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            EnemyEnteredDamageZone?.Invoke(this, enemy);
        }
    }
}
