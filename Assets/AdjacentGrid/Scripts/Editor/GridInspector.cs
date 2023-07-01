using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GridManager))]
public class GridInspector : Editor
{
    public VisualTreeAsset inspectorXML;

    public override VisualElement CreateInspectorGUI()
    {
        //DrawDefaultInspector();

        GridManager manager = target as GridManager;

        VisualElement inspector = new VisualElement();

        inspectorXML.CloneTree(inspector);

        // Get a reference to the default inspector foldout control
        VisualElement inspectorFoldout = inspector.Q("Default_Inspector");

        Button generateButton = inspector.Q<Button>("Generate_Button");
        generateButton.clicked += () =>
        {
            manager.GenerateGrid();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        };

        Button populateButton = inspector.Q<Button>("Populate_Grid");
        populateButton.clicked += () =>
        {
            manager.GeneratePieces();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        };

        Button clearGridButton = inspector.Q<Button>("Clear_Grid");
        clearGridButton.clicked += () =>
        {
            manager.ClearPieces();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        };


        // Attach a default inspector to the foldout
        InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

        return inspector;
    }
}
