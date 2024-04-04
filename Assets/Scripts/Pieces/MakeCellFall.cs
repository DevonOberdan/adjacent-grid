using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCellFall : MonoBehaviour
{
    public void MakeFall(Cell cell)
    {
        if(!TryGetComponent(out Rigidbody rb))
        {
            rb = cell.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }
}
