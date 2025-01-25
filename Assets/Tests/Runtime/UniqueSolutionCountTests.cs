using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UniqueSolutionCountTests
{
    private static readonly List<(int, int)> LevelSolutions = new()
    {
        (0, 8),
        (1, 2),
        (3, 14),
        (11, 70),
        (21, 19),
        (31, 724)
    };

    [UnityTest]
    public IEnumerator UniqueSolutionBotFindsCorrectCount([ValueSource(nameof(LevelSolutions))] (int levelIndex, int solutionCount) testCase)
    {
        yield return TestUtilities.GetToGame();
        yield return RunBot(testCase.levelIndex);
        Assert.AreEqual(testCase.solutionCount, GridManager.Instance.PuzzleConfig.SolutionCount);
        yield return null;
    }

    private IEnumerator RunBot(int levelIndex)
    {
        GridLevelManager levelManager = GameObject.FindAnyObjectByType<GridLevelManager>();
        UniqueSolutionBot solutionBot = GameObject.FindAnyObjectByType<UniqueSolutionBot>();
        levelManager.SetLevelIndex(levelIndex);

        yield return solutionBot.SolvePuzzle();
    }
}
