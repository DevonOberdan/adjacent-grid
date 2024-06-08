using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private Transform gameView, levelSelectView;
    [SerializeField] private float time;
    private void Awake()
    {
        
    }

    public void CamToLevelSelectView()
    {
        Camera.main.transform.DOMove(levelSelectView.position, time);
    }

    public void CamToGameView()
    {
        Camera.main.transform.DOMove(gameView.position, time);

    }
}