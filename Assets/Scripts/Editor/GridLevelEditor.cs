using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(GridLevelManager))]
public class GridLevelEditor : Editor
{
    public VisualTreeAsset inspectorXML;
    public Material material;

    public override VisualElement CreateInspectorGUI()
    {
        GridLevelManager levelManager = target as GridLevelManager;
        VisualElement inspector = new VisualElement();
        inspectorXML.CloneTree(inspector);

        // Get a reference to the default inspector foldout control
        VisualElement inspectorFoldout = inspector.Q("Default_Inspector");

        Button generateButton = inspector.Q<Button>("Generate_Config_Button");
        generateButton.clicked += () => GenerateGridConfig(levelManager);

        // Attach a default inspector to the foldout
        InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);
        return inspector;
    }

    private void GenerateGridConfig(GridLevelManager levelManager)
    {
        List<GridPiece> gridPieceList = new();
        List<CellConfigData> cellCollection = new();

        levelManager.GridManager.GrabCells();
        levelManager.GridManager.GrabPieces();

        if (!Application.isPlaying)
            levelManager.GridManager.SetPiecesToGrid();

        foreach (Cell cell in levelManager.GridManager.Cells)
        {
            GridPiece piecePrefab = cell.CurrentPiece != null ? cell.CurrentPiece.GetPrefabFromSource() : null;
            gridPieceList.Add(piecePrefab);

            cellCollection.Add(new(levelManager.GridManager.DefaultCellPrefab, cell.transform.localPosition, cell.transform.localRotation));
        }

        GridPuzzleConfigSO puzzleConfig = ScriptableObject.CreateInstance<GridPuzzleConfigSO>();

        puzzleConfig.name = levelManager.NewPuzzleName;
        puzzleConfig.SetPiecesConfig(gridPieceList);
        puzzleConfig.SetCellConfig(cellCollection);

        SaveAsset(puzzleConfig);
    }

    private void SaveAsset(GridPuzzleConfigSO puzzleConfig)
    {
        string path = "Assets/ScriptableObjects/GridConfigs/";

        if (puzzleConfig.name.Equals(string.Empty))
            puzzleConfig.name = "NewGridConfig";

        string fullPath = path + puzzleConfig.name + ".asset";

        Debug.Log(fullPath);
        AssetDatabase.CreateAsset(puzzleConfig, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Successful puzzle config created at: "+fullPath);
    }
}
