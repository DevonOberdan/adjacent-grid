using UnityEngine;

public class GridInputHandler : MonoBehaviour
{
    public static GridInputHandler Instance;

    [SerializeField] private GameObject visualIndicator;
    [SerializeField] private Color invalidPlacementColor;

    [SerializeField] private float dragCatchUpSpeed = 5f;

    public float DragCatchUpSpeed => dragCatchUpSpeed;
    public GameObject VisualIndicator => visualIndicator;

    public Color InvalidPlacementColor => invalidPlacementColor;

    private void Awake()
    {
        Instance = this;
    }
}