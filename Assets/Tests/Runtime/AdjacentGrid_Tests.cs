using System.Collections;
using System.Collections.Generic;
using FinishOne.SceneManagement;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AdjacentGrid_Tests
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator AdjacentGrid_CorrectSolutionCountFound()
    {
        yield return RunBot(0);

        GridManager gridManager = GameObject.FindAnyObjectByType<GridManager>();
        int count = gridManager.PuzzleConfig.SolutionCount;

        Assert.AreEqual(8, count);

        yield return null;
    }

    private IEnumerator RunBot(int level)
    {
        yield return GetToGame();

        GridLevelManager levelManager = GameObject.FindAnyObjectByType<GridLevelManager>();
        UniqueSolutionBot solutionBot = GameObject.FindAnyObjectByType<UniqueSolutionBot>();

        levelManager.SetLevelIndex(level);

        yield return new WaitForEndOfFrame();

        yield return solutionBot.StartCoroutine(solutionBot.SolvePuzzle());
    }

    private IEnumerator GetToGame()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(0);
        yield return new WaitForSeconds(.25f);
        GameObject.FindAnyObjectByType<SceneLoadRequester>().Request();
        yield return new WaitForSeconds(.25f);
    }
}
