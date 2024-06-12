using DG.Tweening;
using FinishOne.GeneralUtilities;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private GameEvent OnEnterLevelSelect;

    [SerializeField] private Transform gameView, levelSelectView;
    [SerializeField] private float transitionTime;
    [SerializeField] private float backgroundDarkenPerc;

    [SerializeField] private Ease ease;

    [SerializeField] private MeshRenderer background;
    [SerializeField] private Button leftButton, rightButton, playButton;

    [SerializeField] private GridLevelManager levelManager;
    [SerializeField] private PostProcessingController postProcess;
    [SerializeField] private GameObject lockedImage;
    [SerializeField] private bool showLock;

    [SerializeField] Transform lockIcon;
    [SerializeField] float lockShakeFactor;

    private Material backgroundMat;

   // private Tween camTween, waterTween, vignetteTween;

    private Tween shakeTween;
    private Vector3 ShakeFactor;

    private Color startColor, tweenedColor;

    private bool viewingLockedLevels;
    private int currentLevel;

    private const string BACKGROUND_COLOR = "_Water_Color";

    private bool entered = false;

    private void Awake()
    {
        backgroundMat = background.material;
        startColor = backgroundMat.GetColor(BACKGROUND_COLOR);
        tweenedColor = startColor;

        levelManager.OnNewLevel.AddListener(ConfigureFromLevel);

        ShakeFactor = new Vector3(0, 0, lockShakeFactor);
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

        rightButton.interactable = level < levelManager.PuzzleCount - 1;

        lockedImage.SetActive(currentLevelLocked);
        playButton.interactable = !currentLevelLocked;
        postProcess.EnableDepthOfField(currentLevelLocked);

        if (currentLevelLocked)
        {
            if (shakeTween.IsActive())
            {
                shakeTween.Kill();
                lockIcon.eulerAngles = lockIcon.eulerAngles.NewZ(0);
            }

            shakeTween = lockIcon.DOPunchRotation(ShakeFactor, 0.5f, 8);
        }

        // now viewing locked level
        if (currentLevelLocked && !viewingLockedLevels)
        {

        }

        viewingLockedLevels = currentLevelLocked;
    }

    public void MoveToLevelSelect()
    {
        currentLevel = levelManager.LevelIndex;
        levelManager.SetLevelIndex(currentLevel);
        postProcess.EnableVignette(true);

        if(Camera.main != null)
        {
            Camera.main.transform.DOMove(levelSelectView.position, transitionTime).SetEase(ease).OnComplete(EnteredLevelSelect);
        }
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

    public void StartInLevelSelect()
    {
        if (entered)
            return;

        entered = true;

        OnEnterLevelSelect.Raise();

    }

    private void EnteredLevelSelect()
    {

    }

    private void ExitedLevelSelect()
    {
        currentLevel = -1;
    }


    private void EnterLockedLevelView(bool viewingLockedLevel)
    {

    }
}