using FinishOne.GeneralUtilities.Audio;
using UnityEngine;

public class GridAudioManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    [Header("Audio Configs")]
    [SerializeField] private AudioConfigSO pickupAudio;
    [SerializeField] private AudioConfigSO dropAudio;
    [SerializeField] private AudioConfigSO failDropAudio;
    [SerializeField] private AudioConfigSO indicatorAudio;

    private AudioPlayRequester audioPlayRequester;
    private bool pieceMoved;

    private void Awake()
    {
        audioPlayRequester = GetComponent<AudioPlayRequester>();
    }

    private void Start()
    {
        gridManager.OnPiecePickedUp += HandlePiecePickedUp;
        gridManager.OnPieceDropped += HandlePieceDropped;
        gridManager.OnPieceIndicatorMoved += HandleIndicatorMoved;
    }

    private void HandlePiecePickedUp(GridPiece piece)
    {
        PlayAudioConfig(pickupAudio);
    }

    private void HandlePieceDropped(GridPiece piece, bool canDrop)
    {
        if (!pieceMoved)
            return;

        if (canDrop)
            PlayAudioConfig(dropAudio);
        else
            PlayAudioConfig(failDropAudio);

        pieceMoved = false;
    }


    private void HandleIndicatorMoved(Cell cell)
    {
        GridPiece selectedPiece = gridManager.SelectedPiece;

        if (selectedPiece != null && cell != selectedPiece.CurrentCell)
        {
            PlayAudioConfig(indicatorAudio);
            pieceMoved = true;
        }
        else
        {
            pieceMoved = false;
        }
    }
    
    private void PlayAudioConfig(AudioConfigSO audioConfig)
    {
        if(audioConfig == null) return;

        audioPlayRequester.Request(audioConfig);
    }
}