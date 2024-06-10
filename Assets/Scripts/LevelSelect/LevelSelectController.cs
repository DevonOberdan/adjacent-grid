using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private Transform gameView, levelSelectView;
    [SerializeField] private float transitionTime;
    [SerializeField] private float backgroundDarkenPerc;

    [SerializeField] private Ease ease;

    [SerializeField] private MeshRenderer background;
    Material backgroundMat;
    Color startColor;
    private float metallicStart;

    Color tweenedColor;

    private const string BACKGROUND_COLOR = "_Water_Color";

    private void Awake()
    {
        backgroundMat = background.material;
        startColor = backgroundMat.GetColor(BACKGROUND_COLOR);
        tweenedColor = startColor;
    }

    private void Update()
    {
        if(backgroundMat.GetColor(BACKGROUND_COLOR) != tweenedColor)
        {
            backgroundMat.SetColor(BACKGROUND_COLOR, tweenedColor);

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