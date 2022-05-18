using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _damage = 1;
    [SerializeField] private int _scorePoints = 1;
    [SerializeField] private int _speed;

    public EventHandler OnEliminatedHandler;

    public int Damage => _damage;
    public int ScorePoints => _scorePoints;

    private Vector3 MoveDirection { get; set; }

    private int _currentHp;
    public void SetHp(int hp)
    {
        _currentHp = hp;
    }

    public void SetDirection(Vector3 enemyMoveDirection)
    {
        MoveDirection = enemyMoveDirection;
        float angle = Mathf.Atan2(MoveDirection.y, MoveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Move(float deltaTime)
    {
        transform.Translate(MoveDirection * (_speed * deltaTime), Space.World);
    }

    public void OnHit()
    {
        _currentHp--;
        if (_currentHp <= 0)
        {
            OnEliminatedHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}
