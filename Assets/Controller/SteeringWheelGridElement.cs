using System;
using UnityEngine;

public class SteeringWheelGridElement : GridElement
{
    public Texture2D Texture { get; private set; }

    private Vector3 _wheelPosition { get; set; }
    public float CurrentAngle { get; private set; }

    public SteeringWheelGridElement(int inputCode, Vector2Int gridPosition, byte[] textureData)
    {
        Size = new Vector2Int(5, 3);
        InputCode = inputCode;
        Position = gridPosition;
        Texture = Texture2D.normalTexture;
        Texture.LoadImage(textureData);
    }

    public void OnUserTouchInteraction(Vector3 wheelPosition, Vector3 startTouchPos, Vector3 lastTouchPos)
    {
        Vector3 v1 = startTouchPos - wheelPosition;
        Vector3 v2 = lastTouchPos - wheelPosition;
        CurrentAngle = Mathf.Acos((v1.x * v2.x + v1.y * v2.y) / (v1.magnitude * v2.magnitude));
    }
    public override void OnUserInteraction()
    {
        NetworkMessageRequest.Invoke(CurrentAngle.ToString());
    }
}
