using System.Collections;
using System.Collections.Generic;
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
        GridManager gridManager = GameObject.FindAnyObjectByType<GridManager>();
        Assert.AreEqual(8, gridManager.PuzzleConfig.SolutionCount);
        yield return null;
    }

    [UnityTest]
    public IEnumerator AdjacentGrid_CorrectSolutionCountFound_Level4()
    {
        yield return RunBot(3);
        GridManager gridManager = GameObject.FindAnyObjectByType<GridManager>();
        Assert.AreEqual(14, gridManager.PuzzleConfig.SolutionCount);
        yield return null;
    }

    [UnityTest]
    public IEnumerator AdjacentGrid_CorrectSolutionCountFound_Level22()
    {
        yield return RunBot(21);
        GridManager gridManager = GameObject.FindAnyObjectByType<GridManager>();
        Assert.AreEqual(19, gridManager.PuzzleConfig.SolutionCount);
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
        yield return SceneManager.LoadSceneAsync(0);
        yield return new WaitForSeconds(.25f);
        GameObject.FindAnyObjectByType<SceneLoadRequester>().Request();
        yield return new WaitForSeconds(.25f);
    }
}
