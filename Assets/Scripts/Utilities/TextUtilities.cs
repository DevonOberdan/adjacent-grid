using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TextUtilities : MonoBehaviour
{
    private TMP_Text text;
    private float startFontSize;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        startFontSize = text.fontSize;
    }

    public void WriteInt(int value) => text.text = value.ToString();

    public void IncreaseFontSize(bool increase)
    {
        text.fontSize = increase ? startFontSize*1.08333f : startFontSize;
    }
}