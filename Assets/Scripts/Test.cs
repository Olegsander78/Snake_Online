using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private int _detailCount;
    [SerializeField] private Controller _controllerPrefab;
    [SerializeField] private Snake _snakePrefab;

    private Controller _controller;
    private Snake _snake;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (_snake)
                _snake.Destroy();

            if (_controller)
                Destroy(_controller.gameObject);

            _snake = Instantiate(_snakePrefab);
            _snake.Init(_detailCount);
            _controller = Instantiate(_controllerPrefab);
            _controller.Init(_snake);
        }
    }
}
