using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Puyo : MonoBehaviour
{
    public ColorType Color;
    public int SpawnRate;

    public enum ColorType
    {
        Red,
        Blue,
        Yellow,
        Green,
        Purple
    }
}
