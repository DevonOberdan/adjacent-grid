using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct CellConfigData
{
    public Cell Prefab;
    public Vector3 Pos;
    public Quaternion Rot;

    public CellConfigData(Cell cell, Vector3 pos, Quaternion rot)
    {
        this.Prefab = cell;
        this.Pos = pos;
        this.Rot = rot;
    }
}

[CreateAssetMenu(fileName ="GridPuzzleConfig", menuName ="New GridPuzzleConfig SO", order = 0)]
public class GridPuzzleConfigSO : ScriptableObject
{
    [SerializeField] List<GridPiece> pieceConfig;
    [SerializeField] private int solutionCount;

    [field: SerializeField]
    public List<CellConfigData> CellConfig { get; private set; }

    public int SolutionCount => solutionCount;
    public List<GridPiece> Pieces => pieceConfig;

    public void SetSolutionCount(int count)
    {
        solutionCount = count;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    
    public void SetPiecesConfig(List<GridPiece> pieces)
    {
        pieceConfig = pieces;
    }

    public void SetCellConfig(List<CellConfigData> cells)
    {
        CellConfig = cells;
    }
}
