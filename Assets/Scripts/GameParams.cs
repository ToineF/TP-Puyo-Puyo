using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameParams")]
public class GameParams : ScriptableObject
{
    public float GameFallSpeed;
    public int GameTimer;
    public int GameTargetScore;
    public int ScoreByPuyo;
    [field:SerializeField] public List<Puyo> GamePuyos;
}
