using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CellElement
{
    public Puyo CurrentPuyo;

    public CellElement(Type _type)
    {
        CellType = _type;
    }

    public enum Type
    {
        Empty,
        FallingPuyo,
        GroundedPuyo,
    }

    public Type CellType;
}
