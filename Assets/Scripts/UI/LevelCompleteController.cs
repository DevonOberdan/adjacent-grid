using DG.Tweening;
using FinishOne.GeneralUtilities;
using TMPro;
using UnityEngine;

public class LevelCompleteController : MonoBehaviour
{
    [SerializeField] private RectTransform banner;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject buttonGroup;

    [SerializeField] private float openTime = 0.25f;

    [SerializeField] private float startHeight = 250;
    [SerializeField] private float endHeight = 450;
    [SerializeField] private Ease ease;
    private Sequence openSeq;

    [SerializeField] private bool tween;

    private void Awake()
    {
        banner.localScale = banner.localScale.NewX(0);
    }

    public void Open()
    {
        if (openSeq.IsActive())
        {
            openSeq.Kill();
        }

        openSeq = DOTween.Sequence();

        //ensure it starts closed
        banner.gameObject.SetActive(true);

        banner.localScale = banner.localScale.NewX(1);
        banner.sizeDelta = new Vector2(banner.sizeDelta.x, startHeight);



        Vector2 endSize = banner.sizeDelta;
        endSize.y = endHeight;

        text.enabled = true;
        //open

        if (tween)
        {
            openSeq//.Append(banner.DOScaleX(1, openTime).OnComplete(() => text.enabled = true))
                    .Append(banner.DOSizeDelta(endSize, openTime).SetEase(ease).OnComplete(() => buttonGroup.SetActive(true)));
        }
        else
        {
            banner.sizeDelta = endSize;
            buttonGroup.SetActive(true);
        }

    }

    public void Close()
    {
        banner.localScale = banner.localScale.NewX(0);
        banner.sizeDelta = new Vector2(banner.sizeDelta.x, startHeight);
        banner.gameObject.SetActive(false);
        text.enabled = false;
        buttonGroup.SetActive(false);
    }
}