using FinishOne.GeneralUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(GridManager))]
public class GridInspector : Editor
{
    public VisualTreeAsset inspectorXML;

    [SerializeField] private Cell cellPrefab;
    [SerializeField] private SpriteRenderer boardPrefab;

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
            GenerateGrid(manager);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        };


        Button populateButton = inspector.Q<Button>("Populate_Grid");
        populateButton.clicked += () =>
        {
            GenerateRandomPieces(manager);
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

    private void GenerateRandomPieces(GridManager manager)
    {
        manager.ClearPieces();

        manager.GrabCells();


        foreach (Cell cell in manager.Cells)
        {
            GridPiece pieceToSpawn = manager.PiecePrefabs[Random.Range(0, manager.PiecePrefabs.Count)];

            if (pieceToSpawn != null)
            {
                GridPiece spawnedPiece = Instantiate(pieceToSpawn, manager.PieceParent);
                spawnedPiece.CurrentCell = cell;
            }
        }

        manager.GrabPieces();
        manager.SetPiecesToGrid();
    }


    private void ClearGrid(GridManager manager)
    {
        manager.ClearPieces();

        for (int i = manager.CellParent.childCount - 1; i >= 0; i--)
        {
            Cell cell = manager.CellParent.GetChild(i).GetComponent<Cell>();
            DestroyImmediate(cell.gameObject);
        }

        //cells = new List<Cell>();
        // destroy any and all boards
        for (int i = manager.transform.childCount - 1; i >= 0; i--)
        {
            if (manager.transform.GetChild(i) == manager.CellParent)
                continue;
            if (manager.transform.GetChild(i) == manager.PieceParent)
                continue;

            DestroyImmediate(manager.transform.GetChild(i).gameObject);
        }



        if (manager.Board != null)
        {
            DestroyImmediate(manager.Board);
            //board = null;
        }
    }

    private float Offset(int dimension) => (float)dimension / 2 - 0.5f;

    private void GenerateGrid(GridManager manager)
    {
        ClearGrid(manager);

        Vector2 center = new Vector2(Offset(manager.Width), Offset(manager.Height));
        manager.transform.position = -center;

        for (int i = 0; i < manager.Height; i++)
        {
            for (int j = 0; j < manager.Width; j++)
            {
                Cell newCell = CustomMethods.Instantiate(cellPrefab, manager.CellParent);
                newCell.transform.localPosition = new Vector2(j, i);
                //.Add(newCell);
            }
        }

        SpriteRenderer board = CustomMethods.Instantiate(boardPrefab, manager.transform);

        board.transform.localPosition = center;
        board.transform.localScale = new Vector2(manager.Width, manager.Width) + new Vector2(0.1f, 0.1f);
    }
}
