using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSwap : MonoBehaviour
{
    [SerializeField] Sprite active, inactive;

    private Image image;

    public void SetActive(bool value) => image.sprite = value ? active : inactive;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
}
