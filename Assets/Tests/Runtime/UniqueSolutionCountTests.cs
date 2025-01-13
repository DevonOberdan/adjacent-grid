using System.Collections;
using System.Collections.Generic;
using FinishOne.SceneManagement;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        yield return GetToGame();

        GridLevelManager levelManager = GameObject.FindAnyObjectByType<GridLevelManager>();
        UniqueSolutionBot solutionBot = GameObject.FindAnyObjectByType<UniqueSolutionBot>();
        levelManager.SetLevelIndex(levelIndex);

        yield return solutionBot.SolvePuzzle();
    }

    private IEnumerator GetToGame()
    {
        yield return SceneManager.LoadSceneAsync(0);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("MainMenuScene").isLoaded);
        GameObject.FindAnyObjectByType<SceneLoadRequester>().Request();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("GameScene").isLoaded);
    }
}
