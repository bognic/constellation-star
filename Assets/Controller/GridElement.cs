using System;
using UnityEngine;

public class GridElement
{
    public Vector2Int Position { get; protected set; } = Vector2Int.zero;
    public Vector2Int Size { get; protected set; } = Vector2Int.one;
    public int InputCode { get; protected set; }

    public Action<string> NetworkMessageRequest { get; set; }

    public virtual void OnUserInteraction()
    {
        NetworkMessageRequest.Invoke(InputCode.ToString());
    }
}
