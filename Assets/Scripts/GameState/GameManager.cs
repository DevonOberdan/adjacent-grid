using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent<bool> PauseStateBroadcast;

    public UnityEvent<bool> PausableBroadcast;

    public void Pause() => Paused = true;
    public void Play() => Paused = false;
    public void TogglePause() => Paused = !Paused;

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

    public void SetPausable(bool pausable)
    {
        CanPause = pausable;
        PausableBroadcast.Invoke(pausable);
    }




    private void Awake()
    {
        CanPause = true;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}