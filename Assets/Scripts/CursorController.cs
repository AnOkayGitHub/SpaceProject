using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D mouseCursor;
    private CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        Vector2 cursorHotspot = new Vector2(mouseCursor.width / 2, mouseCursor.height / 2);
        Cursor.SetCursor(mouseCursor, cursorHotspot, cursorMode);
    }
}
