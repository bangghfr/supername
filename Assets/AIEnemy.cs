using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    private float _currentDistance;
    [SerializeField] private float _walkingSpeed;
    [SerializeField] private float _chasingSpeed;
    [SerializeField] public Transform[] _currentPoints;
    private float _currentSpeed;   
    private float _direction;
    private float _minDistance = 0.1f;
    private bool _isChasing;

    private float _pointDistance;
    private Transform _currentPoint;
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _currentSpeed = _chasingSpeed;
        SelectCurrentPoint();
    }

    private void SelectCurrentPoint()
    {
        if (_isChasing) return;
        int pointIndex = Random.Range(0, _currentPoints.Length);
        _currentPoint = _currentPoints[pointIndex];
    }

    private void FixedUpdate()
    {
        _direction = _currentPoint.position.x - transform.position.x;
        _direction = _direction < 0 ? -1 : 1;
        Rotate();
        Move();
        if (Mathf.Abs(_currentPoint.position.x - transform.position.x) <= _minDistance)
        {
            SelectCurrentPoint();
        }
    }

    private void Move()
    {
        _rigidbody2D.velocity = new Vector2(_direction * _currentSpeed, _rigidbody2D.velocity.y);
    }
    private void Rotate ()
    {
        transform.rotation = Quaternion.Euler (0, _direction * 90 + 90, 0);
    }
}
