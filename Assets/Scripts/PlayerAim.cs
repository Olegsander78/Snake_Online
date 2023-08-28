using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private float _overlapRadius = .5f;
    [SerializeField] private float _speedRotation = 90f;

    private Transform _snakeHead;
    private Vector3 _targetDirection = Vector3.zero;
    private float _speed;

    private void Update()
    {
        Rotate();
        Move();
    }

    private void FixedUpdate()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(_snakeHead.position,_overlapRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<Apple>(out Apple apple))
            {
                apple.Collect();
            }
        }
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
}
