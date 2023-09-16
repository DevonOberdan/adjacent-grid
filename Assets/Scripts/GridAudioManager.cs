using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAudioManager : MonoBehaviour
{
    [SerializeField] GridManager gridManager;

    [Header("Clips")]
    [SerializeField] AudioClip pickupSound;
    [SerializeField] AudioClip dropSound;
    [SerializeField] AudioClip failDropSound;
    [SerializeField] AudioClip indicatorSound;

    AudioSource source;

    GridPiece selectedPiece;

    bool pieceMoved;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        gridManager.OnPiecePickedUp += (piece) => PlayClip(pickupSound);
        gridManager.OnPieceDropped += (piece, canDrop) =>
        {
            if (!pieceMoved)
                return;

            if (canDrop)
                PlayClip(dropSound);
            else
                PlayClip(failDropSound);

            pieceMoved = false;
        };


        gridManager.OnPieceIndicatorMoved += (cell) =>
        {
            if(cell != gridManager.SelectedPiece.CurrentCell)
            {
                PlayClip(indicatorSound);
                pieceMoved = true;
            }
            else
            {
                pieceMoved = false;
            }

        };


    }

    void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;
        print("playing "+clip.name);
        source.PlayOneShot(clip);
    }
}
