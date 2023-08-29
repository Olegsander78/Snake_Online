using System;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    [SerializeField] private Transform _detailPrefab;
    [SerializeField] private float _detailDistance = 1;

    private float _snakeSpeed = 3f;
    private Transform _head;
    private List<Transform> _details = new();
    private List<Vector3> _positionHistory = new List<Vector3>();
    private List<Quaternion> _rotationHistory = new List<Quaternion>();
    private int _playerLayer;
    private bool _isPlayer;

    private void Update()
    {
        var distance = (_head.position - _positionHistory[0]).magnitude;

        while (distance > _detailDistance)
        {
            var direction = (_head.position - _positionHistory[0]).normalized;

            _positionHistory.Insert(0, _positionHistory[0] + direction * _detailDistance);
            _positionHistory.RemoveAt(_positionHistory.Count - 1);

            _rotationHistory.Insert(0, _head.rotation);
            _rotationHistory.RemoveAt(_rotationHistory.Count - 1);

            distance -= _detailDistance;
        }

        for (int i = 0; i < _details.Count; i++)
        {
            var percent = distance / _detailDistance;

            _details[i].position = Vector3.Lerp(_positionHistory[i + 1], _positionHistory[i], percent);
            _details[i].rotation = Quaternion.Lerp(_rotationHistory[i + 1], _rotationHistory[i], percent);
        }
    }

    public void Init(Transform head,float speed, int detailCount, int playerLayer, bool isPlayer)
    {
        _playerLayer = playerLayer;
        _isPlayer = isPlayer;

        if (isPlayer)
            SetPlayerLayer(gameObject);

        _snakeSpeed = speed;
        _head = head;

        _details.Add(transform);
        _positionHistory.Add(_head.position);
        _rotationHistory.Add(_head.rotation);
        _positionHistory.Add(transform.position);
        _rotationHistory.Add(transform.rotation);

        SetDetailCount(detailCount);
    }

    public void Destroy()
    {
        for (int i = 0; i < _details.Count; i++)
        {
            Destroy(_details[i].gameObject);
        }
    }

    public void SetDetailCount(int detailCount)
    {
        if (detailCount == _details.Count - 1)
            return;

        int diff = (_details.Count - 1) - detailCount;

        if(diff < 1)
        {
            for (int i = 0; i < -diff; i++)
            {
                AddDetail();
            }
        }
        else
        {
            for (int i = 0; i < diff; i++)
            {
                RemoveDetail();
            }
        }
    }

    public DetailPositions GetDetailpositions()
    {
        var detailCount = _details.Count;
        DetailPosition[] ds = new DetailPosition[detailCount];
        for (int i = 0; i < detailCount; i++)
        {
            ds[i] = new DetailPosition()
            {
                x = _details[i].position.x,
                z = _details[i].position.z
            };
        }

        DetailPositions detailPositions = new DetailPositions()
        {
            ds = ds
        };

        return detailPositions;
    }

    private void AddDetail()
    {
        var position = _details[_details.Count - 1].position;
        var rotation = _details[_details.Count - 1].rotation;
        var detail = Instantiate(_detailPrefab, position, rotation);
        
        if (_isPlayer)
            SetPlayerLayer(detail.gameObject);

        _details.Insert(0, detail);
        _positionHistory.Add(position);
        _rotationHistory.Add(rotation);
    }

    private void RemoveDetail()
    {
        if(_details.Count <= 1)
        {
            Debug.LogError("We are trying to remove a part that is not there!");
            return;
        }

        var detail = _details[0];
        _details.Remove(detail);
        Destroy(detail.gameObject);
        _positionHistory.RemoveAt(_positionHistory.Count - 1);
        _rotationHistory.RemoveAt(_rotationHistory.Count - 1);
    }

    private void SetPlayerLayer(GameObject gameObject)
    {
        gameObject.layer = _playerLayer;
        var children = GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].gameObject.layer = _playerLayer;
        }
    }    
}

[Serializable]
public struct DetailPosition
{
    public float x;
    public float z;
}

[Serializable]
public struct DetailPositions
{
    public string id;
    public DetailPosition[] ds;
}
