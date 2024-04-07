using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UniqueSolutionBot : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private AdjacentGridGameManager gameManager;
    [SerializeField] private GridHistoryManager historyManager;

    [SerializeField] private TMP_Text uniqueSolutionText;

    private void Awake()
    {
        gridManager.OnGridReset += BeginNewPuzzle;
    }

    private void BeginNewPuzzle()
    {
        //uniqueSolutionText.text = ""+0;

        uniqueSolutionText.text = ""+SolvePuzzle();
    }

    private int SolvePuzzle()
    {
        int count = 0;
        foreach(PieceGroup group in gameManager.CurrentGroups)
        {
            MoveGroup(group);
        }

        return count;
    }

    private void MoveGroup(PieceGroup group)
    {
        GridPiece drivingPiece = group.Group[0];

        if (drivingPiece == null)
            return;

        foreach(Cell cell in drivingPiece.CurrentCell.AdjacentCells)
        {
            drivingPiece.IndicatorCell = cell;

            if (drivingPiece.UserDropPiece())
            {

            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
