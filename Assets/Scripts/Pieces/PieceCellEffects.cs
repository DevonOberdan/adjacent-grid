using UnityEngine;

public class PieceCellEffects : MonoBehaviour
{
    private Cell pieceCell;

    private Color originalCellColor;

    public void MakeFall(Cell cell)
    {
        if(!cell.TryGetComponent(out Rigidbody rb))
        {
            rb = cell.gameObject.AddComponent<Rigidbody>();
        }

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    public void HideCellPlacementIndicator(Cell cell)
    {
        pieceCell = cell;


        if (TryGetComponent(out PieceIndicator3DCellHighlight cellHighlight))
        {
            originalCellColor = cellHighlight.StartingColor();
            Invoke(nameof(Hide), 0.01f);
        }
    }


    [ContextMenu(nameof(Hide))]
    public void Hide()
    {
        if (TryGetComponent(out PieceIndicator3DCellHighlight cellHighlight))
        {
            cellHighlight.SetStartingColor(Color.white);

            cellHighlight.ShowIndicator(false);
        }

        pieceCell.CanSetIndicatorColor = false;
    }

    private void OnDestroy()
    {
        if (pieceCell != null)
        {
            pieceCell.GetComponentInChildren<Collider>().enabled = true;
            pieceCell.CanSetIndicatorColor = true;
        }

        if (TryGetComponent(out PieceIndicator3DCellHighlight cellHighlight))
        {
            cellHighlight.SetStartingColor(originalCellColor);
            cellHighlight.ShowIndicator(false);
        }
    }
}
