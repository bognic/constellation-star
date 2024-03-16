using UnityEngine;

public class JoystickGridElement : GridElement
{
    public Vector2 StickPolarPosition { get; protected set; } = Vector2.zero;

    public JoystickGridElement(Vector2Int gridPosition)
    {
        Size = new Vector2Int(3, 3);
        Position = gridPosition;
    }
}
