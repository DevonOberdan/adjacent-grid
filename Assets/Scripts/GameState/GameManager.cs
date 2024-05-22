using FinishOne.GeneralUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputSystemUIInputModule inputModule;

    [Header("Game State Broadcasts")]
    [SerializeField] GameEvent RequestTogglePause;
    [SerializeField] BoolGameEvent RequestPauseState;

    private GameEventClassListener togglePauseListener;
    private GameEventClassListener<bool> pauseStateListener;

#region Pause State
    public UnityEvent<bool> PauseStateBroadcast;

    public UnityEvent<bool> PausableBroadcast;

    private bool paused;
    public bool Paused 
    {
        get => paused;
        set 
        {
            if (!CanPause)
                return;

            paused = value;
            PauseStateBroadcast?.Invoke(paused);
        }
    }

    public bool CanPause { get; private set; }
    #endregion

    private void Awake()
    {
        CanPause = true;

        if(inputModule == null)
        {
            Debug.LogError("InputSystemUIInputModule must be assigned to inputModule field.");
        }

        togglePauseListener = new GameEventClassListener(TogglePause);
        RequestTogglePause.RegisterListener(togglePauseListener);

        pauseStateListener = new GameEventClassListener<bool>(SetPausable);
        RequestPauseState.RegisterListener(pauseStateListener);
    }

    public void Pause() => Paused = true;
    public void Play() => Paused = false;
    public void TogglePause() => Paused = !Paused;

    public void SetPausable(bool pausable)
    {
        CanPause = pausable;
        PausableBroadcast.Invoke(pausable);
    }

    public void DisableInput(bool disable) => inputModule.enabled = !disable;
}