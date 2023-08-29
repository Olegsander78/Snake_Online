using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private float _overlapRadius = .5f;
    [SerializeField] private float _speedRotation = 90f;

    private Transform _snakeHead;
    private Vector3 _targetDirection = Vector3.zero;
    private float _speed;

    private void Update()
    {
        Rotate();
        Move();
        CheckOutOfBounds();
    }

    private void FixedUpdate()
    {
        CheckCollision();
    }    

    public void Init(Transform snakeHead, float speed)
    {
        _snakeHead = snakeHead;
        _speed = speed;
    }   

    public void SetTargetDirection(Vector3 pointToLook)
    {
        _targetDirection = pointToLook - transform.position;        
    }

    public void GetMoveInfo(out Vector3 position)
    {
        position = transform.position;
    }

    private void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * _speedRotation);
    }

    private void Move()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }
    private void CheckOutOfBounds()
    {
        if (Mathf.Abs(_snakeHead.position.x) > 126 || Mathf.Abs(_snakeHead.position.z) > 126)
            GameOver();
    }
    private void CheckCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(_snakeHead.position, _overlapRadius, _collisionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<Apple>(out Apple apple))
            {
                apple.Collect();
            }
            else
            {
                if (colliders[i].GetComponentInParent<Snake>())
                {
                    var enemy = colliders[i].transform;
                    var playerAngle = Vector3.Angle(enemy.position - _snakeHead.position, transform.forward);
                    var enemyAngle = Vector3.Angle(_snakeHead.position - enemy.position, enemy.forward);

                    if (playerAngle < enemyAngle + 5)
                    {
                        GameOver();
                    }

                }
                else
                {
                    GameOver();
                }
            }
        }
    }

    private void GameOver()
    {
        FindObjectOfType<Controller>().Destroy();
        Destroy(gameObject);
    }
}
