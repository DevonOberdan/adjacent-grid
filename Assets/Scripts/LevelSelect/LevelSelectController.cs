using DG.Tweening;
using FinishOne.GeneralUtilities;
using FinishOne.GeneralUtilities.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private float transitionTime;

    [SerializeField] private GameEvent OnEnterLevelSelect;
    [SerializeField] private Transform gameView, levelSelectView;
    [SerializeField] private Button leftButton, rightButton, playButton;
    [SerializeField] private GridLevelManager levelManager;
    [SerializeField] private PostProcessingController postProcess;

    [Header("Locked Levels")]
    [SerializeField] private GameObject lockedImage;
    [SerializeField] private Transform lockIcon;
    [SerializeField] private AudioConfigSO lockAudio;
    
    private int currentLevel;
    private bool entered = false;

    private AudioConfigSO startButtonAudio;
    private ButtonAudioHelper leftArrowAudio, rightArrowAudio;

    private Coroutine setButtonCoroutine;
    private Tween shakeTween;
    private Vector3 ShakeFactor;
    private Ease ease;

    private void Awake()
    {
        ShakeFactor = new Vector3(0, 0, 5);
        ease = Ease.OutBack;

        leftArrowAudio = leftButton.GetComponent<ButtonAudioHelper>();
        rightArrowAudio = rightButton.GetComponent<ButtonAudioHelper>();
        startButtonAudio = leftArrowAudio.AudioConfig;
    }

    private void Start()
    {
        levelManager.OnNewLevel.AddListener(ConfigureFromLevel);
    }

    public void ConfigureFromLevel(int level)
    {
        leftButton.interactable = level > 0;
        rightButton.interactable = level < levelManager.PuzzleCount - 1;

        bool currentLevelLocked = level > levelManager.CompletedLevelCount;

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

        bool lockedLeft = (level - 1) > levelManager.CompletedLevelCount;
        bool lockedRight = (level + 1) > levelManager.CompletedLevelCount;

        if (setButtonCoroutine != null)
        {
            StopCoroutine(setButtonCoroutine);
        }
        setButtonCoroutine = StartCoroutine(SetArrowSounds(lockedLeft, lockedRight));
    }

    private IEnumerator SetArrowSounds(bool lockedLeft, bool lockedRight)
    {
        yield return new WaitForEndOfFrame();
        leftArrowAudio.SetAudio(lockedLeft ? lockAudio : startButtonAudio);
        rightArrowAudio.SetAudio(lockedRight ? lockAudio : startButtonAudio);
    }

    public void MoveToLevelSelect()
    {
        currentLevel = levelManager.LevelIndex;

        levelManager.SetLevelIndex(currentLevel);
        postProcess.EnableVignette(true);

        Camera.main.transform.DOMove(levelSelectView.position, transitionTime).SetEase(ease);
        DOTween.To(() => postProcess.VignetteRange.x, x => postProcess.SetVignetteIntensity(x), postProcess.VignetteRange.y, transitionTime).SetEase(ease);
    }

    public void MoveToGameView()
    {
        Camera.main.transform.DOMove(gameView.position, transitionTime).SetEase(ease);
        DOTween.To(() => postProcess.VignetteRange.y, val => postProcess.SetVignetteIntensity(val), postProcess.VignetteRange.x, transitionTime)
               .SetEase(ease).OnComplete(() => postProcess.EnableVignette(false));

        postProcess.EnableDepthOfField(false);
    }

    public void CloseLevelSelect()
    {
        if (currentLevel >= 0 && currentLevel != levelManager.LevelIndex)
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
}