using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

public class AiController : MonoBehaviour
{
    [SerializeField] private Transform[] _targerPoints;
    [SerializeField] private float _chasingSpeed;
    [SerializeField] private float _walkingSpeed;
    [SerializeField] private float _viewDistance;

    private readonly float _minDistanceToPoint = 0.1f;
    private RaycastHit2D _hit;
    private Rigidbody2D _rigidbody2D;
    private Transform _currentPoint;
    private Transform _target;
    private float _currentSpeed;
    private float _direction;
    private bool _isChasing;
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        SelectCurrentPoint();
        _currentSpeed = _walkingSpeed;
    }

    private void SelectCurrentPoint()
    {
        if (_isChasing) return;
        int pointIndex = Random.Range(0, _targerPoints.Length);
        _currentPoint = _targerPoints[pointIndex];
    }

    private void FixedUpdate()
    {
        _direction = _currentPoint.position.x - transform.position.x;
        _direction = _direction < 0 ? -1 : 1;
        Rotate();
        TryFindPlayer();
        Move();

        if (Mathf.Abs(transform.position.x - _currentPoint.position.x) < _minDistanceToPoint)
        {
            SelectCurrentPoint();
        }
    }
    private void Rotate()
    {
        transform.rotation = Quaternion.Euler(0, -_direction * 90 + 90, 0);
    }    

    private void TryFindPlayer()
    {
        _hit = Physics2D.Raycast(transform.position, transform.right, _viewDistance);
        Debug.DrawRay(transform.position, transform.right *  _viewDistance, Color.red);
        if (_hit.collider.CompareTag("Player"))
        {
            Debug.Log("Игрок найден");
            _currentPoint = _hit.transform;
            _isChasing = true;
        }
        else 
        {
            _isChasing = false;
        }
    }
    private void Move()
    {
        _rigidbody2D.velocity = new Vector2(_direction * _currentSpeed, _rigidbody2D.velocity.y);
    }
}
