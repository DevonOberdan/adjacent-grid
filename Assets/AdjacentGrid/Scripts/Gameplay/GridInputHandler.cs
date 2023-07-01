using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridInputHandler : MonoBehaviour
{
    public static GridInputHandler Instance;

    [SerializeField] SpriteRenderer visualIndicator;
    [SerializeField] Color invalidPlacementColor;

    [SerializeField] float dragCatchUpSpeed = 5f;

    public float DragCatchUpSpeed => dragCatchUpSpeed;
    public SpriteRenderer VisualIndicator => visualIndicator;

    public Color InvalidPlacementColor => invalidPlacementColor;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        print("Click occurred");
    }

}
