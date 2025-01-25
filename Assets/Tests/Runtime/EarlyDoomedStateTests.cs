using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

public class EarlyDoomedStateTests
{
    private static readonly List<(int, bool)> DoomedTestLevels = new()
    {
        (1, true),
        (2, true),
        (3, true)
    };

    private static readonly string UnitTestLevelPath = "Assets/ScriptableObjects/GridConfigs/UnitTests";
    private static string GetLevelPath(int num) => $"{UnitTestLevelPath}/Test{num}.asset";

    [UnityTest]
    public IEnumerator EarlyDoomedStateTest([ValueSource(nameof(DoomedTestLevels))] (int levelNum, bool doomed) testCase)
    {
        yield return TestUtilities.GetToGame();

        var handle = Addressables.LoadAssetAsync<GridPuzzleConfigSO>(GetLevelPath(testCase.levelNum));

        yield return handle;

        GridPuzzleConfigSO level = handle.Result;
        GridManager.Instance.PuzzleConfig = level;

        yield return null;

        AdjacentGridGameManager gameManager = GameObject.FindAnyObjectByType<AdjacentGridGameManager>();
        Assert.AreEqual(testCase.doomed, gameManager.Doomed);

        yield return null;
    }
}
