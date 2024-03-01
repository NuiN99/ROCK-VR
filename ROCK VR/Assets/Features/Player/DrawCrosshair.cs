using UnityEngine;

public class DrawCrosshair : MonoBehaviour
{
    [SerializeField] Texture2D dotTexture;
    [SerializeField] float verticalOffset;

    [SerializeField] float size = 75f;

    void OnGUI()
    {
        float x = Screen.width / 2f - dotTexture.width / 2f;
        float y = Screen.height / 2f - dotTexture.height / 2f;
            
        GUI.DrawTexture(new Rect(x, y - verticalOffset, size, size), dotTexture);
    }
}
