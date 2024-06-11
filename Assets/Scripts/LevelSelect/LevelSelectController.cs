using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private GameObject lockedImage;
    [SerializeField] private int returnRange = 5;
    [SerializeField] private bool showLock;

    Material backgroundMat;
    Color startColor;
    Color tweenedColor;

    private const string BACKGROUND_COLOR = "_Water_Color";

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

        if (showLock)
        {
            rightButton.interactable = level < levelManager.PuzzleCount - 1;
            
            lockedImage.SetActive(level > levelManager.CompletedLevelCount);
            playButton.interactable = !lockedImage.activeInHierarchy;
            returnButton.gameObject.SetActive(level > (levelManager.CompletedLevelCount + returnRange));
        }
        else
        {
            rightButton.interactable = level < levelManager.CompletedLevelCount;
        }
    }

    public void MoveToLevelSelect()
    {
        Camera.main.transform.DOMove(levelSelectView.position, transitionTime).SetEase(ease);
        DOTween.To(() => tweenedColor, x => tweenedColor = x, startColor.DarkenedToPercent(0.46f), transitionTime).SetEase(ease);
    }

    public void MoveToGameView()
    {
        Camera.main.transform.DOMove(gameView.position, transitionTime).SetEase(ease);
        DOTween.To(() => tweenedColor, x => tweenedColor = x, startColor, transitionTime).SetEase(ease);
    }
}