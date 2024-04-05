using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCellFall : MonoBehaviour
{
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
}
