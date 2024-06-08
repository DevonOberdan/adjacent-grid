using FinishOne.GeneralUtilities.Audio;
using UnityEngine;

public class GridAudioManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    [Header("Clips")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip failDropSound;
    [SerializeField] private AudioClip indicatorSound;

    [Header("Audio Configs")]
    [SerializeField] private AudioConfigSO pickupAudio;
    [SerializeField] private AudioConfigSO dropAudio;
    [SerializeField] private AudioConfigSO failDropAudio;
    [SerializeField] private AudioConfigSO indicatorAudio;

    private AudioSource source;
    private AudioPlayRequester audioPlayRequester;
    private bool pieceMoved;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
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
        // PlayClip(pickupSound);
        PlayAudioConfig(pickupAudio);
    }

    private void HandlePieceDropped(GridPiece piece, bool canDrop)
    {
        if (!pieceMoved)
            return;

        if (canDrop)
        //    PlayClip(dropSound);
            PlayAudioConfig(dropAudio);

        else
          //  PlayClip(failDropSound);
            PlayAudioConfig(failDropAudio);


        pieceMoved = false;
    }


    private void HandleIndicatorMoved(Cell cell)
    {
        GridPiece selectedPiece = gridManager.SelectedPiece;
        if (selectedPiece != null && cell != selectedPiece.CurrentCell)
        {
           // PlayClip(indicatorSound);
            PlayAudioConfig(indicatorAudio);
            pieceMoved = true;
        }
        else
        {
            pieceMoved = false;
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;
        source.PlayOneShot(clip);
    }
    
    private void PlayAudioConfig(AudioConfigSO audioConfig)
    {
        if(audioConfig == null) return;

        audioPlayRequester.Request(audioConfig);
    }
}