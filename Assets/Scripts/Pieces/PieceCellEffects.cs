using UnityEngine;

public class PieceCellEffects : MonoBehaviour
{
    private Cell pieceCell;

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

    private void OnDisable()
    {
        ResetIndicator();
    }

    private void OnDestroy()
    {
        ResetIndicator();
    }

    private void ResetIndicator()
    {
        if (pieceCell != null)
        {
            pieceCell.GetComponentInChildren<Collider>().enabled = true;
            pieceCell.CanSetIndicatorColor = true;
        }
    }
}
