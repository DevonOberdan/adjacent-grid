using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIGridManager : MonoBehaviour
{
    [SerializeField] int gridSize = 4;

    [SerializeField] RectTransform cellContainer;

    [SerializeField] Color cellColor;

    const float strokeSize = 5f;

    float cellSize;


    public float CellSize => cellSize;

    List<Image> cells;

    List<Vector2> cellCenters;

    void Awake()
    {

        // store all cell transforms
        //cells = GetCells();


        //if (cellContainer == null)
        //    gridImage = GetComponent<Image>();

        cellSize = (cellContainer.rect.width - (strokeSize * (gridSize + 1))) / gridSize;

        cellCenters = CalculateCenters();


        print(cellSize);
    }


    List<Vector2> CalculateCenters()
    {
        List<Vector2> centers = new List<Vector2>();

        for(int i = 0; i<gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                Vector2 newCenter = new Vector2();

                newCenter.x = cellSize * j + (cellSize / 2) + ((j + 1) * strokeSize);
                newCenter.y = cellSize * i + (cellSize / 2) + ((i + 1) * strokeSize);

                centers.Add(newCenter);
            }
        }


        return centers;
    }


    public Vector3 GetClosestCell(RectTransform tile)
    {
        Vector2 tilePos = tile.anchoredPosition;
        print("tilePos: "+ tilePos);
        Vector2 closestCenter = cellCenters.OrderBy(cell => Vector2.Distance(tilePos, cell)).First();
        print(closestCenter);
        return closestCenter;
    }


    //public Transform GetClosestCell(Transform tile)
    //{
    //    //cells.Select(cell => Vector3.Distance(tile.position, cell.transform.position))
    //    //     ;

    //    //Image closestCell = cells.OrderBy(cell => Vector3.Distance(tile.position, cell.transform.position))
    //                             //.FirstOrDefault();
    //    //print(closestCell.name);
    //    // return closestCell != null ? closestCell.transform : null;
    //    float min = Mathf.Infinity;
    //    RectTransform closestCell = cells[^1].rectTransform;
    //    //print(tile.position);
    //    for (int i = 0; i < cells.Count; i++)
    //    {
    //        float distance = Vector3.Distance(tile.position, cells[i].rectTransform.position);
    //        print("cell: "+ cells[i].rectTransform.name +" ... position: " + cells[i].rectTransform.position);

    //        if (distance < min)
    //        {
    //            min = distance;
    //            closestCell = cells[i].rectTransform;
    //        }
    //    }

    //    //print("cell: "+closestCell.name +" ... position: " + closestCell.position + ", min: " + min);
    //    return closestCell;
    //}

    private void OnValidate()
    {
       // cells = GetCells();
       // cells.ForEach(cell => cell.color = cellColor);
    }


    List<Image> GetCells()
    {
        Image[] currentCells = cellContainer.GetComponentsInChildren<Image>();
        return currentCells.ToList();
    }



    void Update()
    {
        
    }
}
