using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    public static CursorHandler Instance { get; private set; }

    [SerializeField] public Texture2D defaultCursor;
    [SerializeField] public Texture2D hoverCursor;
    [SerializeField] public Texture2D holdCursor;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void Default() => SetCursor(defaultCursor);
    public void Hover() => SetCursor(hoverCursor);
    public void Hold() => SetCursor(holdCursor);

    private void SetCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
    }
}