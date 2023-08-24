using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private Transform _cursor;
    private Snake _snake;

    private Camera _camera;
    private Plane _plane;

    public void Init(Snake snake)
    {
        _snake =snake;

        _camera = Camera.main;
        _plane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveCursor();
            _snake.LookAt(_cursor.position);
        }
    }

    private void MoveCursor()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        _plane.Raycast(ray, out float distance);
        var point = ray.GetPoint(distance);

        _cursor.position = point;
    }
}
