using Colyseus;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiplayerManager : ColyseusManager<MultiplayerManager>
{    
    #region ServerSideCode

    private const string GAME_ROOM_NAME = "state_handler";

    private ColyseusRoom<State> _room;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        InitializeClient();
        Connect();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        LeaveRoom();
    }

    public void LeaveRoom()
    {
        _room?.Leave();
    }

    public void SendMessage(string key, Dictionary<string, object> data)
    {
        _room.Send(key, data);
    }

    private async void Connect()
    {
        _room = await client.JoinOrCreate<State>(GAME_ROOM_NAME);

        _room.OnStateChange += OnChange;
    }

    private void OnChange(State state, bool isFirstState)
    {
        if (isFirstState == false)
            return;
        _room.OnStateChange -= OnChange;

        state.players.ForEach((key, player) =>
        {
            if (key == _room.SessionId)
                CreatePlayer(player);
            else
                CreateEnemy(key, player);
        });

        _room.State.players.OnAdd += CreateEnemy;
        _room.State.players.OnRemove += RemoveEnemy;

    }
    #endregion

    #region Player

    [SerializeField] private Controller _controllerPrefab;
    [SerializeField] private Snake _snakePrefab;
    private void CreatePlayer(Player player)
    {
        var position = new Vector3(player.x, 0f, player.z);
        Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
        snake.Init(player.d);
        Controller controller = Instantiate(_controllerPrefab);
        controller.Init(snake);
    }
    #endregion

    #region Enemy
    private Dictionary<string, EnemyController> _enemies = new();
    private void CreateEnemy(string key, Player player)
    {
        var position = new Vector3(player.x, 0f, player.z);
        
        Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
        snake.Init(player.d);

        EnemyController enemy = snake.AddComponent<EnemyController>();
        enemy.Init(player, snake);

        _enemies.Add(key, enemy);
    }

    private void RemoveEnemy(string key, Player value)
    {
        if (_enemies.TryGetValue(key, out EnemyController enemy))
        {
            _enemies.Remove(key);
            enemy.Destroy();
        }
        else
        {
            Debug.LogError("Enemy to destroy not found!");
            return;
        }
    }
    #endregion
}