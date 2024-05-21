using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextUtilities : MonoBehaviour
{
    private TMP_Text text;
    
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void WriteInt(int value) => text.text = value.ToString();
}