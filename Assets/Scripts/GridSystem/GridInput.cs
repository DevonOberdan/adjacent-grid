using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInput : MonoBehaviour
{

    Camera cam;

    [SerializeField] Grid grid;
    [SerializeField] GameObject tileIndicator;
    [SerializeField] LayerMask layerMask;

    private void Awake()
    {
        cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Update()
    {
        Vector3 gridMousePosition = GetMouseGridPosition();

        tileIndicator.SetActive(gridMousePosition != Vector3.zero);

        Vector3Int cellPos =  grid.WorldToCell(gridMousePosition);

        tileIndicator.transform.position = grid.CellToWorld(cellPos);
    }

    public Vector3 GetMouseGridPosition()
    {
        Vector3 gridPosition = new();

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.nearClipPlane;



        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);

        if (hit.collider != null)
            gridPosition = hit.point;

        //if (Physics2D.Raycast(ray.origin, ray.direction, 100))
        //{
        //    gridPosition = hit.point;
        //    print("hit grid: " + hit.point);
        //}

        return gridPosition;
        // raycast from camera
    }
}
