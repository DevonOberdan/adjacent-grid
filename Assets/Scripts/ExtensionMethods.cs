using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static void SetColor(this GameObject go, Color color)
    {
        MeshRenderer rend = go.GetComponentInChildren<MeshRenderer>();

        if (rend != null)
        {
            rend.material.color = color;
        }
        else
        {
            SpriteRenderer sprite = go.GetComponentInChildren<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.color = color;
            }
        }
    }

    public static Color GetColor(this GameObject go)
    {
        Color color = Color.white;
        MeshRenderer rend = go.GetComponentInChildren<MeshRenderer>();
        SpriteRenderer sprite = go.GetComponentInChildren<SpriteRenderer>();

        if (rend != null)
        {
            color = rend.material.color;
        }
        else if (sprite != null)
        {
            color = sprite.color;
        }
        else
        {
            Debug.LogError("Color not found in GameObject " + go.name, go);
        }

        return color;
    }
}
