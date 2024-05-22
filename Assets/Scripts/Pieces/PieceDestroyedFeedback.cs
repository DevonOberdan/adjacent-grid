using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceDestroyedFeedback : MonoBehaviour
{
    public virtual void HandleDestroyed(GridPiece destroyer)
    {
        gameObject.SetActive(false);
    }
}