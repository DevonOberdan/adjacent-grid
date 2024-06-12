using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private Transform gameView, levelSelectView;
    [SerializeField] private float transitionTime;
    [SerializeField] private float backgroundDarkenPerc;

    [SerializeField] private Ease ease;

    [SerializeField] private MeshRenderer background;

    [SerializeField] private Button leftButton, rightButton, playButton, returnButton;

    [SerializeField] private GridLevelManager levelManager;
    [SerializeField] private PostProcessingController postProcess;
    [SerializeField] private GameObject lockedImage;
    [SerializeField] private int returnRange = 5;
    [SerializeField] private bool showLock;

    Material backgroundMat;
    Color startColor;
    Color tweenedColor;


    private bool viewingLockedLevels;
    
    private const string BACKGROUND_COLOR = "_Water_Color";

    List<GridPiece> currentPieces;
    GridPuzzleConfigSO currentConfig;
    int currentLevel;

    private void Awake()
    {
        backgroundMat = background.material;
        startColor = backgroundMat.GetColor(BACKGROUND_COLOR);
        tweenedColor = startColor;

        levelManager.OnNewLevel.AddListener(ConfigureFromLevel);
    }

    private void Update()
    {
        if(backgroundMat.GetColor(BACKGROUND_COLOR) != tweenedColor)
        {
            backgroundMat.SetColor(BACKGROUND_COLOR, tweenedColor);

        }
    }

    public void ConfigureFromLevel(int level)
    {
        leftButton.interactable = level > 0;

        bool currentLevelLocked = level > levelManager.CompletedLevelCount;

        if (showLock)
        {
            rightButton.interactable = level < levelManager.PuzzleCount - 1;
            
            lockedImage.SetActive(currentLevelLocked);
            playButton.interactable = !currentLevelLocked;
            returnButton.gameObject.SetActive(level > (levelManager.CompletedLevelCount + returnRange));

            postProcess.EnableDepthOfField(currentLevelLocked);
        }
        else
        {
            rightButton.interactable = level < levelManager.CompletedLevelCount;
        }

        viewingLockedLevels = currentLevelLocked;
    }

    public void MoveToLevelSelect()
    {
       // currentPieces = levelManager.GridManager.Pieces;
        //currentConfig = levelManager.GridManager.PuzzleConfig;
        currentLevel = levelManager.LevelIndex;

        levelManager.SetLevelIndex(currentLevel);

        postProcess.EnableVignette(true);

        Camera.main.transform.DOMove(levelSelectView.position, transitionTime).SetEase(ease).OnComplete(EnteredLevelSelect);
        DOTween.To(() => tweenedColor, x => tweenedColor = x, startColor.DarkenedToPercent(0.46f), transitionTime).SetEase(ease);
        DOTween.To(() => postProcess.VignetteRange.x, x => postProcess.SetVignetteIntensity(x), postProcess.VignetteRange.y, transitionTime).SetEase(ease);
    }

    public void MoveToGameView()
    {
        Camera.main.transform.DOMove(gameView.position, transitionTime).SetEase(ease).OnComplete(ExitedLevelSelect);
        DOTween.To(() => tweenedColor, val => tweenedColor = val, startColor, transitionTime).SetEase(ease);
        DOTween.To(() => postProcess.VignetteRange.y, val => postProcess.SetVignetteIntensity(val), postProcess.VignetteRange.x, transitionTime)
               .SetEase(ease).OnComplete(() => postProcess.EnableVignette(false));

        postProcess.EnableDepthOfField(false);
    }

    public void CloseLevelSelect()
    {
        if(currentLevel > 0 && currentLevel != levelManager.LevelIndex)
        {
            levelManager.SetLevelIndex(currentLevel);
        }
    }

    private void EnteredLevelSelect()
    {

    }

    private void ExitedLevelSelect()
    {
        currentPieces = null;
        currentConfig = null;
        currentLevel = -1;
    }


    private void EnterLockedLevelView(bool viewingLockedLevel)
    {

    }
}