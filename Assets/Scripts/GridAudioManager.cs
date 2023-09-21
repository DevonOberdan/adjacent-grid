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
        gridManager.OnPiecePickedUp += (piece) => PlayClip(pickupSound);
        gridManager.OnPieceDropped += HandlePieceDropped;
        gridManager.OnPieceIndicatorMoved += HandleIndicatorMoved;
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
        if (cell != gridManager.SelectedPiece.CurrentCell)
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