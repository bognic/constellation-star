using UnityEngine;

public class ButtonGridElement : GridElement
{
    public string Legend { get; private set; }
    public Texture2D Texture { get; private set; }

    public ButtonGridElement(int inputCode, Vector2Int gridPosition, string legend, byte[] textureData = null)
    {
        Size = new Vector2Int(1, 1);
        InputCode = inputCode;
        Position = gridPosition;
        Legend = legend;
        if (textureData is not null)
        {
            Texture = Texture2D.normalTexture;
            Texture.LoadImage(textureData);
        }
    }
}
