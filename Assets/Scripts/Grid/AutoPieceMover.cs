using UnityEngine;
using UnityEngine.Events;

public class AutoPieceMover : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> OnMovingChanged;

    public enum DIR { UP, RIGHT, DOWN, LEFT };
}