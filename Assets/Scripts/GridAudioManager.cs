using UnityEngine;

public class GridAudioManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    [Header("Clips")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip failDropSound;
    [SerializeField] private AudioClip indicatorSound;

    private AudioSource source;
    private bool pieceMoved;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        gridManager.OnPiecePickedUp += HandlePiecePickedUp;
        gridManager.OnPieceDropped += HandlePieceDropped;
        gridManager.OnPieceIndicatorMoved += HandleIndicatorMoved;
    }

    private void HandlePiecePickedUp(GridPiece piece)
    {
        PlayClip(pickupSound);
    }

    private void HandlePieceDropped(GridPiece piece, bool canDrop)
    {
        if (!pieceMoved)
            return;

        if (canDrop)
            PlayClip(dropSound);
        else
            PlayClip(failDropSound);

        pieceMoved = false;
    }


    private void HandleIndicatorMoved(Cell cell)
    {
        GridPiece selectedPiece = gridManager.SelectedPiece;
        if (selectedPiece != null && cell != selectedPiece.CurrentCell)
        {
            PlayClip(indicatorSound);
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
}