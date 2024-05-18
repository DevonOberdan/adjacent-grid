using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ApplicationKeyPresses : MonoBehaviour
{
    [SerializeField] InputActionAsset inputAsset;
    private InputAction pauseAction;

    public bool Pressable {  get;  set; }

    [SerializeField] private UnityEvent OnPausePressed;

    private void Awake()
    {
        Pressable = true;

        inputAsset.Enable();
        pauseAction = inputAsset.FindActionMap("UI").FindAction("Pause");
    }

    private void Update()
    {
        if(Pressable && pauseAction.WasPressedThisFrame())
            PressPause();
    }

    public void PressPause()
    {
        OnPausePressed.Invoke();
    }
}