using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ArrowPieceTests;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

public class FreeformCellTests
{
    public struct FreeformCellTestCase
    {
        public int testNum;
        public int cellIndex;

        // up, right, down, left
        public List<int?> adjacentIndeces;

        public FreeformCellTestCase(int testNum, int cellIndex, List<int?> indeces)
        {
            this.testNum = testNum;
            this.cellIndex = cellIndex;
            this.adjacentIndeces = indeces;
        }
    }

    private static readonly List<FreeformCellTestCase> TestCases = new()
    {
        new(1, 0, new() {4, 1, null, null } ),
        new(1, 10, new() {14, 11, 6, 9 } ),
        new(1, 2, new() {6, 3, null, 1 } ),
        new(2, 0, new() {null, null, null, 15 } ),
    };


    private static readonly string UnitTestLevelPath = "Assets/Tests/Runtime/FreeformTestConfigs";
    private static string GetLevelPath(int num) => $"{UnitTestLevelPath}/{num}_FreeformTest.asset";

    private const float DELAY = 0.1f;

    [UnityTest]
    public IEnumerator ArrowPieceTest([ValueSource(nameof(TestCases))] FreeformCellTestCase testCase)
    {
        yield return TestUtilities.GetToGame();

        AdjacentGridGameManager adjacentManager = GameObject.FindAnyObjectByType<AdjacentGridGameManager>();
        GridLevelManager levelManager = GameObject.FindAnyObjectByType<GridLevelManager>();
        GridManager gridManager = levelManager.GridManager;

        var handle = Addressables.LoadAssetAsync<GridPuzzleConfigSO>(GetLevelPath(testCase.testNum));
        yield return handle;

        gridManager.PuzzleConfig = handle.Result;

        yield return new WaitForSeconds(DELAY);

        Cell cellToTest = levelManager.GridManager.Cells[testCase.cellIndex];

        List<int?> actualResult = cellToTest.AdjacentCells.Select(c => (int?)(c != null ? c.IndexInGrid : null)).ToList();

        Assert.AreEqual(testCase.adjacentIndeces, actualResult);
    }
}