using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="GridPuzzleConfig", menuName ="New GridPuzzleConfig SO", order = 0)]
public class GridPuzzleConfigSO : ScriptableObject
{
    [SerializeField] List<GridPiece> pieceConfig;

    [SerializeField] private int solutionCount;

    public void SetSolutionCount(int count)
    {
        solutionCount = count;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    public int SolutionCount => solutionCount;

    public List<GridPiece> Pieces => pieceConfig;

    public void SetPiecesConfig(List<GridPiece> pieces)
    {
        pieceConfig = pieces;
    }
}
