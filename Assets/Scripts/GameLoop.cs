using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private GameParams _gameParams;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _scoreText;
    private float _timer;
    private int _score;

    private void Start()
    {
        _timer = _gameParams.GameTimer;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        _timerText.text = ((int)_timer).ToString();
        _scoreText.text = _score.ToString();
    }

    public void AddScore(int amount)
    {
        _score += amount * _gameParams.ScoreByPuyo;
    }
}
