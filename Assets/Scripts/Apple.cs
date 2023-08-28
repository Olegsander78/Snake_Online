using Colyseus.Schema;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    private Vector2Float _apple;

    public void Init(Vector2Float apple)
    {
        _apple = apple;
        _apple.OnChange += OnChange;
    }    

    public void Collect()
    {
        Dictionary<string, object> data = new()
        {
            {"id", _apple.id }
        };

        MultiplayerManager.Instance.SendMessage("collect", data);
            
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        if (_apple != null)
            _apple.OnChange -= OnChange;

        Destroy(gameObject);
    }

    private void OnChange(List<DataChange> changes)
    {
        Vector3 position = transform.position;

        foreach (var change in changes)
        {
            switch (change.Field)
            {
                case "x":
                    position.x = (float)change.Value;
                    break;
                case "z":
                    position.z = (float)change.Value;
                    break;
                default:
                    Debug.LogWarning($"Apple does not respond to field changes: {change.Field}");
                    break;
            }
        }

        Debug.Log($"Apple has moved from position {transform.position} to {position}");
        transform.position = position;
        gameObject.SetActive(true);
    }
}
