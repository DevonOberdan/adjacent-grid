using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

public class ArrowPieceTests
{
    public struct ArrowTestCase
    {
        public int solutionNum;
        public int levelIndex;
        public int pieceIndex;
        public AutoPieceMover.DIR direction;

        public ArrowTestCase(int solutionNum, int levelIndex, int pieceIndex, AutoPieceMover.DIR direction)
        {
            this.solutionNum = solutionNum;
            this.levelIndex = levelIndex;
            this.pieceIndex = pieceIndex;
            this.direction = direction;
        }
    }

    private static readonly List<ArrowTestCase> TestCases = new()
    {
        new(1, 16, 0, AutoPieceMover.DIR.RIGHT),
        new(2, 17, 0, AutoPieceMover.DIR.UP),
        new(3, 20, 15, AutoPieceMover.DIR.LEFT),
        new(4, 24, 0, AutoPieceMover.DIR.RIGHT),
        new(5, 24, 3, AutoPieceMover.DIR.LEFT),
        new(6, 24, 13, AutoPieceMover.DIR.RIGHT),

        //new(6, 25, 0, AutoPieceMover.DIR.RIGHT),
        //new(7, 27, 0, AutoPieceMover.DIR.RIGHT),
        //new(8, 29, 0, AutoPieceMover.DIR.RIGHT),
        //new(9, 30, 0, AutoPieceMover.DIR.RIGHT)
    };

    private static readonly string UnitTestLevelPath = "Assets/Tests/Runtime/ArrowTestResults/";
    private static string GetLevelPath(int num) => $"{UnitTestLevelPath}/{num}_CorrectResult.asset";

    private const float DELAY = 0.1f;

    [UnityTest]
    public IEnumerator ArrowPieceTest([ValueSource(nameof(TestCases))] ArrowTestCase testCase)
    {
        yield return TestUtilities.GetToGame();

        AdjacentGridGameManager adjacentManager = GameObject.FindAnyObjectByType<AdjacentGridGameManager>();
        GridLevelManager levelManager = GameObject.FindAnyObjectByType<GridLevelManager>();
        GridManager gridManager = levelManager.GridManager;

        levelManager.SetLevelIndex(testCase.levelIndex);
        GridPiece piece = levelManager.GridManager.Cells[testCase.pieceIndex].CurrentPiece;
        gridManager.PickedUpPiece(piece);

        yield return new WaitForSeconds(DELAY);

        adjacentManager.PickupGroupedPieces(piece);

        yield return new WaitForSeconds(DELAY);

        Cell nextCell = piece.CurrentCell.AdjacentCells[(int)testCase.direction];
        piece.IndicatorCell = nextCell;
        adjacentManager.MoveGroupIndicators(nextCell, true);

        yield return new WaitForSeconds(DELAY);

        piece.UserDropPiece();

        yield return new WaitForSeconds(3f);

        var handle = Addressables.LoadAssetAsync<GridPuzzleConfigSO>(GetLevelPath(testCase.solutionNum));
        
        yield return handle;

        IEnumerable<GridPiece> endGridPieces = gridManager.Cells.Select(cell => cell.CurrentPiece);
        List<GridPiece> expectedResult = handle.Result.Pieces;
        List<GridPiece> actualResult = endGridPieces.Select(p => p != null ? p.PrefabContainer.PiecePrefab : null).ToList();

        Assert.AreEqual(expectedResult, actualResult);
    }

}
