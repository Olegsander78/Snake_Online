using Colyseus;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    public void SendMessage(string key, string data)
    {
        _room.Send(key, data);
    }

    private async void Connect()
    {
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"login", PlayerSettings.Instance.Login }
        };

        _room = await client.JoinOrCreate<State>(GAME_ROOM_NAME, data);

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

        _room.State.apples.ForEach(CreateApple);
        _room.State.apples.OnAdd += (key, apple) => CreateApple(apple);
        _room.State.apples.OnRemove += RemoveApple;
    }    
    #endregion

    #region Player

    [SerializeField] private Controller _controllerPrefab;
    [SerializeField] private Snake _snakePrefab;
    [SerializeField] private PlayerAim _playerAim;
    private void CreatePlayer(Player player)
    {
        var position = new Vector3(player.x, 0f, player.z);
        Quaternion quaternion = Quaternion.identity;

        Snake snake = Instantiate(_snakePrefab, position, quaternion);
        snake.Init(player.d, true);

        PlayerAim aim = Instantiate(_playerAim, position, quaternion);
        aim.Init(snake.Head, snake.Speed);

        Controller controller = Instantiate(_controllerPrefab);
        controller.Init(_room.SessionId, aim, player, snake);

        AddLeader(_room.SessionId, player);
    }
    #endregion

    #region Enemy
    private Dictionary<string, EnemyController> _enemiesMap = new Dictionary<string, EnemyController>();
    private void CreateEnemy(string key, Player player)
    {
        var position = new Vector3(player.x, 0f, player.z);
        
        Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
        snake.Init(player.d);

        EnemyController enemy = snake.AddComponent<EnemyController>();
        enemy.Init(key, player, snake);

        _enemiesMap.Add(key, enemy);

        AddLeader(key, player);
    }

    private void RemoveEnemy(string key, Player value)
    {
        RemoveLeader(key);
        
        if (_enemiesMap.ContainsKey(key) == false)
        {
            Debug.LogError("Enemy to destroy not found!");
            return;
        }
        EnemyController enemy = _enemiesMap[key];

        _enemiesMap.Remove(key);
        enemy.Destroy();
    }
    #endregion

    #region Apple
    [SerializeField] private Apple _applePrefab;
    [SerializeField] private Dictionary<Vector2Float, Apple> _applesMap = new Dictionary<Vector2Float, Apple>(); 
    private void CreateApple(Vector2Float vector2Float)
    {
        Vector3 position = new Vector3(vector2Float.x, 0f, vector2Float.z);
        var apple = Instantiate(_applePrefab, position, Quaternion.identity);
        apple.Init(vector2Float);
        _applesMap.Add(vector2Float, apple);
    }

    private void RemoveApple(int key, Vector2Float vector2Float)
    {
        if (_applesMap.ContainsKey(vector2Float) == false)
            return;

        var apple = _applesMap[vector2Float];
        _applesMap.Remove(vector2Float);
        apple.Destroy();
    }
    #endregion

    #region LeaderBoard
    private class LoginScorePair
    {
        public string login;
        public float score;
    }

    [SerializeField] private Text _text;

    Dictionary<string, LoginScorePair> _leadersMap = new Dictionary<string, LoginScorePair>();

    public void UpdateScore(string sessionId, int score)
    {
        if (_leadersMap.ContainsKey(sessionId) == false)
            return;

        _leadersMap[sessionId].score = score;

        UpdateBoard();
    }

    private void AddLeader(string sessionId, Player player)
    {
        if (_leadersMap.ContainsKey(sessionId))
            return;

        _leadersMap.Add(sessionId, new LoginScorePair
        {
            login = player.login,
            score = player.score
        });

        UpdateBoard();
    }

    private void RemoveLeader(string sessionId)
    {
        if (_leadersMap.ContainsKey(sessionId) == false)
            return;

        _leadersMap.Remove(sessionId);

        UpdateBoard();
    }

    private void UpdateBoard()
    {
        int topCount = Mathf.Clamp(_leadersMap.Count, 0, 8);
        var topBest = _leadersMap.OrderByDescending(pair => pair.Value.score).Take(topCount);

        string text = "";
        int i = 1;
        foreach (var item in topBest)
        {
            text += $"{i}. {item.Value.login}: {item.Value.score}\n";
            i++;

        }
        _text.text = text;
    }
    #endregion
}
