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
        (3, 14),
        (21, 19),
        (1, 2)
    };

    [UnityTest]
    public IEnumerator UniqueSolutionBotFindsCorrectCount([ValueSource(nameof(LevelSolutions))] (int levelIndex, int solutionCount) testCase)
    {
        yield return RunBot(testCase.levelIndex);
        Assert.AreEqual(testCase.solutionCount, GridManager.Instance.PuzzleConfig.SolutionCount);
        yield return null;
    }

    private IEnumerator RunBot(int levelIndex)
    {
        yield return TestUtilities.GetToGame();

        yield return new WaitForSeconds(0.05f);

        GridLevelManager levelManager = GameObject.FindAnyObjectByType<GridLevelManager>();
        UniqueSolutionBot solutionBot = GameObject.FindAnyObjectByType<UniqueSolutionBot>();
        levelManager.SetLevelIndex(levelIndex);

      // yield return new WaitForSeconds(DELAY);

        yield return solutionBot.SolvePuzzle();
    }
}
