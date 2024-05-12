using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventsOnStartup : MonoBehaviour
{
    private enum START_POINT { Awake, Start}
    [SerializeField] private START_POINT eventPoint;

    [SerializeField] private UnityEvent OnStartup;

    private void Awake()
    {
        if(eventPoint == START_POINT.Awake)
        {
            OnStartup.Invoke();
        }
    }

    private void Start()
    {
        if(eventPoint == START_POINT.Start)
        {
            OnStartup.Invoke();
        }
    }
}