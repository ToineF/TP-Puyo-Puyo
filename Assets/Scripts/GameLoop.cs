using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private GameParams _gameParams;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _targetScoreText;
    [SerializeField] private TMP_Text _winText;
    [SerializeField] private string _winDialogue;
    [SerializeField] private string _loseDialogue;
    [SerializeField] private GameObject _backgroundEnd;
    private float _timer;
    private int _score;
    private bool _hasEnded;
    bool _hasWon;

    private void Start()
    {
        _timer = _gameParams.GameTimer;
        _targetScoreText.text = _gameParams.GameTargetScore.ToString();
        _backgroundEnd.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (_hasEnded) return;

        if (_timer < 0)
        {
            _hasEnded = true;
            _gridManager.HasGameEnded = true;
            _backgroundEnd.transform.DOScale(Vector3.one * 10, 0.6f);
            _hasWon = (_score >= _gameParams.GameTargetScore);
            _winText.text = _hasWon ? _winDialogue : _loseDialogue;
        }

        _timer -= Time.deltaTime;
        string currentText = _timerText.text;
        _timerText.text = ((int)_timer).ToString();
        if (currentText != _timerText.text)
        {
            _timerText.transform.DOPunchScale(new Vector3(1,1,1) * 0.1f, 0.5f, 10, 0.8f);
        }
        _scoreText.text = _score.ToString();

        
    }

    public void AddScore(int amount)
    {
        _score += amount * _gameParams.ScoreByPuyo;
        _scoreText.transform.DOPunchScale(new Vector3(1, 1, 1) * 0.1f, 0.5f, 10, 0.8f);

    }

    public void Lose()
    {
        _score = int.MinValue;
        _timer = -1;
    }
}
