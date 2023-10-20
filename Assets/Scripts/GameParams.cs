using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameParams")]
public class GameParams : ScriptableObject
{
    public float GameFallSpeed;
    public int GameTimer; // not in GridManager
    public int GameTargetScore; // not in GridManager
    [field:SerializeField] public List<Puyo> GamePuyos;
}
