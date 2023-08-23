using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _directionPoint;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _rotateSpeed = 90f;

    private Vector3 _targetDirection = Vector3.zero;

    private void Update()
    {
        Rotate();
        Move();        
    }

    private void Move()
    {
        transform.position += _head.forward * _speed * Time.deltaTime;
    }

    private void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        _head.rotation = Quaternion.RotateTowards(_head.rotation, targetRotation, Time.deltaTime * _rotateSpeed);

        //var diffY = _directionPoint.eulerAngles.y - _head.eulerAngles.y;

        //if (diffY > 180)
        //    diffY = (diffY - 180) * -1;
        //else if (diffY < -180)
        //    diffY = (diffY + 180) * -1;

        //float maxAngle = Time.deltaTime * _rotateSpeed;
        //float rotateY = Mathf.Clamp(diffY, -maxAngle, maxAngle);
        //_head.Rotate(0f, rotateY, 0f);
    }

    public void LookAt(Vector3 cursorPosition)
    {
        _targetDirection = cursorPosition - _head.position;
        
        //_directionPoint.LookAt(cursorPosition);
    }
}
