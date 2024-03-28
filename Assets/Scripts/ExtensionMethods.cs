using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public static class ExtensionMethods
{
    public static Renderer GrabRenderer(this GameObject go)
    {
        if (!go.TryGetComponent(out Renderer renderer))
        {
            renderer = go.GetComponentInChildren<Renderer>();

            if (renderer == null)
            {
                Debug.LogError("No Renderer in GridPiece", go);
            }
        }
        Debug.Log("Renderer: "+renderer.name, renderer.gameObject);
        return renderer;
    }

    public static void SetColor(this Renderer renderer, Color color)
    {
        MeshRenderer mesh = renderer as MeshRenderer;
        SpriteRenderer sprite = renderer as SpriteRenderer;

        if (mesh != null)
        {
            mesh.material.color = color;
        }
        else if (sprite != null)
        {
            sprite.color = color;
        }
    }

    public static Color GetColor(this Renderer rend)
    {
        Color color = Color.white;


        MeshRenderer mesh = rend as MeshRenderer;
        SpriteRenderer sprite = rend as SpriteRenderer;

        if (mesh != null)
        {
            color = Application.isPlaying ? mesh.material.color : mesh.sharedMaterial.color;
        }
        else if (sprite != null)
        {
            color = sprite.color;
        }
        else
        {
            Debug.LogError("Color not found in GameObject " + rend.name, rend.gameObject);
        }

        return color;
    }
}
