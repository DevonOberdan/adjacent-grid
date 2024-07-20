using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FinishOne.UI
{
    [RequireComponent(typeof(Selectable))]
    public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent<bool> OnInteractionSet, OnHoverChanged;

        private bool interactable;
        public bool Interactable {
            get => interactable;
            set {
                interactable = value;
                button.interactable = value;
                OnInteractionSet?.Invoke(value);

                if(!Interactable)
                    OnHoverChanged?.Invoke(false);
            }
        }

        private Selectable button;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Interactable) return;

            OnHoverChanged?.Invoke(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Interactable) return;

            OnHoverChanged?.Invoke(false);
        }

        private void Awake()
        {
            TryGetComponent(out button);
            Interactable = button.interactable;
        }

        private void Start()
        {

        }

        private void Update()
        {

        }
    }
}