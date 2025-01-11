using System.Collections;
using FinishOne.SceneManagement;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AdjacentGrid_Tests
{
    [UnityTest]
    public IEnumerator AdjacentGrid_CorrectSolutionCountFound_Level1()
    {
        yield return RunBot(0);
        Assert.AreEqual(8, GridManager.Instance.PuzzleConfig.SolutionCount);
        yield return null;
    }

    [UnityTest]
    public IEnumerator AdjacentGrid_CorrectSolutionCountFound_Level4()
    {
        yield return RunBot(3);
        Assert.AreEqual(14, GridManager.Instance.PuzzleConfig.SolutionCount);
        yield return null;
    }

    [UnityTest]
    public IEnumerator AdjacentGrid_CorrectSolutionCountFound_Level22()
    {
        yield return RunBot(21);
        Assert.AreEqual(19, GridManager.Instance.PuzzleConfig.SolutionCount);
        yield return null;
    }

    private IEnumerator RunBot(int level)
    {
        yield return GetToGame();

        GridLevelManager levelManager = GameObject.FindAnyObjectByType<GridLevelManager>();
        UniqueSolutionBot solutionBot = GameObject.FindAnyObjectByType<UniqueSolutionBot>();
        levelManager.SetLevelIndex(level);

        yield return solutionBot.StartCoroutine(solutionBot.SolvePuzzle());
    }

    private IEnumerator GetToGame()
    {
        yield return SceneManager.LoadSceneAsync(0);

        while (!SceneManager.GetSceneByName("MainMenuScene").isLoaded)
            yield return new WaitForEndOfFrame();

        GameObject.FindAnyObjectByType<SceneLoadRequester>().Request();

        while (!SceneManager.GetSceneByName("GameScene").isLoaded)
            yield return new WaitForEndOfFrame();
    }
}
