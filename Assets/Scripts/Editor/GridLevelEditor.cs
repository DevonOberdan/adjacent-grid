using System.Collections.Generic;
using System.Linq;
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


        levelManager.GridManager.GrabCells();
        levelManager.GridManager.GrabPieces();
        levelManager.GridManager.SetPiecesToGrid();

        foreach (Cell cell in levelManager.GridManager.Cells)
        {
            if (cell.CurrentPiece != null)
            {
                GridPiece prefabPiece = levelManager.GridManager.PiecePrefabs
                                        .FirstOrDefault(piece =>
                                        piece.GetRenderer().GetColor()
                                        .Equals(cell.CurrentPiece.GetRenderer().GetColor()));

                gridPieceList.Add(prefabPiece);
            }
            else
                gridPieceList.Add(null);
        }

        GridPuzzleConfigSO puzzleConfig = ScriptableObject.CreateInstance<GridPuzzleConfigSO>();

        string name = levelManager.NewPuzzleName;
        puzzleConfig.SetPiecesConfig(gridPieceList);
        puzzleConfig.name = name;

        SaveAsset(puzzleConfig, name);
    }

    private void SaveAsset(GridPuzzleConfigSO puzzleConfig, string name)
    {
        string path = "Assets/ScriptableObjects/GridConfigs/";

        if (name.Equals(string.Empty))
            name = "NewGridConfig";

        string fullPath = path + name + ".asset";

        Debug.Log(fullPath);
        AssetDatabase.CreateAsset(puzzleConfig, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
