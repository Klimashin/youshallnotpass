using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _damage = 1;
    [SerializeField] private int _speed;

    public EventHandler OnEliminatedHandler;

    public int Damage => _damage;

    private Vector3 MoveDirection { get; set; }

    public void SetDirection(Vector3 enemyMoveDirection)
    {
        MoveDirection = enemyMoveDirection;
        transform.rotation.SetLookRotation(MoveDirection, Vector3.forward);
    }

    public void Move(float deltaTime)
    {
        transform.Translate(MoveDirection * (_speed * deltaTime));
    }

    public void OnHit()
    {
        OnEliminatedHandler?.Invoke(this, EventArgs.Empty);
    }
}
