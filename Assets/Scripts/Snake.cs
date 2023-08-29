using UnityEngine;

public class Snake : MonoBehaviour
{
    [field: SerializeField] public Transform Head { get; private set; }
    [SerializeField] private int _playerLayer = 6;
    [SerializeField] private Tail _tailPrefab;
    [SerializeField] private float _speed = 2f;
    public float Speed => _speed;

    private Tail _tail;

    private void Update()
    {
        Move();
    }

    public void Init(int detailCount, bool isPlayer = false)
    {
        if (isPlayer)
        {
            gameObject.layer = _playerLayer;
            var children = GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].gameObject.layer = _playerLayer;
            }
        }
        _tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        _tail.Init(Head, _speed, detailCount, _playerLayer, isPlayer);
    }

    public void SetDetailCount(int detailCount)
    {
        _tail.SetDetailCount(detailCount);
    }

    public void Destroy(string clientId)
    {
        var detailPositions = _tail.GetDetailpositions();
        detailPositions.id = clientId;
        string json = JsonUtility.ToJson(detailPositions);
        MultiplayerManager.Instance.SendMessage("gameOver", json);
        _tail.Destroy();
        Destroy(gameObject);
    }            

    public void SetRotation(Vector3 pointToLook)
    {
        Head.LookAt(pointToLook);
    }    

    private void Move()
    {
        transform.position += Head.forward * _speed * Time.deltaTime;
    }    
}
