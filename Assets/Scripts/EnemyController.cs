using Colyseus.Schema;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private string _clientId;
    private Player _player;
    private Snake _snake;

    public void Init(string clientId, Player player, Snake snake)
    {
        _clientId = clientId;
        _player = player;
        _snake = snake;

        _player.OnChange += Onchange;
    }

    public void Destroy()
    {
        _player.OnChange -= Onchange;
        _snake?.Destroy(_clientId);
    }

    private void Onchange(List<DataChange> changes)
    {
        var position = _snake.transform.position;

        for (int i = 0; i < changes.Count; i++)
        {
            switch (changes[i].Field)
            {
                case "x":
                    position.x = (float)changes[i].Value;
                    break;
                case "z":
                    position.z = (float)changes[i].Value;
                    break;
                case "d":
                    _snake.SetDetailCount((byte)changes[i].Value);
                    break;
                case "score":
                    MultiplayerManager.Instance.UpdateScore(_clientId, (ushort)changes[i].Value);
                    break;
                default:
                    Debug.LogWarning($"Field change not handled: {changes[i].Field}");
                    break;
            }
        }

        _snake.SetRotation(position);
    }    
}
